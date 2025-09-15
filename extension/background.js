var port = null;

connect();

chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {    
    if (request.type === "printRequest" || request.type === "profile") {
        if (port) {
            port.postMessage(request);
        } else {
            chrome.runtime.sendMessage({
                type: "exception",
                message: "Error happened when sending native message"
            }).catch(() => {});
        }
    }
});

function onNativeMessage(message) {
    if (!message?.type) return;
    
    chrome.runtime.sendMessage(message).catch(() => {}); 

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

function sendProfileToNativeApp(profile) {
    if (port && profile) {
        port.postMessage(profile.toObject());
    }
}

function onDisconnected() {
    console.log("Disconnected");
    port = null;
}

async function connect() {
    const hostName = "com.printadot";
    try {
        port = chrome.runtime.connectNative(hostName);
        port.onMessage.addListener(onNativeMessage);
        port.onDisconnect.addListener(onDisconnected);
        console.log("Connected to native app");

        await sendProfilesToNativeHost();
    } catch (error) {
        console.error("Connection failed:", error);
    }
}

async function sendProfilesToNativeHost() {
    const profiles = await chrome.storage.local.get('profiles');
    
        port.postMessage({
            version: 1,
            type: "profiles",
            profiles: null
        });
}
