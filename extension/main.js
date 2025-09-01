var port = null;

connect();

function sendNativeMessage(message) {
    if (port) {
        port.postMessage({"text": message});
    }
}

function print() {
    if (port) {
        port.postMessage({
            $type: "printRequestMessageV1",
            type: "PrintRequest",
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
    const command = message?.message;

    if (!command) {
        console.log("No data received");
        return;
    }
    
    const commandParams = command.split(" ");
    console.log("Extension message received:", commandParams[0]);
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
