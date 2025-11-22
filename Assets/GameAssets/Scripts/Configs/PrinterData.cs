using System;
using System.Collections.Generic;

public static class PrinterData
{
    public static readonly Dictionary<string, PrinterType> Types =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["FDM"] = new PrinterType
            {
                minigames = new List<MinigameType>
                    { MinigameType.Modelling, MinigameType.Slicer, MinigameType.Postprocess },
                description = "Fused Deposition Modeling (FDM)",
                level = 1
            },
            ["SLA"] = new PrinterType
            {
                minigames = new List<MinigameType> { MinigameType.Empty },
                description = "Stereolithography (SLA)",
                level = 2
            }
        };
}

public enum MinigameType
{
    Empty,
    Slicer,
    Postprocess,
    Modelling,
    Scanning
}

public class PrinterType
{
    public List<MinigameType> minigames { get; set; }
    public string description { get; set; }
    public int level { get; set; }
}