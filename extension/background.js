var port = null;
var isConnected = false;

const PRINT_TYPE_KEY = "printTypeMapping";
const PROFILES_KEY = "profiles";

connect();

chrome.runtime.onMessage.addListener(async (request, sender, sendResponse) => {
    console.log(request);

    if (request.type === "CheckExtensionInstalled") {
    sendResponse({
        type: "CheckExtensionInstalledResponse",
        installed: true
    });

    chrome.tabs.query({ active: true, currentWindow: true }, (tabs) => {
        chrome.tabs.sendMessage(tabs[0].id, {
            type: "CheckExtensionInstalledResponse",
            installed: true
        });
    });

    return true;
}
    if (request.type == "CheckConnetcionToNativeApp") {
        sendResponse({
            type: "CheckConnetcionToNativeAppResponse",
            isConnected: isConnected
        });

        chrome.tabs.query({ active: true, currentWindow: true }, (tabs) => {
            chrome.tabs.sendMessage(tabs[0].id, {
                type: "CheckConnetcionToNativeAppResponse",
                isConnected: isConnected
            });
        });

        return true;
    }

    if (request.type == "GetPrintersRequest"){
        sendMessage(request);
        return;
    }

    if (request.type !== "PrintRequest")
        return;

    if (!isConnected) {
        chrome.runtime.sendMessage({
            type: "Exception",
            messageText: "Native app is not connected. Application is not installed or have errors in installed version."
        }).catch(() => { });
        return;
    }

    if (request.isFromExtension) {
        request = await handleMessageFromExtension(request);
    } else {
        request = await handleMessageFromSite(request);
    }
    request = await addProfile(request);

    console.log(request);
    sendMessage(request);
});

function checkConnection(request, sendResponse) {

}

function sendMessage(message) {
    if (port && isConnected) {
        port.postMessage(message);
    } else {
        chrome.runtime.sendMessage({
            type: "Exception",
            messageText: "Native application is not connected. Please make sure the native app is installed and running."
        }).catch(() => { });
    }
}

async function addProfile(message) {
    const result = await chrome.storage.local.get(PROFILES_KEY);
    const profilesObject = result.profiles || {};

    const profilesArray = Object.values(profilesObject);

    const profile = profilesArray.find(profile =>
        profile.profileName === message.profile
    );

    message.profile = profile;

    return message;
}

async function handleMessageFromSite(message) {
    try {
        const result = await chrome.storage.local.get(PRINT_TYPE_KEY);
        let mapping = result[PRINT_TYPE_KEY] || {};
        let printType = message.printType;

        if (!mapping[printType]) {
            mapping[printType] = "A4";
            await chrome.storage.local.set({ [PRINT_TYPE_KEY]: mapping });
        }

        return {
            type: message.type,
            version: message.version,
            profile: mapping[printType],
            items: message.items
        };
    } catch (error) {
        console.error("Error in handleMessageFromSite:", error);
        return message;
    }
}

async function handleMessageFromExtension(message) {

    return {
        type: message.type,
        version: message.version,
        profile: message.profile,
        items: message.items
    };
}

async function onNativeMessage(message) {
    if (!message?.type) return;

     console.log("Native message:", message);

    if (message.type == "UpdateNativeApp") {
        onDisconnected();

        await connect();
        return;
    }

    chrome.runtime.sendMessage(message).catch(() => { });
   
}

async function onDisconnected() {
    port = null;
    isConnected = false;

   await new Promise(resolve => setTimeout(resolve, 1000));

    await connect();
}

async function connect() {
    const hostName = "com.printadot"; // com узнать!! (если домен то ru)
    try {
        port = chrome.runtime.connectNative(hostName);
        port.onMessage.addListener(onNativeMessage);
        port.onDisconnect.addListener(onDisconnected);
        isConnected = true;
        console.log("Connected to native app");

        chrome.runtime.sendMessage({
            type: "NativeAppConnected"
        }).catch(() => { });
    } catch (error) {
        console.error("Connection failed:", error);
        isConnected = false;
    }
}