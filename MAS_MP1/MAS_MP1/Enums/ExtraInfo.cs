using System.ComponentModel;

namespace MAS_MP1.Enums;

public enum ExtraInfo
{
    [Description("PEGI +18")]
    Adult,
    [Description("PEGI +12")]
    Teen,
    [Description("For everyone")]
    Kids,
    [Description("Multiplayer")]
    Multiplayer,
    [Description("Online only")]
    OnlineOnly,
    [Description("Singleplayer")]
    Singleplayer,
    [Description("Co-op")]
    Coop,
    [Description("Multiplatform")]
    Multiplatform,
    [Description("Inluceds microtransactions")]
    Microtransactions
}