var port = null;
var isConnected = false;

const PRINT_TYPE_KEY = "printTypeMapping";
const PROFILES_KEY = "profiles";

connect();

chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
  if (request.type === "GET_PRINTERS") {
    if (chrome.printing && chrome.printing.getPrinters) {
      chrome.printing.getPrinters()
        .then(printers => sendResponse({ success: true, printers }))
        .catch(error => sendResponse({ success: false, error: error.message }));
    } else {
      sendResponse({ success: false, error: "Printing API not available" });
    }
    return true;
  }
});

chrome.runtime.onMessage.addListener(async (request, sender, sendResponse) => {
    if (request.type == "CheckConnetcionToNativeApp") {
        sendResponse({
            type: "CheckConnetcionToNativeAppResponse",
            isConnected: isConnected
        });
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

function onNativeMessage(message) {
    if (!message?.type) return;

    chrome.runtime.sendMessage(message).catch(() => { });
    console.log("Native message:", message);
}

function onDisconnected() {
    console.log("Disconnected");
    port = null;
    isConnected = false;

    chrome.runtime.sendMessage({
        type: "NativeAppDisconnected"
    }).catch(() => { });
}

async function connect() {
    const hostName = "com.printadot";
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