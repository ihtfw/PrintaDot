let currentMessages = {};

async function loadMessages(lang) {
    const url = chrome.runtime.getURL(`_locales/${lang}/messages.json`);
    const res = await fetch(url);
    currentMessages = await res.json();
}

function getMessage(key) {
    if (currentMessages[key] && currentMessages[key].message) {
        return currentMessages[key].message;
    }
    return key;
}

async function localizeHtmlPage(lang) {
    await loadMessages(lang);

    var objects = document.getElementsByTagName('html');
    for (var j = 0; j < objects.length; j++) {
        var obj = objects[j];
        var valStrH = obj.innerHTML.toString();

        var valNewH = valStrH.replace(/__MSG_(\w+)__/g, function (match, v1) {
            return v1 ? getMessage(v1) : "";
        });

        if (valNewH !== valStrH) {
            obj.innerHTML = valNewH;
        }
    }
}

async function initLocalization() {
    const savedLang = localStorage.getItem("appLanguage") || "en";
    localStorage.setItem("appLanguage", savedLang); 

    const langSelect = document.getElementById("languageSelect");
    if (langSelect) {
        langSelect.value = savedLang;

        langSelect.addEventListener("change", async function () {
            const newLang = langSelect.value;
            localStorage.setItem("appLanguage", newLang);
            await localizeHtmlPage(newLang);
        });
    }

    await localizeHtmlPage(savedLang);
}
