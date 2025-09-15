class Profile {
    constructor(id = 0, settings = {}) {
        this.id = id; // Уникальный ID профиля настроек
        
        // Main settings
        this.paperHeight = settings.paperHeight || 297;
        this.paperWidth = settings.paperWidth || 210;
        this.labelHeight = settings.labelHeight || 50;
        this.labelWidth = settings.labelWidth || 80;
        this.marginX = settings.marginX || 2;
        this.marginY = settings.marginY || 2;
        this.offsetX = settings.offsetX || 0;
        this.offsetY = settings.offsetY || 0;
        this.labelsPerRow = settings.labelsPerRow || 2;
        this.labelsPerColumn = settings.labelsPerColumn || 5;

        // Text settings
        this.textAlignment = settings.textAlignment || 'center';
        this.textMaxLength = settings.textMaxLength || 20;
        this.textTrimLength = settings.textTrimLength || 18;
        this.textFontSize = settings.textFontSize || 12;
        this.textAngle = settings.textAngle || 0;

        // Barcode type
        this.useDataMatrix = settings.useDataMatrix || false;

        // Number settings
        this.numbersAlignment = settings.numbersAlignment || 'center';
        this.numbersFontSize = settings.numbersFontSize || 10;
        this.numbersAngle = settings.numbersAngle || 0;

        // Barcode settings
        this.barcodeAlignment = settings.barcodeAlignment || 'center';
        this.barcodeFontSize = settings.barcodeFontSize || 8;
        this.barcodeAngle = settings.barcodeAngle || 0;
    }

    // Convert to plain object for storage
    toObject() {
        return {
            id: this.id,
            paperHeight: this.paperHeight,
            paperWidth: this.paperWidth,
            labelHeight: this.labelHeight,
            labelWidth: this.labelWidth,
            marginX: this.marginX,
            marginY: this.marginY,
            offsetX: this.offsetX,
            offsetY: this.offsetY,
            labelsPerRow: this.labelsPerRow,
            labelsPerColumn: this.labelsPerColumn,
            textAlignment: this.textAlignment,
            textMaxLength: this.textMaxLength,
            textTrimLength: this.textTrimLength,
            textFontSize: this.textFontSize,
            textAngle: this.textAngle,
            useDataMatrix: this.useDataMatrix,
            numbersAlignment: this.numbersAlignment,
            numbersFontSize: this.numbersFontSize,
            numbersAngle: this.numbersAngle,
            barcodeAlignment: this.barcodeAlignment,
            barcodeFontSize: this.barcodeFontSize,
            barcodeAngle: this.barcodeAngle
        };
    }

    // Create from plain object
    static fromObject(obj) {
        return new Profile(obj.id, obj);
    }

    // Get default settings (id = 0)
    static getDefaultProfile() {
        return new Profile(0);
    }
}

// Глобальная переменная для хранения следующего ID профиля
let nextProfileId = 1;

document.addEventListener('DOMContentLoaded', function () {
    localizeHtmlPage();
    initializeOptionsPage();
});

async function initializeOptionsPage() {
    await loadProfiles();
    await loadProfile();

    loadPrinters();

    initEventHandlers();

    initCollapsibleGroups();
}

function initEventHandlers() {
    document.getElementById('saveBtn').addEventListener('click', saveCurrentProfile);
    document.getElementById('resetBtn').addEventListener('click', resetToDefault);

    document.getElementById('newProfileBtn').addEventListener('click', createNewProfile);
    document.getElementById('deleteProfileBtn').addEventListener('click', deleteCurrentProfile);

    document.getElementById('profileSelect').addEventListener('change', loadSelectedProfile);
}

function initCollapsibleGroups() {
    document.querySelectorAll('.group-toggle').forEach(toggle => {
        toggle.addEventListener('click', function () {
            const group = this.closest('.settings-group');
            group.classList.toggle('collapsed');
            this.textContent = group.classList.contains('collapsed') ? '►' : '▼';
        });
    });
}

async function loadProfiles() {
    const result = await chrome.storage.local.get(['profiles', 'nextProfileId']);
    const profiles = result.profiles || {
        'default': Profile.getDefaultProfile().toObject()
    };
    
    // Восстанавливаем следующий ID или устанавливаем по умолчанию
    nextProfileId = result.nextProfileId || 1;

    const select = document.getElementById('profileSelect');
    select.innerHTML = '';

    Object.keys(profiles).forEach(profileName => {
        const option = document.createElement('option');
        option.value = profileName;
        option.textContent = profileName;
        option.dataset.id = profiles[profileName].id; // Сохраняем ID в data-атрибуте
        select.appendChild(option);
    });

    await chrome.storage.local.set({ profiles, nextProfileId });
}

async function loadProfile() {
    const result = await chrome.storage.local.get(['currentProfile', 'currentProfile']);
    const currentProfile = result.currentProfile || 'default';

    document.getElementById('profileSelect').value = currentProfile;

    if (result.currentProfile) {
        const settings = Profile.fromObject(result.currentProfile);
        applyProfile(settings);
    } else {
        await loadSelectedProfile();
    }
}

async function loadSelectedProfile() {
    const profileName = document.getElementById('profileSelect').value;
    const result = await chrome.storage.local.get(['profiles']);
    const profiles = result.profiles || {};

    if (profiles[profileName]) {
        const settings = Profile.fromObject(profiles[profileName]);
        applyProfile(settings);

        await chrome.storage.local.set({ 
            currentProfile: profileName,
            currentProfile: profiles[profileName]
        });
    }
}

function applyProfile(settings) {
    // Main settings
    document.getElementById('paperHeight').value = settings.paperHeight;
    document.getElementById('paperWidth').value = settings.paperWidth;
    document.getElementById('labelHeight').value = settings.labelHeight;
    document.getElementById('labelWidth').value = settings.labelWidth;
    document.getElementById('marginX').value = settings.marginX;
    document.getElementById('marginY').value = settings.marginY;
    document.getElementById('offsetX').value = settings.offsetX;
    document.getElementById('offsetY').value = settings.offsetY;
    document.getElementById('labelsPerRow').value = settings.labelsPerRow;
    document.getElementById('labelsPerColumn').value = settings.labelsPerColumn;

    // Text settings
    document.getElementById('textAlignment').value = settings.textAlignment;
    document.getElementById('textMaxLength').value = settings.textMaxLength;
    document.getElementById('textTrimLength').value = settings.textTrimLength;
    document.getElementById('textFontSize').value = settings.textFontSize;
    document.getElementById('textAngle').value = settings.textAngle;

    // Barcode type
    document.getElementById('useDataMatrix').checked = settings.useDataMatrix;

    // Number settings
    document.getElementById('numbersAlignment').value = settings.numbersAlignment;
    document.getElementById('numbersFontSize').value = settings.numbersFontSize;
    document.getElementById('numbersAngle').value = settings.numbersAngle;

    // Barcode settings
    document.getElementById('barcodeAlignment').value = settings.barcodeAlignment;
    document.getElementById('barcodeFontSize').value = settings.barcodeFontSize;
    document.getElementById('barcodeAngle').value = settings.barcodeAngle;
}

function getCurrentProfile() {
    const profileSelect = document.getElementById('profileSelect');
    const currentId = parseInt(profileSelect.options[profileSelect.selectedIndex].dataset.id) || 0;
    
    return new Profile(currentId, {
        // Main settings
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

        // Text settings
        textAlignment: document.getElementById('textAlignment').value,
        textMaxLength: parseInt(document.getElementById('textMaxLength').value),
        textTrimLength: parseInt(document.getElementById('textTrimLength').value),
        textFontSize: parseInt(document.getElementById('textFontSize').value),
        textAngle: parseInt(document.getElementById('textAngle').value),

        // Barcode type
        useDataMatrix: document.getElementById('useDataMatrix').checked,

        // Number settings
        numbersAlignment: document.getElementById('numbersAlignment').value,
        numbersFontSize: parseInt(document.getElementById('numbersFontSize').value),
        numbersAngle: parseInt(document.getElementById('numbersAngle').value),

        // Barcode settings
        barcodeAlignment: document.getElementById('barcodeAlignment').value,
        barcodeFontSize: parseInt(document.getElementById('barcodeFontSize').value),
        barcodeAngle: parseInt(document.getElementById('barcodeAngle').value)
    });
}

function getDefaultProfile() {
    return Profile.getDefaultProfile().toObject();
}

async function saveCurrentProfile() {
    await saveCurrentProfile();
}

async function createNewProfile() {
    const profileName = document.getElementById('profileName').value.trim();
    if (!profileName) {
        alert('Please enter a profile name');
        return;
    }

    const result = await chrome.storage.local.get(['profiles', 'nextProfileId']);
    const profiles = result.profiles || {};
    nextProfileId = result.nextProfileId || 1;

    if (profiles[profileName]) {
        if (!confirm(`Profile "${profileName}" already exists. Overwrite?`)) {
            return;
        }
    }

    // Создаем новый профиль с уникальным ID
    const currentProfile = getCurrentProfile();
    currentProfile.id = nextProfileId++; // Присваиваем новый ID и увеличиваем счетчик
    
    profiles[profileName] = currentProfile.toObject();
    
    await chrome.storage.local.set({ profiles, nextProfileId });
    await loadProfiles();

    document.getElementById('profileSelect').value = profileName;
    document.getElementById('profileName').value = '';

    alert(`Profile "${profileName}" created successfully with ID: ${currentProfile.id}!`);
}

async function saveCurrentProfile() {
    const profileName = document.getElementById('profileSelect').value;
    if (profileName === 'default') {
        alert('Cannot overwrite default profile. Create a new profile instead.');
        return;
    }

    const result = await chrome.storage.local.get(['profiles']);
    const profiles = result.profiles || {};

    // Сохраняем настройки с сохранением текущего ID
    const currentProfile = getCurrentProfile();
    profiles[profileName] = currentProfile.toObject();
    
    await chrome.storage.local.set({ profiles });
    alert(`Profile "${profileName}" saved successfully!`);
}

async function deleteCurrentProfile() {
    const profileName = document.getElementById('profileSelect').value;
    if (profileName === 'default') {
        alert('Cannot delete default profile');
        return;
    }

    if (!confirm(`Are you sure you want to delete profile "${profileName}"?`)) {
        return;
    }

    const result = await chrome.storage.local.get(['profiles']);
    const profiles = result.profiles || {};

    delete profiles[profileName];
    await chrome.storage.local.set({ profiles });

    await loadProfiles();
    document.getElementById('profileSelect').value = 'default';
    await loadSelectedProfile();

    alert(`Profile "${profileName}" deleted successfully!`);
}

async function resetToDefault() {
    if (confirm('Are you sure you want to reset to default settings?')) {
        const defaultProfile = Profile.getDefaultProfile();
        applyProfile(defaultProfile);
        await saveCurrentProfile();
        alert('Profile reset to default!');
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