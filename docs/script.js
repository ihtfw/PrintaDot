import { PrintaDotClient, PrintItem } from "../packages/printadot/src/printaDotClient.js";

document.addEventListener("DOMContentLoaded", async () => {
    const printTypeInput = document.getElementById('printType');
    const duplicateBarcodeCheckbox = document.getElementById('duplicateBarcode');
    const itemsContainer = document.getElementById('itemsContainer');
    const addItemButton = document.getElementById('addItem');
    const itemCountSelect = document.getElementById('itemCount');
    const sendButton = document.getElementById('sendBtn');
    const clearButton = document.getElementById('clearBtn');
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
        });

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
    }

    function updateFiguresFields() {
        const itemElements = itemsContainer.querySelectorAll(".item");
        
        itemElements.forEach((itemEl, index) => {
            const i = index + 1;
            const barcodeInput = itemEl.querySelector(`#barcode${i}`);
            const figuresInput = itemEl.querySelector(`#figures${i}`);
            
            if (duplicateBarcodeCheckbox.checked) {
                figuresInput.value = barcodeInput.value;
            } else {
                if (figuresInput.value === barcodeInput.value) {
                    figuresInput.value = "";
                }
            }
        });
    }

    function getItemsFromDOM() {
        const items = [];
        const itemElements = itemsContainer.querySelectorAll(".item");

        itemElements.forEach((itemEl, index) => {
            const i = index + 1;

            const header = itemEl.querySelector(`#header${i}`).value.trim() || `Item Header ${i}`;
            const barcode = itemEl.querySelector(`#barcode${i}`).value.trim() || `1234567890${i}`;
            let figures = itemEl.querySelector(`#figures${i}`).value.trim();

            if (duplicateBarcodeCheckbox.checked && !figures) {
                figures = barcode;
            }

            items.push(new PrintItem(
                header || null,
                barcode,
                figures || null
            ));
        });

        return items;
    }

    function showMessage(text, type = "success") {
        messageDiv.textContent = text;
        messageDiv.className = "message " + type;
        messageDiv.style.display = "block";
        setTimeout(() => { messageDiv.style.display = "none"; }, 3000);
    }

    duplicateBarcodeCheckbox.addEventListener("change", updateFiguresFields);

    itemsContainer.addEventListener("input", (e) => {
        if (e.target.id.startsWith('barcode') && duplicateBarcodeCheckbox.checked) {
            const index = e.target.id.replace('barcode', '');
            const figuresInput = document.getElementById(`figures${index}`);
            if (figuresInput) {
                figuresInput.value = e.target.value;
            }
        }
    });

    addItemButton.addEventListener("click", addItem);

    itemCountSelect.addEventListener("change", () => {
        const target = Number(itemCountSelect.value);
        let current = itemsContainer.querySelectorAll(".item").length;

        while (current < target) { 
            addItem(); 
            current++; 
        }
        while (current > target) {
            itemsContainer.lastElementChild.remove();
            current--;
        }
        updateItemNumbers();
        if (duplicateBarcodeCheckbox.checked) {
            updateFiguresFields();
        }
    });

    sendButton.addEventListener("click", async () => {
        const printType = printTypeInput.value.trim() || "default";
        const items = getItemsFromDOM();

        try {
            showMessage(`Sending print request...`, "success");
            
            await client.sendPrintRequest(items, printType);
            showMessage(`All items printed successfully`, "success");
        } catch (error) {
            showMessage(`Error: ${error.message}`, "error");
        }
    });

    clearButton.addEventListener("click", () => {
        printTypeInput.value = "default";
        duplicateBarcodeCheckbox.checked = false;
        itemsContainer.innerHTML = "";
        addItem();
        itemCountSelect.value = 1;
    });

    printTypeInput.value = "default";
    addItem();
});