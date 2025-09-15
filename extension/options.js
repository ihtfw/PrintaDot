document.addEventListener('DOMContentLoaded', function () {
    localizeHtmlPage();
    initializeOptionsPage();
});

async function initializeOptionsPage() {
    await loadProfiles();
    await loadSettings();

    loadPrinters();

    initEventHandlers();

    initCollapsibleGroups();
}

function initEventHandlers() {

    document.getElementById('saveBtn').addEventListener('click', saveCurrentSettings);
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
    const result = await chrome.storage.local.get(['profiles']);
    const profiles = result.profiles || {
        'default': getDefaultSettings()
    };

    const select = document.getElementById('profileSelect');
    select.innerHTML = '';

    Object.keys(profiles).forEach(profileName => {
        const option = document.createElement('option');
        option.value = profileName;
        option.textContent = profileName;
        select.appendChild(option);
    });

    await chrome.storage.local.set({ profiles });
}

async function loadSettings() {
    const result = await chrome.storage.local.get(['currentProfile', 'currentSettings']);
    const currentProfile = result.currentProfile || 'default';

    document.getElementById('profileSelect').value = currentProfile;

    if (result.currentSettings) {
        applySettings(result.currentSettings);
    } else {

        await loadSelectedProfile();
    }
}

async function loadSelectedProfile() {
    const profileName = document.getElementById('profileSelect').value;
    const result = await chrome.storage.local.get(['profiles']);
    const profiles = result.profiles || {};

    if (profiles[profileName]) {
        applySettings(profiles[profileName]);

        await chrome.storage.local.set({ currentProfile: profileName });
    }
}

function applySettings(settings) {
    // Main settings
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

    // Text settings
    document.getElementById('textAlignment').value = settings.textAlignment || 'center';
    document.getElementById('textMaxLength').value = settings.textMaxLength || 20;
    document.getElementById('textTrimLength').value = settings.textTrimLength || 18;
    document.getElementById('textFontSize').value = settings.textFontSize || 12;
    document.getElementById('textAngle').value = settings.textAngle || 0;

    // Barcode type
    document.getElementById('useDataMatrix').checked = settings.useDataMatrix || false;

    // Settings of number
    document.getElementById('numbersAlignment').value = settings.numbersAlignment || 'center';
    document.getElementById('numbersFontSize').value = settings.numbersFontSize || 10;
    document.getElementById('numbersAngle').value = settings.numbersAngle || 0;

    // Settings of barcode
    document.getElementById('barcodeAlignment').value = settings.barcodeAlignment || 'center';
    document.getElementById('barcodeFontSize').value = settings.barcodeFontSize || 8;
    document.getElementById('barcodeAngle').value = settings.barcodeAngle || 0;
}

function getCurrentSettings() {
    return {
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

        // Settings of number
        numbersAlignment: document.getElementById('numbersAlignment').value,
        numbersFontSize: parseInt(document.getElementById('numbersFontSize').value),
        numbersAngle: parseInt(document.getElementById('numbersAngle').value),

        // Settings of barcode
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
    await saveCurrentProfile();
}

async function createNewProfile() {
    const profileName = document.getElementById('profileName').value.trim();
    if (!profileName) {
        alert('Please enter a profile name');
        return;
    }

    const result = await chrome.storage.local.get(['profiles']);
    const profiles = result.profiles || {};

    if (profiles[profileName]) {
        if (!confirm(`Profile "${profileName}" already exists. Overwrite?`)) {
            return;
        }
    }

    profiles[profileName] = getCurrentSettings();
    await chrome.storage.local.set({ profiles });

    await loadProfiles();

    document.getElementById('profileSelect').value = profileName;
    document.getElementById('profileName').value = '';

    alert(`Profile "${profileName}" created successfully!`);
}

async function saveCurrentProfile() {
    const profileName = document.getElementById('profileSelect').value;
    if (profileName === 'default') {
        alert('Cannot overwrite default profile. Create a new profile instead.');
        return;
    }

    const result = await chrome.storage.local.get(['profiles']);
    const profiles = result.profiles || {};

    profiles[profileName] = getCurrentSettings();
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