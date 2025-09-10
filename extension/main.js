var port = null;

connect();

chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
    if (request.type === "printRequest") {
        port.postMessage(request);
    }
});

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

function localizeHtmlPage()
{
    var objects = document.getElementsByTagName('html');
    for (var j = 0; j < objects.length; j++)
    {
        var obj = objects[j];

        var valStrH = obj.innerHTML.toString();
        var valNewH = valStrH.replace(/__MSG_(\w+)__/g, function(match, v1)
        {
            return v1 ? chrome.i18n.getMessage(v1) : "";
        });

        if(valNewH != valStrH)
        {
            obj.innerHTML = valNewH;
        }
    }
}
