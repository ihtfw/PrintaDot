namespace PrintaDot.Shared.Printing;

public class PaperSettings
{
    public float Height { get; init; }
    public float Width { get; init; }
    public int LabelsPerRow {  get; init; }
    public int LabelsPerColumn { get; init; }
    public int? Offset {  get; init; }
    public int? Repeat { get; init; }
}
