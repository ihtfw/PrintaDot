function generateGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        const r = Math.random() * 16 | 0;
        const v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

class PrintItem {
    constructor(header, barcode, figures = null) {
        this.header = header;
        this.barcode = barcode;
        this.figures = figures;
    }
}

class PrintRequest {
    constructor(printType = "default", items = []) {
        this.version = 1;
        this.type = "PrintRequest";
        this.id = generateGuid();
        this.printType = printType;
        this.items = items;
    }

    toObject() {
        return {
            version: this.version,
            type: this.type,
            id: this.id,
            printType: this.printType,
            items: this.items
        };
    }

    toJSON() {
        return JSON.stringify(this.toObject(), null, 2);
    }
}

class PrintaDotClient {
    constructor() {
        this._MESSAGE_TYPES = {
            CHECK_EXTENSION: "CheckExtensionInstalled",
            CHECK_NATIVE_APP: "CheckConnetcionToNativeApp",
            PRINT_REQUEST: "PrintRequest"
        };
        
        this._MESSAGE_RESPONSE_TYPES = {
            CHECK_EXTENSION: "CheckExtensionInstalledResponse",
            CHECK_NATIVE_APP: "CheckConnetcionToNativeAppResponse"
        };
        
        this._TIMEOUT_MS = 2000;
        
        this._pendingMessages = new Map();
        this._initResponseListener();
    }

    _initResponseListener() {
        window.addEventListener("message", (event) => {
            const data = event.data;

            if (data?.messageIdToResponse) {
                const pendingMessage = this._pendingMessages.get(data.messageIdToResponse);
                if (pendingMessage) {
                    clearTimeout(pendingMessage.timeoutId);
                    this._pendingMessages.delete(data.messageIdToResponse);
                    
                    if (data.type === "Exception") {
                        pendingMessage.reject(new Error(data.messageText));
                    } else {
                        pendingMessage.resolve(data);
                    }
                }
            }
        });
    }

    _sendMessage(messageType, messageData = {}, timeoutMs = this._TIMEOUT_MS) {
        return new Promise((resolve, reject) => {
            const messageId = generateGuid();
            
            const timeoutId = setTimeout(() => {
                this._pendingMessages.delete(messageId);
                reject(new Error(`Request timeout after ${timeoutMs}ms`));
            }, timeoutMs);

            this._pendingMessages.set(messageId, {
                resolve,
                reject,
                timeoutId
            });

            try {
                window.postMessage({
                    type: messageType,
                    id: messageId,
                    ...messageData
                }, "*");
            } catch (error) {
                clearTimeout(timeoutId);
                this._pendingMessages.delete(messageId);
                reject(new Error(`Failed to send message: ${error.message}`));
            }
        });
    }

    async checkExtensionConnection() {
        try {
            const response = await this._sendMessage(this._MESSAGE_TYPES.CHECK_EXTENSION);
            if (!response.isConnected) {
                throw new Error("Extension is not connected");
            }

            return response;
        } catch (error) {
            throw new Error(`Extension connection failed!`);
        }
    }

    async checkNativeAppConnection() {
        try {
            const response = await this._sendMessage(this._MESSAGE_TYPES.CHECK_NATIVE_APP);
            if (!response.isConnected) {
                throw new Error("Native application is not connected");
            }
            
            return response;
        } catch (error) {
            throw new Error(`Native app connection failed!`);
        }
    }

    async checkAllConnections() {
        await this.checkExtensionConnection();
        await this.checkNativeAppConnection();
    }

    calculateTimeout(printRequest) {
        const itemsCount = printRequest.items.length;
        return (2 * 60 * 1000) + (itemsCount * 2 * 1000);
    }
    
    /**
    * Method for sending print request to extension.
    * 
    * @param {PrintRequest} printRequest
    */
    async sendPrintRequest(printRequest) {
        if (!(printRequest instanceof PrintRequest)) {
            throw new Error("printRequest must be an instance of PrintRequest");
        }

        await this.checkAllConnections();

        const timeoutMs = this.calculateTimeout(printRequest);
        const message = printRequest.toObject();

        return this._sendMessage(this._MESSAGE_TYPES.PRINT_REQUEST, message, timeoutMs);
    }
}
