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

async function calculateNextProfileId(profiles) {
    if (!profiles || Object.keys(profiles).length === 0) {
        return 2;
    }
    
    let maxId = 1;
    
    Object.values(profiles).forEach(profile => {
        if (profile.id > maxId) {
            maxId = profile.id;
        }
    });
    
    return maxId + 1;
}

async function loadProfiles() {
    const result = await chrome.storage.local.get(['profiles']);
    const profiles = result.profiles || {
        'default': Profile.getDefaultProfile().toObject()
    };
    
    nextProfileId = await calculateNextProfileId(profiles);

    const select = document.getElementById('profileSelect');
    select.innerHTML = '';

    Object.keys(profiles).forEach(profileName => {
        const option = document.createElement('option');
        option.value = profileName;
        option.textContent = profileName;
        option.dataset.id = profiles[profileName].id;
        select.appendChild(option);
    });

    await chrome.storage.local.set({ profiles });
}

async function loadProfile() {
    const result = await chrome.storage.local.get(['currentProfileName', 'profiles']);
    const currentProfileName = result.currentProfileName || 'default';
    const profiles = result.profiles || { 'default': Profile.getDefaultProfile().toObject() };

    document.getElementById('profileSelect').value = currentProfileName;

    if (profiles[currentProfileName]) {
        const settings = Profile.fromObject(profiles[currentProfileName]);
        applyProfile(settings);
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
            currentProfileName: profileName
        });

        await sendProfileToBackground(settings);
    }
}

async function sendProfileToBackground(profile) {
    try {
        await chrome.runtime.sendMessage({
            type: "profileUpdated",
            profile: profile.toObject()
        });
    } catch (error) {
        console.error("Failed to send profile to background:", error);
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

    const currentProfile = getCurrentProfile();
    
    const nextId = await calculateNextProfileId(profiles);
    currentProfile.id = nextId;
    
    profiles[profileName] = currentProfile.toObject();
    
    await chrome.storage.local.set({ profiles });
    await loadProfiles();

    document.getElementById('profileSelect').value = profileName;
    document.getElementById('profileName').value = '';

    await chrome.storage.local.set({ currentProfileName: profileName });

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

    const currentProfile = getCurrentProfile();
    profiles[profileName] = currentProfile.toObject();
    
    await chrome.storage.local.set({ profiles });
    
    await sendProfileToBackground(currentProfile);
    
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
    
    await chrome.storage.local.set({ currentProfileName: 'default' });
    
    await loadSelectedProfile();

    alert(`Profile "${profileName}" deleted successfully!`);
}

async function resetToDefault() {
    if (confirm('Are you sure you want to reset to default settings?')) {
        const defaultProfile = Profile.getDefaultProfile();
        applyProfile(defaultProfile);
        
        const currentProfileName = document.getElementById('profileSelect').value;
        if (currentProfileName !== 'default') {
            await saveCurrentProfile();
        }
        
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