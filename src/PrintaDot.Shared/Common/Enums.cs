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
    Queued = 0,
    Printing = 1,
    Success = 2,
    Error = 3,
    Unknown = 4,
}
