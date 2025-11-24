/**
 * Item entity for printing.
 * 
 * @typedef PrintItem
 * @property {string?} header - Header text
 * @property {string} barcode - Barcode string
 * @property {string?} figures - Figures string (optional)
 */
export class PrintItem {
    constructor(header, barcode, figures = null) {
        this.header = header;
        this.barcode = barcode;
        this.figures = figures;
    }
}

export function generateGuid() {
    return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
        const r = (Math.random() * 16) | 0;
        const v = c === "x" ? r : (r & 0x3) | 0x8;
        return v.toString(16);
    });
}

/**
 * Class for communicating with extension.
 * 
 * @typedef PrintaDotClient
 */
export class PrintaDotClient {
    constructor() {
        this._MESSAGE_TYPES = {
            CHECK_EXTENSION: "CheckExtensionInstalled",
            CHECK_NATIVE_APP: "CheckConnetcionToNativeApp",
            PRINT_REQUEST: "PrintRequest",
        };

        this._MESSAGE_RESPONSE_TYPES = {
            CHECK_EXTENSION: "CheckExtensionInstalledResponse",
            CHECK_NATIVE_APP: "CheckConnetcionToNativeAppResponse",
        };

        this._TIMEOUT_MS = 2000;

        this._pendingMessages = new Map();
        this._initResponseListener();
    }

    _initResponseListener() {
        if (typeof window === "undefined") return;

        window.addEventListener("message", (event) => {
            const data = event.data;

            if (data?.messageIdToResponse) {
                const pending = this._pendingMessages.get(data.messageIdToResponse);
                if (!pending) return;

                clearTimeout(pending.timeoutId);
                this._pendingMessages.delete(data.messageIdToResponse);

                if (data.type === "Exception") {
                    pending.reject(new Error(data.messageText));
                } else {
                    pending.resolve(data);
                }
            }
        });
    }

    _sendMessage(type, payload = {}, timeout = this._TIMEOUT_MS) {
        return new Promise((resolve, reject) => {
            const id = generateGuid();

            const timeoutId = setTimeout(() => {
                this._pendingMessages.delete(id);
                reject(new Error(`Request timeout after ${timeout}ms`));
            }, timeout);

            this._pendingMessages.set(id, { resolve, reject, timeoutId });

            try {
                window.postMessage({ type, id, ...payload }, "*");
            } catch (e) {
                clearTimeout(timeoutId);
                this._pendingMessages.delete(id);
                reject(new Error(`Failed to send message: ${e.message}`));
            }
        });
    }

    async checkExtensionConnection() {
        try {
            const response = await this._sendMessage(this._MESSAGE_TYPES.CHECK_EXTENSION);
            if (!response.isConnected) throw new Error("Extension is not connected");
            return response;
        } catch {
            throw new Error("Extension connection failed!");
        }
    }

    async checkNativeAppConnection() {
        try {
            const response = await this._sendMessage(this._MESSAGE_TYPES.CHECK_NATIVE_APP);
            if (!response.isConnected) throw new Error("Native application is not connected");
            return response;
        } catch {
            throw new Error("Native app connection failed!");
        }
    }

    async checkAllConnections() {
        await this.checkExtensionConnection();
        await this.checkNativeAppConnection();
    }

    /**
 * Send print request to PrintaDot extension.
 *
 * @param {PrintItem[]} items - array of PrintItem objects
 * @param {string} printType - print profile
 * @returns {Promise<void>}
 */
    async sendPrintRequest(items, printType = "default") {
        if (!Array.isArray(items)) throw new Error("items must be an array");
        if (items.length === 0) throw new Error("items array cannot be empty");

        // let's validate items on client
        for (const item of items) {
            if (item.header !== null && typeof item.header !== "string")
                throw new Error("header must be a string or null");

            if (typeof item.barcode !== "string" || !item.barcode)
                throw new Error("barcode must be a non-empty string");

            if (item.figures !== null && typeof item.figures !== "string")
                throw new Error("figures must be a string or null");
        }

        await this.checkAllConnections();

        // 2 minutes + 2 seconds per item
        const timeout = 2 * 60 * 1000 + items.length * 2000;

        await this._sendMessage(
            this._MESSAGE_TYPES.PRINT_REQUEST,
            {
                version: 1,
                printType,
                // let's map items to plain objects for serialization, so we don't accidently send more data than needed
                items: items.map(i => ({
                    header: i.header,
                    barcode: i.barcode,
                    figures: i.figures,
                }))
            },
            timeout
        );
    }
}
