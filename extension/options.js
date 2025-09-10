document.addEventListener('DOMContentLoaded', function() {
    localizeHtmlPage();
    initializeOptionsPage();
});

async function initializeOptionsPage() {
    // Загрузка пресетов и настроек
    await loadPresets();
    await loadSettings();
    
    // Загрузка списка принтеров
    loadPrinters();
    
    // Инициализация обработчиков
    initEventHandlers();
    
    // Инициализация сворачиваемых групп
    initCollapsibleGroups();
}

function initEventHandlers() {
    // Основные кнопки
    document.getElementById('saveBtn').addEventListener('click', saveCurrentSettings);
    document.getElementById('resetBtn').addEventListener('click', resetToDefault);
    
    // Кнопки пресетов
    document.getElementById('newPresetBtn').addEventListener('click', createNewPreset);
    document.getElementById('deletePresetBtn').addEventListener('click', deleteCurrentPreset);
    
    // Выбор пресета
    document.getElementById('presetSelect').addEventListener('change', loadSelectedPreset);
}

function initCollapsibleGroups() {
    document.querySelectorAll('.group-toggle').forEach(toggle => {
        toggle.addEventListener('click', function() {
            const group = this.closest('.settings-group');
            group.classList.toggle('collapsed');
            this.textContent = group.classList.contains('collapsed') ? '►' : '▼';
        });
    });
}

async function loadPresets() {
    const result = await chrome.storage.local.get(['presets']);
    const presets = result.presets || {
        'default': getDefaultSettings()
    };
    
    const select = document.getElementById('presetSelect');
    select.innerHTML = '';
    
    Object.keys(presets).forEach(presetName => {
        const option = document.createElement('option');
        option.value = presetName;
        option.textContent = presetName;
        select.appendChild(option);
    });
    
    // Сохраняем пресеты обратно (на случай первого запуска)
    await chrome.storage.local.set({ presets });
}

async function loadSettings() {
    const result = await chrome.storage.local.get(['currentPreset', 'currentSettings']);
    const currentPreset = result.currentPreset || 'default';
    
    document.getElementById('presetSelect').value = currentPreset;
    
    if (result.currentSettings) {
        applySettings(result.currentSettings);
    } else {
        // Загружаем настройки из выбранного пресета
        await loadSelectedPreset();
    }
}

async function loadSelectedPreset() {
    const presetName = document.getElementById('presetSelect').value;
    const result = await chrome.storage.local.get(['presets']);
    const presets = result.presets || {};
    
    if (presets[presetName]) {
        applySettings(presets[presetName]);
        // Сохраняем текущий пресет
        await chrome.storage.local.set({ currentPreset: presetName });
    }
}

function applySettings(settings) {
    // Основные настройки
    document.getElementById('paperHeight').value = settings.paperHeight || 297;
    document.getElementById('paperWidth').value = settings.paperWidth || 210;
    document.getElementById('labelHeight').value = settings.labelHeight || 50;
    document.getElementById('labelWidth').value = settings.labelWidth || 80;
    document.getElementById('marginX').value = settings.marginX || 2;
    document.getElementById('marginY').value = settings.marginY || 2;
    document.getElementById('offsetX').value = settings.offsetX || 0;
    document.getElementById('offsetY').value = settings.offsetY || 0;
    document.getElementById('labelsPerRow').value = settings.labelsPerRow || 2;
    document.getElementById('labelsPerColumn').value = settings.labelsPerColumn || 5;
    
    // Настройки текста
    document.getElementById('textAlignment').value = settings.textAlignment || 'center';
    document.getElementById('textMaxLength').value = settings.textMaxLength || 20;
    document.getElementById('textTrimLength').value = settings.textTrimLength || 18;
    document.getElementById('textFontSize').value = settings.textFontSize || 12;
    document.getElementById('textAngle').value = settings.textAngle || 0;
    
    // Тип штрих-кода
    document.getElementById('useDataMatrix').checked = settings.useDataMatrix || false;
    
    // Настройки цифр
    document.getElementById('numbersAlignment').value = settings.numbersAlignment || 'center';
    document.getElementById('numbersFontSize').value = settings.numbersFontSize || 10;
    document.getElementById('numbersAngle').value = settings.numbersAngle || 0;
    
    // Настройки штрих-кода
    document.getElementById('barcodeAlignment').value = settings.barcodeAlignment || 'center';
    document.getElementById('barcodeFontSize').value = settings.barcodeFontSize || 8;
    document.getElementById('barcodeAngle').value = settings.barcodeAngle || 0;
}

function getCurrentSettings() {
    return {
        // Основные настройки
        paperHeight: parseInt(document.getElementById('paperHeight').value),
        paperWidth: parseInt(document.getElementById('paperWidth').value),
        labelHeight: parseInt(document.getElementById('labelHeight').value),
        labelWidth: parseInt(document.getElementById('labelWidth').value),
        marginX: parseInt(document.getElementById('marginX').value),
        marginY: parseInt(document.getElementById('marginY').value),
        offsetX: parseInt(document.getElementById('offsetX').value),
        offsetY: parseInt(document.getElementById('offsetY').value),
        labelsPerRow: parseInt(document.getElementById('labelsPerRow').value),
        labelsPerColumn: parseInt(document.getElementById('labelsPerColumn').value),
        
        // Настройки текста
        textAlignment: document.getElementById('textAlignment').value,
        textMaxLength: parseInt(document.getElementById('textMaxLength').value),
        textTrimLength: parseInt(document.getElementById('textTrimLength').value),
        textFontSize: parseInt(document.getElementById('textFontSize').value),
        textAngle: parseInt(document.getElementById('textAngle').value),
        
        // Тип штрих-кода
        useDataMatrix: document.getElementById('useDataMatrix').checked,
        
        // Настройки цифр
        numbersAlignment: document.getElementById('numbersAlignment').value,
        numbersFontSize: parseInt(document.getElementById('numbersFontSize').value),
        numbersAngle: parseInt(document.getElementById('numbersAngle').value),
        
        // Настройки штрих-кода
        barcodeAlignment: document.getElementById('barcodeAlignment').value,
        barcodeFontSize: parseInt(document.getElementById('barcodeFontSize').value),
        barcodeAngle: parseInt(document.getElementById('barcodeAngle').value)
    };
}

function getDefaultSettings() {
    return {
        paperHeight: 297,
        paperWidth: 210,
        labelHeight: 50,
        labelWidth: 80,
        marginX: 2,
        marginY: 2,
        offsetX: 0,
        offsetY: 0,
        labelsPerRow: 2,
        labelsPerColumn: 5,
        textAlignment: 'center',
        textMaxLength: 20,
        textTrimLength: 18,
        textFontSize: 12,
        textAngle: 0,
        useDataMatrix: false,
        numbersAlignment: 'center',
        numbersFontSize: 10,
        numbersAngle: 0,
        barcodeAlignment: 'center',
        barcodeFontSize: 8,
        barcodeAngle: 0
    };
}

async function saveCurrentSettings() {
    await saveCurrentPreset();
}

async function createNewPreset() {
    const presetName = document.getElementById('presetName').value.trim();
    if (!presetName) {
        alert('Please enter a preset name');
        return;
    }
    
    const result = await chrome.storage.local.get(['presets']);
    const presets = result.presets || {};
    
    if (presets[presetName]) {
        if (!confirm(`Preset "${presetName}" already exists. Overwrite?`)) {
            return;
        }
    }
    
    presets[presetName] = getCurrentSettings();
    await chrome.storage.local.set({ presets });
    
    // Обновляем список пресетов
    await loadPresets();
    document.getElementById('presetSelect').value = presetName;
    document.getElementById('presetName').value = '';
    
    alert(`Preset "${presetName}" created successfully!`);
}

async function saveCurrentPreset() {
    const presetName = document.getElementById('presetSelect').value;
    if (presetName === 'default') {
        alert('Cannot overwrite default preset. Create a new preset instead.');
        return;
    }
    
    const result = await chrome.storage.local.get(['presets']);
    const presets = result.presets || {};
    
    presets[presetName] = getCurrentSettings();
    await chrome.storage.local.set({ presets });
    
    alert(`Preset "${presetName}" saved successfully!`);
}

async function deleteCurrentPreset() {
    const presetName = document.getElementById('presetSelect').value;
    if (presetName === 'default') {
        alert('Cannot delete default preset');
        return;
    }
    
    if (!confirm(`Are you sure you want to delete preset "${presetName}"?`)) {
        return;
    }
    
    const result = await chrome.storage.local.get(['presets']);
    const presets = result.presets || {};
    
    delete presets[presetName];
    await chrome.storage.local.set({ presets });
    
    // Переключаемся на default preset
    await loadPresets();
    document.getElementById('presetSelect').value = 'default';
    await loadSelectedPreset();
    
    alert(`Preset "${presetName}" deleted successfully!`);
}

async function resetToDefault() {
    if (confirm('Are you sure you want to reset to default settings?')) {
        applySettings(getDefaultSettings());
        await saveCurrentSettings();
        alert('Settings reset to default!');
    }
}

function loadPrinters() {
    const printerSelect = document.getElementById('printerName');
    printerSelect.innerHTML = '<option value="default">Default Printer</option>';
    
    const option = document.createElement('option');
    option.value = "custom";
    option.textContent = "Custom printer...";
    printerSelect.appendChild(option);
}