document.addEventListener("DOMContentLoaded", async () => {

    const printTypeInput = document.getElementById('printType');
    const duplicateBarcodeCheckbox = document.getElementById('duplicateBarcode');
    const itemsContainer = document.getElementById('itemsContainer');
    const addItemButton = document.getElementById('addItem');
    const itemCountSelect = document.getElementById('itemCount');
    const sendButton = document.getElementById('sendBtn');
    const clearButton = document.getElementById('clearBtn');
    const jsonOutput = document.getElementById('jsonOutput');
    const messageDiv = document.getElementById('message');

    const client = new PrintaDotClient();

    function createItemElement(index) {
        const div = document.createElement("div");
        div.className = "item";

        const defaultHeader = `Item ${index}`;
        const defaultBarcode = `1234567890${index}`;
        const defaultFigures = "";

        div.innerHTML = `
            <div class="item-header">
                <span class="item-number">Item ${index}</span>
                <button class="remove-item" type="button">×</button>
            </div>

            <div class="input-group">
                <label for="header${index}">Header</label>
                <input type="text" id="header${index}" placeholder="Enter header" value="${defaultHeader}">
            </div>

            <div class="input-group">
                <label for="barcode${index}">Barcode</label>
                <input type="text" id="barcode${index}" placeholder="Enter barcode" value="${defaultBarcode}">
            </div>

            <div class="input-group">
                <label for="figures${index}">Figures</label>
                <input type="text" id="figures${index}" placeholder="Enter text (optional)" value="${defaultFigures}">
            </div>
        `;

        div.querySelector(".remove-item").addEventListener("click", () => {
            div.remove();
            updateItemNumbers();
            updateJSON();
        });

        div.querySelectorAll("input").forEach(i => i.addEventListener("input", updateJSON));

        return div;
    }

    function updateItemNumbers() {
        document.querySelectorAll(".item").forEach((item, i) => {
            item.querySelector(".item-number").textContent = `Item ${i + 1}`;
        });
    }

    function addItem() {
        const count = document.querySelectorAll(".item").length + 1;
        itemsContainer.appendChild(createItemElement(count));
        updateJSON();
    }

    function defineMessage(connection) {
        if (!connection.isExtensionConnected){
            return "Extension is not connected";
        } else if (connection.isNativeAppConnected){
            return "Native app is not installed or working properetly";
        } 

        return "Print request send successfully";
    }

    function updateJSON() {
        const request = fromDOM(
            printTypeInput, 
            duplicateBarcodeCheckbox, 
            itemsContainer
        );

        jsonOutput.textContent = request.toJSON(true);
    }

    function  fromDOM(printTypeInput, duplicateBarcodeCheckbox, itemsContainer) {
        const printType = printTypeInput.value.trim() || "default";
        const duplicateBarcode = duplicateBarcodeCheckbox.checked;

        const items = [];
        const itemElements = itemsContainer.querySelectorAll(".item");

        itemElements.forEach((itemEl, index) => {
            const i = index + 1;

            const header = itemEl.querySelector(`#header${i}`).value.trim() || `Item Header ${i}`;
            const barcode = itemEl.querySelector(`#barcode${i}`).value.trim() || `1234567890${i}`;
            let figures = itemEl.querySelector(`#figures${i}`).value.trim();

            if (duplicateBarcode && !figures) {
                figures = barcode;
            }

            items.push(new PrintItem(header, barcode, figures || null));
        });

        return new PrintRequest(printType, items);
    }

    function showMessage(text, type = "success") {
        messageDiv.textContent = text;
        messageDiv.className = "message " + type;
        messageDiv.style.display = "block";
        setTimeout(() => { messageDiv.style.display = "none"; }, 2500);
    }

    addItemButton.addEventListener("click", addItem);

    itemCountSelect.addEventListener("change", () => {
        const target = Number(itemCountSelect.value);
        let current = itemsContainer.querySelectorAll(".item").length;

        while (current < target) { addItem(); current++; }
        while (current > target) {
            itemsContainer.lastElementChild.remove();
            current--;
        }
        updateItemNumbers();
        updateJSON();
    });

    sendButton.addEventListener("click", async () =>  {
        var connection = await client.checkAllConnections();

        console.log(connection);

        if (!connection.isExtensionConnected || !connection.isNativeAppConnected) {
            var message = defineMessage(connection);
            showMessage(message);

            return;
        }

        const request = fromDOM(
            printTypeInput,
            duplicateBarcodeCheckbox,
            itemsContainer
        );
        
        client.sendPrintRequest(request);

        showMessage("Print request send successfully");
    });

    clearButton.addEventListener("click", () => {
        printTypeInput.value = "default";
        duplicateBarcodeCheckbox.checked = false;
        itemsContainer.innerHTML = "";
        addItem();
        itemCountSelect.value = 1;
        updateJSON();
    });

    printTypeInput.addEventListener("input", updateJSON);
    duplicateBarcodeCheckbox.addEventListener("change", updateJSON);

    printTypeInput.value = "default";
    addItem();
});