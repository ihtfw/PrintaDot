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
        this.MESSAGE_TYPES = {
            CHECK_EXTENSION: "CheckExtensionInstalled",
            CHECK_NATIVE_APP: "CheckConnetcionToNativeApp",
            PRINT_REQUEST: "PrintRequest"
        };
        
        this.MESSAGE_RESPONSE_TYPES = {
            CHECK_EXTENSION: "CheckExtensionInstalledResponse",
            CHECK_NATIVE_APP: "CheckConnetcionToNativeAppResponse"
        };
        
        this.TIMEOUT_MS = 1000;
        this.PRINT_TIMEOUT_MS = 7000;
        
        this.pendingMessages = new Map();
        this.initResponseListener();
    }

    initResponseListener() {
        window.addEventListener("message", (event) => {
            const data = event.data;
            console.log(data);
            if (data?.messageIdToResponse) {
                const pendingMessage = this.pendingMessages.get(data.messageIdToResponse);
                if (pendingMessage) {
                    clearTimeout(pendingMessage.timeoutId);
                    this.pendingMessages.delete(data.messageIdToResponse);
                    
                    if (data.type === "Exception") {
                        pendingMessage.reject(new Error(data.messageText));
                    } else {
                        pendingMessage.resolve(data);
                    }
                }
            }
            
            if (data?.type === this.MESSAGE_RESPONSE_TYPES.CHECK_EXTENSION || 
                data?.type === this.MESSAGE_RESPONSE_TYPES.CHECK_NATIVE_APP) {
                return;
            }
        });
    }

    checkConnection(messageType, responseType) {
        return new Promise((resolve) => {
            const messageId = generateGuid();
            const timeout = setTimeout(() => {
                resolve(false);
            }, this.TIMEOUT_MS);

            window.postMessage({ 
                type: messageType,
                messageId: messageId 
            }, "*");

            const handler = (event) => {
                if (event.data?.type === responseType) {
                    clearTimeout(timeout);
                    window.removeEventListener("message", handler);
                    resolve(event.data.isConnected || false);
                }
            };

            window.addEventListener("message", handler);
        });
    }

    checkExtensionConnection() {
        return this.checkConnection(
            this.MESSAGE_TYPES.CHECK_EXTENSION,
            this.MESSAGE_RESPONSE_TYPES.CHECK_EXTENSION
        );
    }

    checkNativeAppConnection() {
        return this.checkConnection(
            this.MESSAGE_TYPES.CHECK_NATIVE_APP,
            this.MESSAGE_RESPONSE_TYPES.CHECK_NATIVE_APP
        );
    }

    async checkAllConnections() {
        const isExtensionConnected = await this.checkExtensionConnection();
        const isNativeAppConnected = await this.checkNativeAppConnection();
        
        return { isNativeAppConnected, isExtensionConnected };
    }

    async sendPrintRequest(printRequest, timeoutMs = this.PRINT_TIMEOUT_MS) {
        return new Promise((resolve, reject) => {
            const message = printRequest.toObject(); // проверять инстанс PrintObject если нет то ошибка
            
            const timeoutId = setTimeout(() => {
                this.pendingMessages.delete(message.id);
                reject(new Error(`Print request timeout after ${timeoutMs}ms`));
            }, timeoutMs);

            this.pendingMessages.set(message.id, {
                resolve: (response) => {
                    clearTimeout(timeoutId);
                    resolve(response);
                },
                reject: (error) => {
                    clearTimeout(timeoutId);
                    reject(error);
                }
            });

            try {
                window.postMessage(message, "*");
                console.log("Sent print request with ID:", message.id);
                console.log("Waiting for response with messageIdToResponse:", message.id);
            } catch (error) {
                clearTimeout(timeoutId);
                this.pendingMessages.delete(message.id);
                reject(new Error(`Failed to send print request: ${error.message}`));
            }
        });
    }
}
