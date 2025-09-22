document.addEventListener('DOMContentLoaded', async function () {   
    await initLocalization();
    initializeOptionsPage();
});

chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
    if (request.type === "Exception" && request.messageText) {
        showError(request.messageText);
    }
});

async function initializeOptionsPage() {
    await loadProfiles();
    await loadProfile();

    document.getElementById('headerInput').value = 'test';
    document.getElementById('barcodeInput').value = '0123456789';

    loadPrinters();
    initEventHandlers();
    initCollapsibleGroups();
    initLanguageToggle();
}

function initEventHandlers() {
    document.getElementById('errorContainer').addEventListener('click', hideError);
    document.getElementById('printBtn').addEventListener('click', handlePrint);
    document.getElementById('saveBtn').addEventListener('click', saveCurrentProfile);
    document.getElementById('resetBtn').addEventListener('click', resetToDefault);
    document.getElementById('newProfileBtn').addEventListener('click', createNewProfile);
    document.getElementById('deleteProfileBtn').addEventListener('click', deleteCurrentProfile);
    document.getElementById('profileSelect').addEventListener('change', loadSelectedProfile);
    document.getElementById('clearAllProfilesBtn').addEventListener('click', clearAllProfiles);
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
        'default': Profile.getDefaultProfile().toStorageObject()
    };

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
    const profiles = result.profiles || {
        'default': Profile.getDefaultProfile().toStorageObject()
    };

    document.getElementById('profileSelect').value = currentProfileName;

    if (profiles[currentProfileName]) {
        const profile = Profile.fromStorageObject(profiles[currentProfileName]);
        applyProfile(profile);
    }
}

async function loadSelectedProfile() {
    const profileName = document.getElementById('profileSelect').value;
    const result = await chrome.storage.local.get(['profiles']);
    const profiles = result.profiles || {};

    if (profiles[profileName]) {
        const profile = Profile.fromStorageObject(profiles[profileName]);
        applyProfile(profile);

        await chrome.storage.local.set({
            currentProfileName: profileName
        });

        await sendProfileToBackground(profile);
    }
}

async function sendProfileToBackground(profile) {
    try {
        await chrome.runtime.sendMessage(profile.toStorageObject());
    } catch (error) {
        console.error("Failed to send profile to background:", error);
    }
}

function applyProfile(profile) {
    // Main settings
    document.getElementById('paperHeight').value = profile.paperHeight;
    document.getElementById('paperWidth').value = profile.paperWidth;
    document.getElementById('labelHeight').value = profile.labelHeight;
    document.getElementById('labelWidth').value = profile.labelWidth;
    document.getElementById('marginX').value = profile.marginX;
    document.getElementById('marginY').value = profile.marginY;
    document.getElementById('offsetX').value = profile.offsetX;
    document.getElementById('offsetY').value = profile.offsetY;
    document.getElementById('labelsPerRow').value = profile.labelsPerRow;
    document.getElementById('labelsPerColumn').value = profile.labelsPerColumn;

    // Text settings
    document.getElementById('textAlignment').value = profile.textAlignment;
    document.getElementById('textMaxLength').value = profile.textMaxLength;
    document.getElementById('textTrimLength').value = profile.textTrimLength;
    document.getElementById('textFontSize').value = profile.textFontSize;
    document.getElementById('textAngle').value = profile.textAngle;

    // Barcode type
    document.getElementById('useDataMatrix').checked = profile.useDataMatrix;

    // Number settings
    document.getElementById('numbersAlignment').value = profile.numbersAlignment;
    document.getElementById('numbersFontSize').value = profile.numbersFontSize;
    document.getElementById('numbersAngle').value = profile.numbersAngle;

    // Barcode settings
    document.getElementById('barcodeAlignment').value = profile.barcodeAlignment;
    document.getElementById('barcodeFontSize').value = profile.barcodeFontSize;
    document.getElementById('barcodeAngle').value = profile.barcodeAngle;
}

function getCurrentProfileFromForm() {
    const profileSelect = document.getElementById('profileSelect');
    const currentProfileName = profileSelect.value;
    const currentId = parseInt(profileSelect.options[profileSelect.selectedIndex].dataset.id) || 0;

    return new Profile(
        currentId,
        currentProfileName,
        {
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
        }
    );
}

async function createNewProfile() {
    const profileName = document.getElementById('profileName').value.trim();
    if (!profileName) {
        showError('Please enter a profile name');
        return;
    }

    const result = await chrome.storage.local.get(['profiles']);
    const profiles = result.profiles || {};

    if (profiles[profileName]) {
        if (!confirm(`Profile "${profileName}" already exists. Overwrite?`)) {
            return;
        }
    }

    const currentProfile = getCurrentProfileFromForm();
    currentProfile.profileName = profileName;

    // Generate new ID for the profile
    const nextId = await calculateNextProfileId(profiles);
    currentProfile.id = nextId;

    // Save to storage using the class method
    profiles[profileName] = currentProfile.toStorageObject();

    await chrome.storage.local.set({ profiles });

    await sendProfileToBackground(currentProfile);

    await loadProfiles();

    document.getElementById('profileSelect').value = profileName;
    document.getElementById('profileName').value = '';

    await chrome.storage.local.set({ currentProfileName: profileName });
}

async function saveCurrentProfile() {
    const profileName = document.getElementById('profileSelect').value;
    if (profileName === 'default') {
        showError('Cannot overwrite default profile. Create a new profile instead.');
        return;
    }

    const result = await chrome.storage.local.get(['profiles']);
    const profiles = result.profiles || {};

    const currentProfile = getCurrentProfileFromForm();
    currentProfile.profileName = profileName;

    // Preserve the original ID
    if (profiles[profileName]) {
        currentProfile.id = profiles[profileName].id;
    }

    profiles[profileName] = currentProfile.toStorageObject();

    await chrome.storage.local.set({ profiles });
    await sendProfileToBackground(currentProfile);
}

async function deleteCurrentProfile() {
    const profileName = document.getElementById('profileSelect').value;
    if (profileName === 'default') {
        showError('Cannot delete default profile');
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
}

async function resetToDefault() {
    const defaultProfile = Profile.getDefaultProfile();
    applyProfile(defaultProfile);

    const currentProfileName = document.getElementById('profileSelect').value;
    if (currentProfileName !== 'default') {
        await saveCurrentProfile();
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

async function clearAllProfiles() {
    if (!confirm(`Are you sure you want to delete all profiles?`)) {
        return;
    }

    const defaultProfile = Profile.getDefaultProfile();
    const profiles = {
        'default': defaultProfile.toStorageObject()
    };

    await chrome.storage.local.set({ profiles });

    await loadProfiles();
    await loadSelectedProfile();
}

function handlePrint() {
     const header = document.getElementById('headerInput').value.trim();
    const barcode = document.getElementById('barcodeInput').value.trim();
    const profile = document.getElementById('profileSelect').value;

    if (!barcode) {
        showError("Please fill in barcode");
        headerInput.focus();
        return;
    }

    chrome.runtime.sendMessage({
        type: "printRequest",
        version: 1,
        profile: profile,
        items: [
            {
                header: header,
                barcode: barcode
            }
        ]
    });
}

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

async function initLanguageToggle() {
    const savedLang = localStorage.getItem("appLanguage") || "en";

    const langRu = document.getElementById("langRu");
    const langEn = document.getElementById("langEn");

    if (savedLang === "ru") {
        langRu.checked = true;
    } else {
        langEn.checked = true;
    }

    await loadMessages(savedLang);
    await localizeHtmlPage(savedLang);

    [langRu, langEn].forEach(input => {
        input.addEventListener("change", async () => {
            if (input.checked) {
                window.location.reload();

                const newLang = input.value;
                localStorage.setItem("appLanguage", newLang);
            }
        });
    });
}
