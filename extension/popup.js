function showError(message) {
    const errorContainer = document.getElementById('errorContainer');
    const errorMessage = document.getElementById('errorMessage');
    
    errorMessage.textContent = message;
    errorContainer.style.display = 'block';
    
    setTimeout(() => {
        hideError();
    }, 10000);
}

function hideError() {
    const errorContainer = document.getElementById('errorContainer');
    errorContainer.style.display = 'none';
}

chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
    if (request.type === "exception" && request.messageText) {
        showError(request.messageText);
    }
});

document.addEventListener('DOMContentLoaded', function () {
    localizeHtmlPage();

    const printBtn = document.getElementById('printBtn');
    const settingsBtn = document.getElementById('settingsBtn');
    const closeBtn = document.getElementById('closeBtn');
    const headerInput = document.getElementById('headerInput');
    const barcodeInput = document.getElementById('barcodeInput');

    printBtn.addEventListener('click', handlePrint);

    settingsBtn.addEventListener('click', function () {
        chrome.runtime.openOptionsPage();
    });

    closeBtn.addEventListener('click', function () {
        window.close();
    });

    headerInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            barcodeInput.focus();
        }
    });

    barcodeInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            handlePrint();
        }
    });

    function handlePrint() {
        const header = headerInput.value.trim();
        const barcode = barcodeInput.value.trim();

        if (!header || !barcode) {
            alert('Please fill all fields');
            headerInput.focus();
            return;
        }

        chrome.runtime.sendMessage({
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

    headerInput.focus();
});
