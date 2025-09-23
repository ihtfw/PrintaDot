var port = null;

const PRINT_TYPE_KEY = "printTypeMapping";
const PROFILES_KEY = "profiles";


initializeStorage().then(() => {
    connect();
});

chrome.runtime.onMessage.addListener(async (request, sender, sendResponse) => {
    if (!request.type === "PrintRequest" && !request.type === "Profile")
        return;

    if (request.type === "PrintRequest" && request.printType) {
        request = await handleMessageFromSite(request);
    }

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

async function initializeStorage() {
    try {
        const result = await chrome.storage.local.get([PROFILES_KEY]);
        
        if (!result.profiles || Object.keys(result.profiles).length === 0) {
            const defaultProfile = Profile.getDefaultProfile();

            const profiles = {
                'default': defaultProfile.toObject()
            };

            await chrome.storage.local.set({
                profiles: profiles,
                currentProfileName: 'default'
            });
        }

    } catch (error) {
        console.error("Failed to initialize storage:", error);
    }
}

function onNativeMessage(message) {
    if (!message?.type) return;

    chrome.runtime.sendMessage(message).catch(() => { });

    console.log("Native message:", message);
}

function print(header, barcode) {
    sendMessage({
        type: "PrintRequest",
        version: 1,
        profile: "default",
        items: [
            {
                header: header,
                barcode: barcode
            }
        ]
    });
}

function sendProfileToNativeApp(profile) {
    sendMessage(profile.toObject());
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
    const result = await chrome.storage.local.get(PROFILES_KEY);
    const profilesObject = result.profiles || {};

    const profilesArray = Object.values(profilesObject);

    sendMessage({
        version: 1,
        type: "Profiles",
        profiles: profilesArray
    });
}