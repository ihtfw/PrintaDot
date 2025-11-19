using Microsoft.JSInterop;
using PrintaDot.Shared.Common;
using PrintaDot.Shared.CommunicationProtocol.V1.Requests;

namespace PrintaDot.Blazor;

public static class JsRuntimeEx
{
    public static async ValueTask<bool> Print(this IJSRuntime jsRuntime, PrintRequestMessageV1 message, Action<Exception>? errorCallback = null)
    {
        try
        {
            return await jsRuntime.InvokeAsync<bool>("printCommunicator.sendPrintRequest", message.ToJson());
        }
        catch (Exception e)
        {
            errorCallback?.Invoke(e);
        }

        return false;
    }
}
