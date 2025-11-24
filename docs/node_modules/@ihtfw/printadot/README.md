# @ihtfw/printadot

Client library for PrintaDot browser extension communication. This package allows web applications to communicate with the PrintaDot browser extension to print labels with barcodes.

## Installation

```bash
npm install @ihtfw/printadot
```

## Usage

### Basic Example

```javascript
import { PrintaDotClient, PrintItem } from "@ihtfw/printadot";

// Create a client instance
const client = new PrintaDotClient();

// Create print items
const items = [
  new PrintItem("Product A", "1234567890", "Size: L"),
  new PrintItem("Product B", "0987654321", "Size: M"),
  new PrintItem(null, "1122334455", null), // header and figures are optional
];

// Send print request
try {
  await client.sendPrintRequest(items, "default");
  console.log("Print job sent successfully");
} catch (error) {
  console.error("Print failed:", error.message);
}
```

### Checking Connections

Before printing, you can verify that both the extension and native application are connected:

```javascript
const client = new PrintaDotClient();

// Check extension connection
try {
  await client.checkExtensionConnection();
  console.log("Extension is connected");
} catch (error) {
  console.error("Extension not available:", error.message);
}

// Check native app connection
try {
  await client.checkNativeAppConnection();
  console.log("Native app is connected");
} catch (error) {
  console.error("Native app not available:", error.message);
}

// Or check both at once
try {
  await client.checkAllConnections();
  console.log("All connections are ready");
} catch (error) {
  console.error("Connection check failed:", error.message);
}
```

## API

### PrintItem

Class representing an item to be printed.

**Constructor:**

```javascript
new PrintItem(header, barcode, figures);
```

**Parameters:**

- `header` (string | null) - Optional header text
- `barcode` (string) - Required barcode string
- `figures` (string | null) - Optional figures/additional text

### PrintaDotClient

Main client class for communicating with the PrintaDot extension.

#### Methods

##### `checkExtensionConnection()`

Checks if the PrintaDot browser extension is installed and connected.

**Returns:** `Promise<Object>` - Response object with connection status

**Throws:** `Error` if extension is not connected

##### `checkNativeAppConnection()`

Checks if the native PrintaDot application is running and connected.

**Returns:** `Promise<Object>` - Response object with connection status

**Throws:** `Error` if native app is not connected

##### `checkAllConnections()`

Checks both extension and native app connections.

**Returns:** `Promise<void>`

**Throws:** `Error` if any connection check fails

##### `sendPrintRequest(items, printType)`

Sends a print request to the PrintaDot extension.

**Parameters:**

- `items` (PrintItem[]) - Array of PrintItem objects to print (required)
- `printType` (string) - Print profile name (default: 'default')

**Returns:** `Promise<void>`

**Throws:** `Error` if:

- items is not an array
- items array is empty
- any item has invalid properties
- connection checks fail
- print request fails or times out

**Example:**

```javascript
const items = [
  new PrintItem("Header 1", "BARCODE123", "Info text"),
  new PrintItem(null, "BARCODE456", null),
];

await client.sendPrintRequest(items, "myProfile");
```

## Requirements

- PrintaDot browser extension must be installed
- PrintaDot native application must be running
- Modern browser with ES6+ support

## License

MIT

## Authors

- Vadim Kandrushin
- Dmitry Alekseev
