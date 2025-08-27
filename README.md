# PrintLink

Installation and Launch

Open the Chrome browser and navigate to the menu:
Extensions → Enable Developer Mode

Click "Load unpacked extension", select the extension folder from the project, and click "OK".

Copy the extension identifier.

Open the PrintLink.sln file in Visual Studio, go to the Manifest.cs file, and paste the identifier into AllowedOrigins in the following format:
chrome-extension://**Your Identifier**/

Build the project and run it. Once the manifest is generated, you can close it.

Enable the extension and pin it to the browser toolbar.

To use the extension, click on the pinned icon, enter the required data in the pop-up window, and press the "Print" button.