// content-script.js
console.log("Content script loaded for printing");

window.addEventListener("message", (event) => {
    if (event.data.type === 'printRequest') {
        console.log("Received print request from page:", event.data);
        chrome.runtime.sendMessage(event.data);
    }
});
