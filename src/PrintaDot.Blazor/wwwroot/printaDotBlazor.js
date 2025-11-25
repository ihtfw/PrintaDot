import { PrintaDotClient, PrintItem } from './printaDotClient.js';

export function createPrintaDotClient() {
    return new PrintaDotClient();
}

export async function checkExtensionConnection(client) {
    return await client.checkExtensionConnection();
}

export async function checkNativeAppConnection(client) {
    return await client.checkNativeAppConnection();
}

export async function checkAllConnections(client) {
    return await client.checkAllConnections();
}

export async function sendPrintRequest(client, items, printType) {
    const jsItems = items.map(item =>
        new PrintItem(item.header, item.barcode, item.figures)
    );

    return await client.sendPrintRequest(jsItems, printType);
}