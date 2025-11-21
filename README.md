# PrintaDot

A Chrome extension for printing barcodes with customizable printing profiles and settings.

⚠️ WARNING - Native application required for correct work of extension. Latest version of exe you can find [here.](https://github.com/yourusername/barcode-printer-extension/releases/latest)

## Features

- **Popup Interface**: Quick access to barcode printing with header, barcode input, and profile selection
- **Customizable Printing Profiles**: Configure paper size, label dimensions, margins, text formatting, and more
- **Native Application Integration**: Requires companion desktop application for printing functionality
- **Developer API**: Programmatic printing from any website

## Usage

### Basic Printing

1. Click on the pinned extension icon in the browser toolbar
2. Enter the required data in the pop-up window:
   - Header text (optional)
   - Barcode content
   - Select printing profile
3. Press the **"Print"** button
4. Use the **"Settings"** button to configure printing profiles

![Popup Interface](./materials/images/popup.png)

### Printing Settings

The extension provides comprehensive printing configuration:

- **Paper Settings**: Paper height, width, margins, offsets
- **Label Layout**: Label dimensions, labels per row/column
- **Text Formatting**: Font size, alignment, maximum length, rotation
- **Barcode Options**: DataMatrix support, barcode alignment and rotation
- **Number Settings**: Font size, alignment, and rotation for figures
- **Printer Selection**: Choose specific printer

![Popup Interface](./materials/images/options.png)

## Installation for developers

1. Open Chrome browser and navigate to: **Extensions** → **Enable Developer Mode**
2. Click **"Load unpacked extension"**, select the extension folder from the project, and click **"OK"**
3. Copy the extension identifier
4. Open `PrintaDot.sln` file in Visual Studio
5. Go to `Manifest.cs` file and paste your extension identifier into `AllowedOrigins` in the format: `chrome-extension://Your_Identifier/`
6. Build the solution and run `PrintaDot.Windows` project
7. Once the manifest is generated and you see "application ready to work" in console, you can close it
8. Enable the extension and pin it to the browser toolbar

#  PrintaDotClient (for developers)

Helper class that communicates with the PrintaDot browser extension and the native host application.

### sendPrintRequest

Before printing, method **sendPrintRequest** ensure that both parts of the system are available:

- Browser extension connection.
If the extension is not installed or does not respond, an error is thrown.

- Native host connection
The method confirms that the native printing application is running and reachable.


```javascript
  /**
   * Send print request to PrintaDot extension.
   *
   * @param {PrintItem[]} items - array of PrintItem objects
   * @param {string} printType - print profile
   * @returns {Promise<void>}
   */
  async sendPrintRequest(items, printType = "default")
```

## PrintItem

Represents a single printable entity passed to the PrintaDot extension.
Each item contains a mandatory barcode value and optional header/figures text.



```javascript
/**
 * @typedef PrintItem
 * @property {string?} header  - Optional header text
 * @property {string} barcode  - Barcode value (required)
 * @property {string?} figures - Optional figures string
 */
class PrintItem {
  constructor(header, barcode, figures = null) {
    this.header = header;
    this.barcode = barcode;
    this.figures = figures;
  }
}
```
