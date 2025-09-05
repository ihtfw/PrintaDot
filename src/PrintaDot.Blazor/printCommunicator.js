window.printCommunicator = {
    sendPrintRequest: function (jsonData) {
        var printData = JSON.parse(jsonData);

        window.postMessage(printData, '*');
    },

    subscribeToPrintResponses: function (dotNetHelper) {
        window.addEventListener('message', (event) => {
            dotNetHelper.invokeMethodAsync('HandlePrintResponse', event.data)
                .catch(error => console.error('Error invoking .NET method:', error));
        });
    }
};