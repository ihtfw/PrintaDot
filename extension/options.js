import { Profile } from "./profile.js";

document.addEventListener('DOMContentLoaded', async function () {   
    await initLocalization();
    await initializeOptionsPage();

    await loadPrintersFromStorage();

    await getPrinters();
});

chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
    console.log(request);
    if (request.type === "Exception" && request.messageText) {
        showError(request.messageText);
    }

    if (request.type === "GetPrintersResponse" && request.printers) {
        chrome.storage.local.set({ printers: request.printers });
        
        updatePrintersList(request.printers);
    }
});

async function loadPrintersFromStorage() {
    const result = await chrome.storage.local.get(['printers']);
    if (result.printers && result.printers.length > 0) {
        updatePrintersList(result.printers);
    }
    
}

function updatePrintersList(printers) {
    const printerSelect = document.getElementById('printerName');
    
    const currentValue = printerSelect.value;
    
    printerSelect.innerHTML = '';
    
    printers.forEach(printer => {
        const option = document.createElement('option');
        option.value = printer;
        option.textContent = printer;
        printerSelect.appendChild(option);
    });
    
    if (printers.includes(currentValue)) {
        printerSelect.value = currentValue;
    } else if (printers.length > 0) {
        printerSelect.value = printers[0];
    }
}

async function initializeOptionsPage() {

    await loadProfiles();
    await loadProfile();

    document.getElementById('headerInput').value = 'test';
    document.getElementById('barcodeInput').value = '0123456789';

    initEventHandlers();
    initCollapsibleGroups();
    initLanguageToggle();

    //workaround: without this function values of all inputs on page got broken
    await new Promise(resolve => setTimeout(resolve, 0));

    await updateMappingTable()
}

function initEventHandlers() {
    document.getElementById('errorContainer').addEventListener('click', hideError);
    document.getElementById('printBtn').addEventListener('click', handlePrint);
    document.getElementById('resetToA4Btn').addEventListener('click', resetToA4);
    document.getElementById('resetToThermoBtn').addEventListener('click', resetToThermo);
    document.getElementById('newProfileBtn').addEventListener('click', createNewProfile);
    document.getElementById('deleteProfileBtn').addEventListener('click', deleteCurrentProfile);
    document.getElementById('profileSelect').addEventListener('change', loadSelectedProfile);
    document.getElementById('clearAllProfilesBtn').addEventListener('click', clearAllProfiles);
    
    const addMappingBtn = document.getElementById('addMappingBtn');
    if (addMappingBtn) {
        addMappingBtn.addEventListener('click', addNewMapping);
    }
    
    const clearMappingsBtn = document.getElementById('clearMappingsBtn');
    if (clearMappingsBtn) {
        clearMappingsBtn.addEventListener('click', clearAllMappings);
    }

    initAutoSaveHandlers();
}

function initAutoSaveHandlers() {
    const settingInputs = document.querySelectorAll(`
        #paperHeight, #paperWidth, #labelHeight, #labelWidth,
        #marginX, #marginY, #offsetX, #offsetY,
        #labelsPerRow, #labelsPerColumn,
        #textAlignment, #textMaxLength, #textTrimLength, #textFontSize, #textAngle,
        #printerName,
        #useDataMatrix,
        #numbersAlignment, #numbersFontSize, #numbersAngle,
        #barcodeAlignment, #barcodeFontSize, #barcodeAngle
    `);

    settingInputs.forEach(input => {
        if (input.type === 'checkbox') {
            input.addEventListener('change', handleSettingChange);
        } else {
            input.addEventListener('input', handleSettingChange);
        }
    });
}

async function handleSettingChange() {
    const profileName = document.getElementById('profileSelect').value;

    await saveCurrentProfile();
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

// async function initializeDefaultProfiles() {
//     const result = await chrome.storage.local.get(['profiles']);
//     let profiles = result.profiles || {};

//     if (!profiles['A4']) {
//         profiles['A4'] = Profile.getA4Profile().toStorageObject();
//     }
//     if (!profiles['Thermo']) {
//         profiles['Thermo'] = Profile.getThermoProfile().toStorageObject();
//     }

//     const currentResult = await chrome.storage.local.get(['currentProfileName']);
//     if (!currentResult.currentProfileName) {
//         await chrome.storage.local.set({ currentProfileName: 'A4' });
//     }

//     await chrome.storage.local.set({ profiles });
// }

async function loadProfiles() {
    const result = await chrome.storage.local.get(['profiles']);
    const profiles = result.profiles || {
        'A4': Profile.getA4Profile().toStorageObject(),
        'Thermo': Profile.getThermoProfile().toStorageObject()
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
    const currentProfileName = result.currentProfileName || 'A4';
    const profiles = result.profiles || {
        'A4': Profile.getA4Profile().toStorageObject(),
        'Thermo': Profile.getThermoProfile().toStorageObject()
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

    // Printer settings
    document.getElementById('printerName').value = profile.printerName || "default";

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

            printerName: document.getElementById('printerName').value,

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

    await loadProfiles();

    document.getElementById('profileSelect').value = profileName;
    document.getElementById('profileName').value = '';

    await chrome.storage.local.set({ currentProfileName: profileName });
}

async function saveCurrentProfile() {
    const profileName = document.getElementById('profileSelect').value;

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
}

async function deleteCurrentProfile() {
    const profileName = document.getElementById('profileSelect').value;

    if (profileName === 'A4' || profileName === 'Thermo') {
        showError('Cannot delete default profiles (A4 and Thermo)');
        return;
    }

    const result = await chrome.storage.local.get(['profiles']);
    const profiles = result.profiles || {};

    delete profiles[profileName];
    await chrome.storage.local.set({ profiles });

    await loadProfiles();
    document.getElementById('profileSelect').value = 'A4';

    await chrome.storage.local.set({ currentProfileName: 'A4' });
    await loadSelectedProfile();
}

async function resetToA4() {
    const defaultProfile = Profile.getA4Profile();
    applyProfile(defaultProfile);

    const currentProfileName = document.getElementById('profileSelect').value;
    if (currentProfileName !== 'A4') {
        await saveCurrentProfile();
    }
}

async function resetToThermo() {
    const defaultProfile = Profile.getThermoProfile();
    applyProfile(defaultProfile);

    const currentProfileName = document.getElementById('profileSelect').value;
    if (currentProfileName !== 'Thermo') {
        await saveCurrentProfile();
    }
}

function showPrinterError(message) {
    const printerSelect = document.getElementById('printerName');
    const option = document.createElement('option');
    option.value = "error";
    option.textContent = message;
    option.disabled = true;
    printerSelect.appendChild(option);
}

async function clearAllProfiles() {
    if (!confirm(`Are you sure you want to delete all profiles?`)) {
        return;
    }

    const profiles = {
        'A4': Profile.getA4Profile(),
        'Thermo': Profile.getThermoProfile()
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
        type: "PrintRequest",
        version: 1,
        id: generateGuid(),
        profile: profile,
        isFromExtension: true,
        items: [
            {
                header: header || null,
                barcode: barcode
            }
        ]
    });
}

async function getPrinters() {
    chrome.runtime.sendMessage({
        type: "GetPrintersRequest",
        id: generateGuid(),
        version: 1,
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

const MAPPING_KEY = "printTypeMapping";

function getMapping() {
    return new Promise((resolve) => {
        chrome.storage.local.get([MAPPING_KEY], (result) => {
            resolve(result[MAPPING_KEY] || {});
        });
    });
}

function saveMapping(mapping) {
    return new Promise((resolve) => {
        chrome.storage.local.set({ [MAPPING_KEY]: mapping }, () => {
            resolve();
        });
    });
}

async function updateMappingTable() {
    try {        
        const mapping = await getMapping();
        const profiles = await loadProfilesForMapping();
        const tbody = document.getElementById("mappingTableBody");
        
        if (!tbody) {
            console.error("Element with id 'mappingTableBody' not found");
            return;
        }
        
        const currentValues = {};
        tbody.querySelectorAll('.mapping-profile-select').forEach(select => {
            const printType = select.dataset.printtype;
            currentValues[printType] = select.value;
        });
        
        tbody.innerHTML = '';
        
        if (Object.keys(mapping).length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="3" class="empty-mapping">
                        No mappings configured
                    </td>
                </tr>
            `;
            console.log('No mappings found, showing empty message');
            return;
        }
        
        Object.entries(mapping).forEach(([printType, profileName]) => {
            const row = document.createElement('tr');
            
            const selectedProfile = currentValues[printType] || profileName;
            
            const profileOptions = Object.keys(profiles).map(profile => 
                `<option value="${profile}" ${profile === selectedProfile ? 'selected' : ''}>${profile}</option>`
            ).join('');
            
             const deleteButtonText = getMessage('deleteButton');

            row.innerHTML = `
                <td class="mapping-key">${printType}</td>
                <td>
                    <select class="mapping-profile-select" data-printtype="${printType}">
                        ${profileOptions}
                    </select>
                </td>
                <td class="mapping-actions">
                    <button class="mapping-delete-btn" data-printtype="${printType}">
                        ${deleteButtonText}
                    </button>
                </td>
            `;
            
            tbody.appendChild(row);
        });
        
        console.log('Table updated with', Object.keys(mapping).length, 'mappings');
        attachMappingEventHandlers();
        
    } catch (error) {
        console.error("Error updating mapping table:", error);
    }
}

function attachMappingEventHandlers() {
    document.querySelectorAll('.mapping-profile-select').forEach(select => {
        select.addEventListener('change', async function() {
            const printType = this.getAttribute('data-printtype');
            const mapping = await getMapping();
            mapping[printType] = this.value;
            await saveMapping(mapping);
            console.log(`Mapping updated: ${printType} -> ${this.value}`);
        });
    });
    
    document.querySelectorAll('.mapping-delete-btn').forEach(button => {
        button.addEventListener('click', async function() {
            const printType = this.getAttribute('data-printtype');
            const mapping = await getMapping();
            delete mapping[printType];
            await saveMapping(mapping);
            updateMappingTable();
            console.log(`Mapping deleted: ${printType}`);
        });
    });
}

async function loadProfilesForMapping() {
    return new Promise((resolve) => {
        chrome.storage.local.get(['profiles'], (result) => {
            const profiles = result.profiles || {
                'A4': Profile.getA4Profile().toStorageObject(),
                'Thermo': Profile.getThermoProfile().toStorageObject(),
            };
            resolve(profiles);
        });
    });
}

chrome.storage.onChanged.addListener((changes, namespace) => {
    if (namespace === 'local' && changes.profiles) {
        updateMappingTable();
    }
});
