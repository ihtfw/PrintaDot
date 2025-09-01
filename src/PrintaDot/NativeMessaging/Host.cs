using PrintaDot.Common;
using PrintaDot.NativeMessaging.CommunicationProtocol;
using PrintaDot.NativeMessaging.CommunicationProtocol.V1;
using PrintaDot.Printing;

namespace PrintaDot.NativeMessaging
{
    /// <summary>
    /// Abstract class that should be extended to communicate with browsers
    /// </summary>
    public class Host
    {
        private readonly Manifest _manifest;
        private bool _sendConfirmationReceipt;
        private readonly PrintService _printService;

        /// <summary>
        /// List of supported browsers.
        /// </summary>
        public List<Browser> SupportedBrowsers { get; }

        /// <summary>
        /// Creates the Host object
        /// </summary>
        /// <param name="sendConfirmationReceipt"><see langword="true" /> for the host to automatically send message confirmation receipt.</param>
        public Host(bool sendConfirmationReceipt = true)
        {
            SupportedBrowsers = new List<Browser>(2);

            _sendConfirmationReceipt = sendConfirmationReceipt;

            _manifest = new Manifest();
            _printService = new PrintService();
        }

        /// <summary>
        /// Starts listening for input.
        /// </summary>
        public void Listen()
        {
            if (!IsRegistered())
            {
                throw new NotRegisteredWithBrowserException(_manifest.HostName);
            }

            Message? message;

            while ((message = PrintaDotStreamHandler.Read()) != null)
            {
                var exactMessage = message as PrintRequestMessageV1;

                if (message != null)
                {
                    Print(exactMessage);
                }

                Log.LogMessage(
                    "Data Received:" + message.ToJson());

                if (_sendConfirmationReceipt)
                {
                    PrintaDotStreamHandler.Write(exactMessage.ToJson());
                }
            }
        }

        private void Print(PrintRequestMessageV1 message)
        {
            try
            {
                _printService.PrintRequestMessageV1(message);
                Log.LogMessage($"Print job completed: {message.Version} - {message.Type}");
            }
            catch (Exception ex)
            {
                Log.LogMessage($"Print error: {ex.Message}");
            }
        }

        #region Chromium Native Messaging Manifest
        /// <summary>
        /// Generates the manifest and saves it to the correct location.
        /// </summary>
        /// <param name="description">Short application description to be included in the manifest.</param>
        /// <param name="allowedOrigins">List of extensions that should have access to the native messaging host.<br />Wildcards such as <code>chrome-extension://*/*</code> are not allowed.</param>
        /// <param name="overwrite">Determines if the manifest should be overwritten if it already exists.<br />Defaults to <see langword="false"/>.</param>
        public void GenerateManifest(bool overwrite = true)
        {
            if (File.Exists(_manifest.ManifestPath) && !overwrite)
            {
                Log.LogMessage("Manifest exists already");
            }
            else
            {
                Log.LogMessage("Generating Manifest");

                string manifest = _manifest.ToJson();

                File.WriteAllText(_manifest.ManifestPath, manifest);

                Log.LogMessage("Manifest Generated");
            }
        }

        /// <summary>
        /// Removes the manifest from application folder
        /// </summary>
        public void RemoveManifest()
        {
            if (File.Exists(_manifest.ManifestPath))
            {
                File.Delete(_manifest.ManifestPath);
            }
        }
        #endregion

        #region Browser Registration
        /// <summary>
        /// Checks if the host is registered with all required browsers.
        /// </summary>
        /// <returns><see langword="true"/> if the required information is present in the registry.</returns>
        public bool IsRegistered()
        {
            bool result = false;

            foreach (Browser browser in SupportedBrowsers)
            {
                result = result || browser.IsRegistered(_manifest.HostName, _manifest.ManifestPath);
            }

            return result;
        }

        /// <summary>
        /// Register the application to open with all required browsers.
        /// </summary>
        public void Register()
        {
            foreach (Browser browser in SupportedBrowsers)
            {
                browser.Register(_manifest.HostName, _manifest.ManifestPath);
            }
        }

        /// <summary>
        /// De-register the application to open with all required browsers.
        /// </summary>
        public void Unregister()
        {
            foreach (Browser browser in SupportedBrowsers)
            {
                browser.Unregister(_manifest.HostName);
            }
        }
        #endregion
    }
}
