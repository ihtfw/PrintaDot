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

async function showNativeAppNotInstalledView() {
    const content = document.querySelector('.content');

    const title = await getMessage('nativeAppNotInstalledTitle');
    const message = await getMessage('nativeAppNotInstalledMessage');
    const downloadBtn = await getMessage('downloadNativeAppButton');
    
    content.innerHTML = `
        <div class="native-app-warning">
            <div class="warning-icon">
                <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"></path>
                    <line x1="12" y1="9" x2="12" y2="13"></line>
                    <line x1="12" y1="17" x2="12.01" y2="17"></line>
                </svg>
            </div>
            <h3>${title}</h3>
            <p>${message}</p>
            <div class="warning-actions">
                <button id="downloadNativeAppBtn" class="main-btn">${downloadBtn}</button>
            </div>
        </div>
    `;

    document.getElementById('closeBtn').addEventListener('click', function() {
        window.close();
    });;

    document.getElementById('downloadNativeAppBtn').addEventListener('click', downloadNativeApp);
}

async function showNormalView() {
    const content = document.querySelector('.content');
    
    const headerLabel = await getMessage('headerLabel');
    const headerPlaceholder = await getMessage('headerPlaceholder');
    const barcodeLabel = await getMessage('barcodeLabel');
    const barcodePlaceholder = await getMessage('barcodePlaceholder');
    const profileLabel = await getMessage('profileLabel') || 'Profile';
    const printButton = await getMessage('printButton');
    const settingsButton = await getMessage('settingsButton');
    const settingsTitle = await getMessage('settingsTitle');
    
    content.innerHTML = `
        <div class="input-group">
            <label for="headerInput">${headerLabel}</label>
            <input type="text" id="headerInput" placeholder="${headerPlaceholder}">
        </div>

        <div class="input-group">
            <label for="barcodeInput">${barcodeLabel}</label>
            <input type="text" id="barcodeInput" placeholder="${barcodePlaceholder}">
        </div>

        <div class="input-group">
            <label for="profileSelect">${profileLabel}</label>
            <select id="profileSelect"></select>
        </div>

        <div class="button-group">
            <button id="printBtn" class="main-btn">${printButton}</button>
            <button id="settingsBtn" class="main-btn btn-secondary"
                title="${settingsTitle}">${settingsButton}</button>
        </div>
    `;

    await loadProfilesForPopup();
    initNormalViewHandlers();
}

async function loadProfilesForPopup() {
    return new Promise((resolve) => {
        chrome.storage.local.get(['profiles', 'currentProfileName'], (result) => {
            const profiles = result.profiles || {
                'A4': Profile.getA4Profile().toStorageObject(),
                'Thermo': Profile.getThermoProfile().toStorageObject(),
            };
            
            const currentProfileName = result.currentProfileName || 'Thermo';
            const profileSelect = document.getElementById('profileSelect');
            
            profileSelect.innerHTML = '';
            
            Object.keys(profiles).forEach(profileName => {
                const option = document.createElement('option');
                option.value = profileName;
                option.textContent = profileName;
                profileSelect.appendChild(option);
            });
            
            profileSelect.value = currentProfileName;
            
            resolve(profiles);
        });
    });
}

function initNormalViewHandlers() {
    const printBtn = document.getElementById('printBtn');
    const settingsBtn = document.getElementById('settingsBtn');
    const headerInput = document.getElementById('headerInput');
    const barcodeInput = document.getElementById('barcodeInput');
    const profileSelect = document.getElementById('profileSelect');

    printBtn.addEventListener('click', handlePrint);
    settingsBtn.addEventListener('click', function() {
        chrome.runtime.openOptionsPage();
    });

    headerInput.addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            barcodeInput.focus();
        }
    });

    barcodeInput.addEventListener('keypress', function(e) {
        if (e.key === 'Enter') {
            e.preventDefault();
            handlePrint();
        }
    });

    profileSelect.addEventListener('change', function() {
        chrome.storage.local.set({ currentProfileName: this.value });
    });

    headerInput.focus();
}

function handlePrint() {
    const headerInput = document.getElementById('headerInput');
    const barcodeInput = document.getElementById('barcodeInput');
    const profileSelect = document.getElementById('profileSelect');
    
    const header = headerInput.value.trim();
    const barcode = barcodeInput.value.trim();
    const profile = profileSelect.value;

    if (!barcode) {
        barcodeInput.focus();
        showError("Fill in barcode input");
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

async function downloadNativeApp() {
    try {
        const apiUrl = 'https://api.github.com/repos/ihtfw/PrintaDot/releases/latest';
        const response = await fetch(apiUrl);
        
        if (!response.ok) {
            throw new Error('Failed to fetch release info');
        }
        
        const releaseData = await response.json();
        const latestVersion = releaseData.tag_name;
        
        const windowsAsset = releaseData.assets.find(asset => 
            asset.name === 'PrintaDot.Windows.exe' || 
            asset.name.includes('PrintaDot.Windows')
        );
        
        if (windowsAsset) {
            const downloadUrl = windowsAsset.browser_download_url;
            
            const a = document.createElement('a');
            a.href = downloadUrl;
            a.download = windowsAsset.name;
            a.style.display = 'none';
            
            document.body.appendChild(a);
            a.click();
            document.body.removeChild(a);
        } else {
            chrome.tabs.create({
                url: releaseData.html_url
            });
        }
        
    } catch (error) {
        console.error('Error fetching latest release:', error);
        chrome.tabs.create({
            url: 'https://github.com/ihtfw/PrintaDot/releases'
        });
    }
}

function checkNativeAppConnection() {
    return new Promise((resolve) => {
        chrome.runtime.sendMessage(
            { type: "CheckConnetcionToNativeApp" },
            (response) => {
                if (chrome.runtime.lastError) {
                    resolve(false);
                } else {
                    resolve(response?.isConnected || false);
                }
            }
        );    
    });
}

async function getMessage(key) {
    return new Promise((resolve) => {
        if (window.currentMessages && window.currentMessages[key]) {
            resolve(window.currentMessages[key].message || key);
            return;
        }
        
        resolve(key);
    });
}

chrome.runtime.onMessage.addListener((request, sender, sendResponse) => {
    if (request.type === "Exception" && request.messageText) {
        showError(request.messageText);
    }
    
    if (request.type === "CheckConnetcionToNativeAppResponse") {
        return true;
    }
});

document.addEventListener('DOMContentLoaded', async function() {
    await initLocalization();
    
    document.getElementById('errorCloseBtn').addEventListener('click', hideError);
    document.getElementById('closeBtn').addEventListener('click', function() {
        window.close();
    });;

    const isConnected = await checkNativeAppConnection();
    
    if (!isConnected) {
        await showNativeAppNotInstalledView();
    } else {
        await showNormalView();
    }
});