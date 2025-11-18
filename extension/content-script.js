window.addEventListener("message", (event) => {
    if (event.data.type === 'PrintRequest' || 
        event.data.type === "CheckExtensionInstalled" || 
        event.data.type === "CheckConnetcionToNativeApp") {
        chrome.runtime.sendMessage(event.data);
    }
});

chrome.runtime.onMessage.addListener((message) => {
    //add types
    window.postMessage(message, '*');
});
