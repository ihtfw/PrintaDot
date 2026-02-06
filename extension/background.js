import { Profile } from "./profile.js";

var port = null;
var isConnected = false;

const PRINT_TYPE_KEY = "printTypeMapping";
const PROFILES_KEY = "profiles";

const pendingMessages = new Map();
const DEFAULT_TIMEOUT = 30000;

connect();

chrome.runtime.onMessage.addListener(async (request, sender, sendResponse) => {
    return await handleRuntimeMessage(request, sendResponse);
});

async function initializeDefaultProfiles() {
    const result = await chrome.storage.local.get(['profiles']);
    let profiles = result.profiles || {};

    if (!profiles['A4']) {
        profiles['A4'] = Profile.getA4Profile().toStorageObject();
    }
    if (!profiles['Thermo']) {
        profiles['Thermo'] = Profile.getThermoProfile().toStorageObject();
    }

    const currentResult = await chrome.storage.local.get(['currentProfileName']);
    if (!currentResult.currentProfileName) {
        await chrome.storage.local.set({ currentProfileName: 'A4' });
    }

    await chrome.storage.local.set({ profiles });
}

async function handleRuntimeMessage(request, sendResponse) {
    switch (request.type) {
        case "CheckExtensionInstalled":
            sendResponseFromBackground({
                type: "CheckExtensionInstalledResponse",
                isConnected: true,
                messageIdToResponse: request.id,
            }, sendResponse);
            break;

        case "CheckConnetcionToNativeApp":
            sendResponseFromBackground({
                type: "CheckConnetcionToNativeAppResponse",
                isConnected: isConnected,
                messageIdToResponse: request.id
            }, sendResponse, request.isFromExtension);
            break;

        case "GetPrintersRequest":
            const printersResponse = await sendMessageWithResponse(request);
            sendResponse({
                ...printersResponse,
                messageIdToResponse: request.id
            });
            break;

        case "PrintRequest":
            const processedRequest = await handleMessage(request);
            const requestWithProfile = await addProfile(processedRequest);
            console.log(request);
            console.log(processedRequest);
             console.log(requestWithProfile);
            const printResponse = await sendMessageWithResponse(requestWithProfile);
            if (printResponse?.type === "Exception") {
                chrome.runtime.sendMessage({
                    ...printResponse,
                    messageIdToResponse: request.id
                });
            }
            break;
        default:
            return false;
    }

    return true;
}

function sendResponseFromBackground(response, sendResponse, isForExtension) {
    if (isForExtension) {
        sendResponse(response);
        return;
    } 
    
    chrome.tabs.query({ active: true, currentWindow: true }, (tabs) => {
        if (tabs[0]?.id) {
            chrome.tabs.sendMessage(tabs[0].id, response);
        }
    });
}

function sendMessageWithResponse(message, timeoutMs = DEFAULT_TIMEOUT) {
    return new Promise((resolve, reject) => {
        if (!port || !isConnected) {
            resolve({
                type: "Exception",
                messageText: "Native application is not connected. Please make sure the native app is installed and running.",
                messageIdToResponse: message.messageId
            });
            return;
        }

        const messageId = message.id;
        if (!messageId) {
            resolve({
                type: "Exception",
                messageText: "Message ID is missing in the request",
                messageIdToResponse: null
            });
            return;
        }

        const timeoutId = setTimeout(() => {
            pendingMessages.delete(messageId);
            resolve({
                type: "Exception",
                messageIdToResponse: messageId,
                messageText: `Request timeout after ${timeoutMs}ms`
            });
        }, timeoutMs);

        pendingMessages.set(messageId, {
            resolve,
            timeoutId,
            timestamp: Date.now()
        });

        try {
            port.postMessage(message);
            console.log("Sent message with ID:", messageId);
        } catch (error) {
            clearTimeout(timeoutId);
            pendingMessages.delete(messageId);
            resolve({
                type: "Exception",
                messageIdToResponse: messageId,
                messageText: `Failed to send message: ${error.message}`
            });
        }
    });
}

function sendMessage(message) {
    if (port && isConnected) {
        port.postMessage(message);
    } else {
        chrome.runtime.sendMessage({
            type: "Exception",
            messageText: "Native application is not connected. Please make sure the native app is installed and running.",
            messageIdToResponse: message.id
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

async function handleMessage(message) {
    if (message.isFromExtension) {
        return {
            type: message.type,
            options: message.options || null,
            version: message.version,
            profile: message.profile,
            items: message.items,
            id: message.id
        };
    }

    const result = await chrome.storage.local.get(PRINT_TYPE_KEY);
    let mapping = result[PRINT_TYPE_KEY] || {};
    let printType = message.printType;

    if (!mapping[printType]) {
        mapping[printType] = "Thermo";
        await chrome.storage.local.set({ [PRINT_TYPE_KEY]: mapping });
    }

    return {
        type: message.type,
        options: message.options || null,
        version: message.version,
        profile: mapping[printType],
        items: message.items,
        id: message.id
    };
}

async function onNativeMessage(message) {
    if (!message?.type) return;

    console.log("Native message:", message);

    if (message.messageIdToResponse) {
        const pendingMessage = pendingMessages.get(message.messageIdToResponse);

        console.log(pendingMessages);

        if (pendingMessage) {
            clearTimeout(pendingMessage.timeoutId);
            pendingMessages.delete(message.messageIdToResponse);
            pendingMessage.resolve(message);
        }
    }

    if (message.type == "UpdateNativeApp") {
        onDisconnected();
        await connect();
        return;
    }

    console.log("Sending message to content script:", message);
    chrome.runtime.sendMessage(message).catch((error) => {
        console.error("Failed to send message to content script:", error);
    });

    chrome.tabs.query({ active: true, currentWindow: true }, (tabs) => {
        if (tabs[0]?.id) {
            chrome.tabs.sendMessage(tabs[0].id, message).catch((error) => {
                // if reciving end does not exists do nothing
            });
        }
    });
}

async function onDisconnected() {
    for (const [messageId, pendingMessage] of pendingMessages.entries()) {
        clearTimeout(pendingMessage.timeoutId);
        pendingMessage.resolve({
            type: "Exception",
            messageIdToResponse: messageId,
            messageText: "Connection to native app was lost"
        });
    }
    pendingMessages.clear();

    port = null;
    isConnected = false;

    await new Promise(resolve => setTimeout(resolve, 1000));
    await connect();
}

async function connect() {
    const hostName = "com.printadot";
    try {
        port = chrome.runtime.connectNative(hostName);
        port.onMessage.addListener(onNativeMessage);
        port.onDisconnect.addListener(onDisconnected);
        isConnected = true;
        await initializeDefaultProfiles();
        console.log("Connected to native app");

        chrome.runtime.sendMessage({
            type: "NativeAppConnected"
        }).catch(() => { });
    } catch (error) {
        console.error("Connection failed:", error);
        isConnected = false;
        
        for (const [messageId, pendingMessage] of pendingMessages.entries()) {
            clearTimeout(pendingMessage.timeoutId);
            pendingMessage.resolve({
                type: "Exception",
                messageIdToResponse: messageId,
                messageText: `Failed to connect to native app: ${error.message}`
            });
        }
        pendingMessages.clear();
    }
}
