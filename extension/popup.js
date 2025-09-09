document.addEventListener('DOMContentLoaded', function () {
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

        if (!header && !barcode) {
            alert('Please fill all fields');
            headerInput.focus();
            return;
        }

        try {
            print(header, barcode);
        } catch (error) {
            console.error('Print error:', error);
            alert('Print Error');
        }
    }

    headerInput.focus();
});