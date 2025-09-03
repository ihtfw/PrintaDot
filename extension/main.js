var port = null;

connect();

chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
    if (request.type === "printRequest") {
        port.postMessage(request);
    }
});

function print() {
    if (port) {
        port.postMessage({
            $type: "printRequestMessageV1",
            type: "printRequest",
            version: 1,
            profile: "DefaultProfile",
            items: [
                {
                    header: "Sample Product",
                    barcode: "1234567890123",
                    figures: "Additional information"
                }
            ]
        });
    }
}

function onNativeMessage(message) {
    const command = message?.type;

    if (!command) {
        console.log("No data received");
        return;
    }
    
    const commandParams = command.split(" ");
    console.log(message);
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
