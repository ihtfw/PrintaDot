var port = null;

connect();

chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {    
    if (request.type === "printRequest") {
        if (port) {
            port.postMessage(request);
        } else {
            chrome.runtime.sendMessage({
                type: "nativeError",
                message: "Не подключено к native app"
            }).catch(() => {});
        }
    }
});

function onNativeMessage(message) {
    if (!message?.type) return;
    
    if (message.type === "exception") {
        chrome.runtime.sendMessage({
            type: "nativeError", 
            message: message.messageText
        }).catch(() => {}); 
    }
    
    console.log("Native message:", message);
}

function print(header, barcode) {
    if (port) {
        port.postMessage({
            type: "printRequest",
            version: 1,
            profile: "DefaultProfile",
            items: [
                {
                    header: header,
                    barcode: barcode
                }
            ]
        });
    }
}

function onDisconnected() {
    console.log("Disconnected");
    port = null;
}

function connect() {
    const hostName = "com.printadot";
    try {
        port = chrome.runtime.connectNative(hostName);
        port.onMessage.addListener(onNativeMessage);
        port.onDisconnect.addListener(onDisconnected);
        console.log("Connected to native app");
    } catch (error) {
        console.error("Connection failed:", error);
    }
}
