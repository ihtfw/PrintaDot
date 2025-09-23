window.addEventListener("message", (event) => {
    if (event.data.type === 'PrintRequest') {
        chrome.runtime.sendMessage(event.data);
    }
});
