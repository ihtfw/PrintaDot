namespace PrintaDot.Shared.Common;

public enum Aligment
{
    Left,
    Right,
    Top,
    Buttom
}

public enum PrintStatus
{
    Queued,
    Printing,
    Success,
    Error,
    Unknown,
}

public enum MessageType
{
    PrintRequest,
    GetPrintStatusRequest,
    GetPrintStatusResponse,
    GetPrintersRequest,
    Exception
}

public enum ResponseType
{
    Exception,
    PrintResponse,
    GetPrintersResponse,
    UpdateNativeApp
}
