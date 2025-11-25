# PrintaDot.Blazor

`PrintaDot.Blazor` is a Blazor library designed for
interacting with a PrintaDot extension or native app to
send print requests via JavaScript Interop.

The library provides the `IPrintaDotClient` interface and its implementation
`PrintaDotClient`, which encapsulate work with a JavaScript module and
provide a convenient API for checking the connection and sending print data.


## Installation

```bash
dotnet nuget install PrintaDot.Blazor
```

To register a client in the DI container, use the extension in `Program.cs`:

``` csharp
builder.Services.AddPrintaDotClient();
```

## Usage

### Basic Example

``` csharp
@inject IPrintaDotClient PrintaDot

<button @onclick="Print">Print</button>

@code {
    private async Task Print()
    {
        var items = new List<PrintItem>
        {
            new PrintItem
            {
                Header = "Item #1",
                Barcode = "1234567890123",
                Figures = "Additional data"
            }
        };

        try {
            await PrintaDot.SendPrintRequestAsync(items);
        } catch (Exception ex) {
            //log exception
        }
    }
}
```

### Checking Connections

``` csharp
        // Check extension connection
        try {
            await PrintaDot.CheckExtensionConnectionAsync();
        } catch (Exception ex) {
            //if not connected exception will be thrown
        }
```
or
``` csharp
        // Check native app connection
        try {
            await PrintaDot.CheckNativeAppConnectionAsync();
        } catch (Exception ex) {
            //if not connected exception will be thrown
        }
```

## API

### Interface `IPrintaDotClient`

``` csharp
public interface IPrintaDotClient
{
    Task CheckExtensionConnectionAsync();
    Task CheckNativeAppConnectionAsync();
    Task SendPrintRequestAsync(IEnumerable<PrintItem> items, string printType = "default");
}
```

### Methods

### `CheckExtensionConnectionAsync()`

Checks the connection with the PrintaDot browser extension.

### `CheckNativeAppConnectionAsync()`

Checks the connection with the native PrintaDot desktop application.

### `SendPrintRequestAsync(IEnumerable<PrintItem> items, string printType = "default")`

Sends a print request.
- `items` --- a collection of `PrintItem` objects
- `printType` --- the print type (default is `default`)

If the collection is empty, an exception is thrown.


## The `PrintaDotClient` Class

`PrintaDotClient` is an interface implementation that interacts with
JavaScript via `IJSRuntime`.

### Main features:

- Lazy and thread-safe initialization of the JS module
(`EnsureInitializedAsync`).
- JS function calls:
- `checkExtensionConnection`
- `checkNativeAppConnection`
- `sendPrintRequest`
- Implementation of `IAsyncDisposable` for graceful release

of the JS module.

### JavaScript Module

The client loads the JS file:

./_content/PrintaDot.Blazor/printaDotBlazor.js

And calls the function in it:

``` js
createPrintaDotClient();
```

which returns a JavaScript client object.

## PrintItem Model

``` csharp
public class PrintItem
{
    public string? Header { get; set; }
    public required string Barcode { get; set; }
    public string? Figures { get; set; }
}
```

### Fields:

- `Header` --- Header
- `Barcode` --- Barcode (required)
- `Figures` --- Additional data

### Example 
``` csharp
var items = new List<PrintItem>
{
    new PrintItem
    {
        Header = "Item #1",
        Barcode = "1234567890123",
        Figures = "Additional data"
    }
};

await PrintaDot.SendPrintRequestAsync(items);
```

## Shutdown

`PrintaDotClient` automatically releases resources when destroyed
(via `DisposeAsync`), closing the JS module.

## Requirements

- PrintaDot browser extension must be installed
- PrintaDot native application must be running
- Modern browser with ES6+ support
- Blazor application

## License

MIT

## Authors

- Vadim Kandrushin
- Dmitry Alekseev