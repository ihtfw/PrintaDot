// content-script.js
window.addEventListener("message", (event) => {
    if (event.data.type === 'PrintRequest') {
        console.log("Received print request from page:", event.data);
        chrome.runtime.sendMessage(event.data);
    }
});
