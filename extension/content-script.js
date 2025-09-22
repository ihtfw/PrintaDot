const MAPPING_KEY = "printTypeMapping";

function getMapping() {
    const json = localStorage.getItem(MAPPING_KEY);
    return json ? JSON.parse(json) : {};
}

function saveMapping(mapping) {
    localStorage.setItem(MAPPING_KEY, JSON.stringify(mapping));
}

window.addEventListener("message", (event) => {
    if (event.data.type === 'PrintRequest') {
        console.log(event.data);
        const mapping = getMapping();

        let printType = event.data.printType;

        if (!mapping[printType]) {
            mapping[printType] = "default";
            saveMapping(mapping);
        }

        const mappedRequest = {
            ...event.data,
            profile: mapping[printType],
        };

        console.log(mappedRequest);

        chrome.runtime.sendMessage({
            type: event.data.type,
            version: event.data.version,
            profile: mapping[printType],
            items: event.data.items
        });
    }
});
