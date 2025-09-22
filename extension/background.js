var port = null;

initializeStorage().then(() => {
    connect();
});

chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {    
    if (request.type === "PrintRequest" || request.type === "Profile") {
        if (port) {
            port.postMessage(request);
        } else {
            chrome.runtime.sendMessage({
                type: "Exception",
                message: "Error happened when sending native message"
            }).catch(() => {});
        }
    }
});

async function initializeStorage() {
    try {
        const result = await chrome.storage.local.get(['profiles']);
        
        if (!result.profiles || Object.keys(result.profiles).length === 0) {
            const defaultProfile = Profile.getDefaultProfile();
            
            const profiles = {
                'default': defaultProfile.toObject()
            };
            
            await chrome.storage.local.set({
                profiles: profiles,
                currentProfileName: 'default'
            });
            
            console.log("Default profile created in storage using Profile.getDefaultProfile()");
        }
    } catch (error) {
        console.error("Failed to initialize storage:", error);
    }
}

function onNativeMessage(message) {
    if (!message?.type) return;
    
    chrome.runtime.sendMessage(message).catch(() => {}); 

    console.log("Native message:", message);
}

function print(header, barcode) {
    if (port) {
        port.postMessage({
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
    const result = await chrome.storage.local.get('profiles');
    const profilesObject = result.profiles || {};
    
    const profilesArray = Object.values(profilesObject);
    
    port.postMessage({
        version: 1,
        type: "Profiles",
        profiles: profilesArray
    });
}
