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
        
        this._TIMEOUT_MS = 1000;
        this._PRINT_TIMEOUT_MS = 120000;
        
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
            
            if (data?.type === this._MESSAGE_RESPONSE_TYPES.CHECK_EXTENSION || 
                data?.type === this._MESSAGE_RESPONSE_TYPES.CHECK_NATIVE_APP) {
                return;
            }
        });
    }

    _checkConnection(messageType, responseType) {
        return new Promise((resolve) => {
            const messageId = generateGuid();
            const timeout = setTimeout(() => {
                resolve({
                    type: "Exception",
                    messageIdToResponse: messageId,
                    isConnected: false
                });
            }, this._TIMEOUT_MS);

            window.postMessage({ 
                type: messageType,
                messageId: messageId 
            }, "*");

            const handler = (event) => {
                if (event.data?.type === responseType) {
                    clearTimeout(timeout);
                    window.removeEventListener("message", handler);
                    resolve(event.data);
                }
            };

            window.addEventListener("message", handler);
        });
    }

    async checkExtensionConnection() {
        const response = await this._checkConnection(
            this._MESSAGE_TYPES.CHECK_EXTENSION,
            this._MESSAGE_RESPONSE_TYPES.CHECK_EXTENSION,
        );

        if (!response.isConnected) {
            response.type = "Exception";
            response.messageText = "Extension is not connected";
            return response;
        }

        response.messageText = "Extension connected successfully";
        return response;
    }

    async checkNativeAppConnection() {
        const response = await this._checkConnection(
            this._MESSAGE_TYPES.CHECK_NATIVE_APP,
            this._MESSAGE_RESPONSE_TYPES.CHECK_NATIVE_APP
        );

        if (!response.isConnected) {
            response.type = "Exception";
            response.messageText = "Native application is not connected";
            return response;
        }
        
        response.messageText = "Native application connected successfully";
        return response;
    }

    async checkAllConnections() {
        const extensionConnectionMessage = await this.checkExtensionConnection();
        const nativeAppConnectionMessage = await this.checkNativeAppConnection();
        
        if (!extensionConnectionMessage.isConnected) {
            return extensionConnectionMessage;
        } else if (!nativeAppConnectionMessage.isConnected) {
            return nativeAppConnectionMessage;
        }

        return {
            type: "CheckAllConnectionsResponse",
            messageText: "Extension and Native app connected successfully",
            isConnected: true,
        };
    }

    calculateTimeout(printRequest) {
        const itemsCount = printRequest.items.length;
        return (2 * 60 * 1000) + (itemsCount * 2 * 1000);
    }

    async sendPrintRequest(printRequest) {
        const connections = await this.checkAllConnections();
        if (!connections.isConnected) {
            return connections;
        }

        const timeoutMs = this.calculateTimeout(printRequest);

        return new Promise((resolve, reject) => {
            if (!(printRequest instanceof PrintRequest)) {
                reject(new Error("printRequest must be an instance of PrintRequest"));
                return;
            }
            
            const message = printRequest.toObject();
            
            const timeoutId = setTimeout(() => {
                this._pendingMessages.delete(message.id);
                reject(new Error(`Print request timeout after ${timeoutMs}ms`));
            }, timeoutMs);

            this._pendingMessages.set(message.id, {
                resolve: (response) => {
                    clearTimeout(timeoutId);
                    resolve(response);
                },
                reject: (error) => {
                    clearTimeout(timeoutId);
                    reject(error);
                },
                timeoutId: timeoutId
            });

            try {
                window.postMessage(message, "*");
            } catch (error) {
                clearTimeout(timeoutId);
                this._pendingMessages.delete(message.id);
                reject(new Error(`Failed to send print request: ${error.message}`));
            }
        });
    }
}