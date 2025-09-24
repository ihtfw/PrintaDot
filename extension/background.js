var port = null;

const PRINT_TYPE_KEY = "printTypeMapping";
const PROFILES_KEY = "profiles";

connect();

chrome.runtime.onMessage.addListener(async (request, sender, sendResponse) => {
    if (!request.type === "PrintRequest")
        return;

    request = await handleMessageFromSite(request);
    request = await addProfile(request);

    console.log(request);

    sendMessage(request);
});

function sendMessage(message) {
    if (port) {
        port.postMessage(message);
    } else {
        chrome.runtime.sendMessage({
            type: "Exception",
            message: "Error happened when sending native message"
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
            mapping[printType] = "default";
            await chrome.storage.local.set({[PRINT_TYPE_KEY]: mapping});
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

function onNativeMessage(message) {
    if (!message?.type) return;

    chrome.runtime.sendMessage(message).catch(() => { });

    console.log("Native message:", message);
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
    } catch (error) {
        console.error("Connection failed:", error);
    }
}
