using Microsoft.JSInterop;

namespace PrintaDot.Blazor
{
    public interface IPrintaDotClient
    {
        Task CheckExtensionConnectionAsync();
        Task CheckNativeAppConnectionAsync();
        Task SendPrintRequestAsync(IEnumerable<PrintItem> items, string printType = "default");
    }

    public class PrintaDotClient : IPrintaDotClient, IAsyncDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private IJSObjectReference? _module;
        private IJSObjectReference? _client;
        private bool _isInitialized = false;

        public PrintaDotClient(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        private async Task EnsureInitializedAsync() // потокобезопасный (симафор либо таску)
        {
            if (!_isInitialized)
            {
                _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                    "import",
                    "./_content/PrintaDot.Blazor/printaDotBlazor.js"
                );

                _client = await _module.InvokeAsync<IJSObjectReference>("createPrintaDotClient");
                _isInitialized = true;
            }
        }

        public async Task CheckExtensionConnectionAsync()
        {
            await EnsureInitializedAsync();
            await _module!.InvokeVoidAsync("checkExtensionConnection", _client);
        }

        public async Task CheckNativeAppConnectionAsync()
        {
            await EnsureInitializedAsync();
            await _module!.InvokeVoidAsync("checkNativeAppConnection", _client);
        }

        public async Task SendPrintRequestAsync(IEnumerable<PrintItem> items, string printType = "default")
        {
            if (items == null || !items.Any())
                throw new ArgumentException("Items cannot be null or empty", nameof(items));

            await EnsureInitializedAsync();
            await _module!.InvokeVoidAsync("sendPrintRequest", _client, items, printType);
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
            {
                try
                {
                    await _module.DisposeAsync();
                }
                catch
                {
                    //js interop already disposed
                }
            }

            _isInitialized = false;
        }
    }
}