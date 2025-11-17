document.addEventListener('DOMContentLoaded', function() {
    const printTypeInput = document.getElementById('printType');
    const duplicateBarcodeCheckbox = document.getElementById('duplicateBarcode');
    const itemsContainer = document.getElementById('itemsContainer');
    const addItemButton = document.getElementById('addItem');
    const itemCountSelect = document.getElementById('itemCount');
    const sendButton = document.getElementById('sendBtn');
    const clearButton = document.getElementById('clearBtn');
    const jsonOutput = document.getElementById('jsonOutput');
    const messageDiv = document.getElementById('message');

    function createItemElement(index) {
        const itemDiv = document.createElement('div');
        itemDiv.className = 'item';
        itemDiv.innerHTML = `
            <div class="item-header">
                <span class="item-number">Item ${index}</span>
                <button class="remove-item" type="button">×</button>
            </div>
            <div class="input-group">
                <label for="header${index}">Header</label>
                <input type="text" id="header${index}" placeholder="Enter header">
            </div>
            <div class="input-group">
                <label for="barcode${index}">Barcode</label>
                <input type="text" id="barcode${index}" placeholder="Enter barcode">
            </div>
            <div class="input-group">
                <label for="figures${index}">Figures (text below barcode)</label>
                <input type="text" id="figures${index}" placeholder="Enter text (optional)">
            </div>
        `;
        
        const removeButton = itemDiv.querySelector('.remove-item');
        removeButton.addEventListener('click', function() {
            itemDiv.remove();
            updateItemNumbers();
            updateJSON();
        });
        
        const inputs = itemDiv.querySelectorAll('input');
        inputs.forEach(input => {
            input.addEventListener('input', updateJSON);
        });
        
        return itemDiv;
    }

    function updateItemNumbers() {
        const items = itemsContainer.querySelectorAll('.item');
        items.forEach((item, index) => {
            const numberSpan = item.querySelector('.item-number');
            numberSpan.textContent = `Item ${index + 1}`;
        });
    }

    function addItem() {
        const itemCount = itemsContainer.querySelectorAll('.item').length;
        const newItem = createItemElement(itemCount + 1);
        itemsContainer.appendChild(newItem);
        updateJSON();
    }

    addItemButton.addEventListener('click', addItem);

    itemCountSelect.addEventListener('change', function() {
        const targetCount = parseInt(this.value);
        const currentCount = itemsContainer.querySelectorAll('.item').length;
        
        if (targetCount > currentCount) {
            for (let i = currentCount; i < targetCount; i++) {
                addItem();
            }
        } else if (targetCount < currentCount) {
            const items = itemsContainer.querySelectorAll('.item');
            for (let i = currentCount - 1; i >= targetCount; i--) {
                items[i].remove();
            }
            updateItemNumbers();
        }
        
        updateJSON();
    });

    function generateData() {
        const printType = printTypeInput.value.trim();
        const duplicateBarcode = duplicateBarcodeCheckbox.checked;
        
        const items = [];
        const itemElements = itemsContainer.querySelectorAll('.item');
        
        itemElements.forEach((itemElement, index) => {
            const header = itemElement.querySelector(`#header${index + 1}`).value.trim();
            const barcode = itemElement.querySelector(`#barcode${index + 1}`).value.trim();
            let figures = itemElement.querySelector(`#figures${index + 1}`).value.trim();
            
            if (duplicateBarcode && !figures) {
                figures = barcode;
            }
            
            items.push({
                header: header || `Item Header ${index + 1}`,
                barcode: barcode || `1234567890${index + 1}`,
                figures: figures || null
            });
        });
        
        return {
            version: 1,
            type: "PrintRequest",
            printType: printType || "default",
            items: items
        };
    }

    function updateJSON() {
        const data = generateData();
        jsonOutput.textContent = JSON.stringify(data, null, 2);
    }

    function sendData() {
        const data = generateData();
        
        try {
            window.postMessage(data, '*');
            
            messageDiv.textContent = 'Data successfully sent via window.postMessage';
            messageDiv.className = 'message success';
            messageDiv.style.display = 'block';
            
            setTimeout(() => {
                messageDiv.style.display = 'none';
            }, 3000);
        } catch (error) {
            messageDiv.textContent = 'Error sending data: ' + error.message;
            messageDiv.className = 'message error';
            messageDiv.style.display = 'block';
        }
    }

    function clearForm() {
        printTypeInput.value = '';
        duplicateBarcodeCheckbox.checked = false;
        
        const items = itemsContainer.querySelectorAll('.item');
        items.forEach(item => item.remove());
        
        addItem();
        
        itemCountSelect.value = '1';
        
        updateJSON();
        
        messageDiv.style.display = 'none';
    }

    sendButton.addEventListener('click', sendData);
    clearButton.addEventListener('click', clearForm);

    printTypeInput.addEventListener('input', updateJSON);
    duplicateBarcodeCheckbox.addEventListener('change', updateJSON);

    addItem();
});