namespace PrintaDot.Blazor;

public class PrintItem
{
    public string? Header { get; set; }
    public required string Barcode { get; set; }
    public string? Figures { get; set; }
}
