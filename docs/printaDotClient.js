/**
 * Item entity for printing.
 * 
 * @typedef PrintItem
 * @property {string?} header - Header text
 * @property {string} barcode - Barcode string
 * @property {string?} figures - Figures string (optional)
 */
class PrintItem {
    constructor(header, barcode, figures = null) {
        this.header = header;
        this.barcode = barcode;
        this.figures = figures;
    }
}

function generateGuid() {
  return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
    const r = (Math.random() * 16) | 0;
    const v = c == "x" ? r : (r & 0x3) | 0x8;
    return v.toString(16);
  });
}

/**
 * Class for communicating with extension.
 * 
 * @typedef PrintaDotClient
 */
class PrintaDotClient {
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
    window.addEventListener("message", (event) => {
      const data = event.data;

      if (data?.messageIdToResponse) {
        const pendingMessage = this._pendingMessages.get(
          data.messageIdToResponse
        );
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
        timeoutId,
      });

      try {
        window.postMessage(
          {
            type: messageType,
            id: messageId,
            ...messageData,
          },
          "*"
        );
      } catch (error) {
        clearTimeout(timeoutId);
        this._pendingMessages.delete(messageId);
        reject(new Error(`Failed to send message: ${error.message}`));
      }
    });
  }

  async checkExtensionConnection() {
    try {
      const response = await this._sendMessage(
        this._MESSAGE_TYPES.CHECK_EXTENSION
      );
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
      const response = await this._sendMessage(
        this._MESSAGE_TYPES.CHECK_NATIVE_APP
      );
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

  /**
   * Send print request to PrintaDot extension.
   *
   * @param {PrintItem[]} items - array of PrintItem objects
   * @param {string} printType - print profile
   * @returns {Promise<void>}
   */
  async sendPrintRequest(items, printType = "default") {
    if (typeof printType !== "string" || !printType) {
      throw new Error("printType must be a non-empty string");
    }

    if (!Array.isArray(items)) {
      throw new Error("items must be an array of PrintItem objects");
    }

    if (items.length === 0) {
      throw new Error("items array cannot be empty");
    }

    // let's validate items on client
    for (const item of items) {
      const header = item.header;
      if (header !== null && typeof header !== "string") {
        throw new Error("Each PrintItem header must be a string or null");
      }

      const barcode = item.barcode;
      if (typeof barcode !== "string" || !barcode) {
        throw new Error("Each PrintItem must have a valid barcode string");
      }

      const figures = item.figures;
      if (figures !== null && typeof figures !== "string") {
        throw new Error("Each PrintItem figures must be a string or null");
      }
    }

    await this.checkAllConnections();

    // 2 minutes + 2 seconds per item
    const timeoutMs = 2 * 60 * 1000 + items.length * 2 * 1000;

    await this._sendMessage(
      this._MESSAGE_TYPES.PRINT_REQUEST,
      {
        version: 1,
        printType: printType,
        // let's map items to plain objects for serialization, so we don't accidently send more data than needed
        items: items.map((x) => ({
          header: x.header,
          barcode: x.barcode,
          figures: x.figures,
        })),
      },
      timeoutMs
    );
  }
}
