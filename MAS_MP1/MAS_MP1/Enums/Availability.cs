using System.ComponentModel;

namespace MAS_MP1.Enums;

public enum Availability
{
    [Description("In stock")]
    InStock,
    [Description("Low stock")]
    LowStock,
    [Description("Out of stock")]
    OutOfStock,
}