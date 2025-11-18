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
        this.printType = printType;
        this.items = items;
    }

    toObject() {
        return {
            version: this.version,
            type: this.type,
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
    }

    checkConnection(messageType, responseType) {
        return new Promise((resolve) => {
            const timeout = setTimeout(() => {
                resolve(false);
            }, this.TIMEOUT_MS);

            window.postMessage({ type: messageType }, "*");

            const handler = (event) => {
                if (event.data?.type === responseType) {
                    clearTimeout(timeout);
                    window.removeEventListener("message", handler);
                    resolve(event.data.isConnected|| false);
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

    sendPrintRequest(printRequest) {
        window.postMessage(printRequest.toObject(), "*");
        return true;
    }
}

window.PrintaDotClient = PrintaDotClient;
window.PrintItem = PrintItem;
window.PrintRequest = PrintRequest;