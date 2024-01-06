using System.ComponentModel;
using System.Reflection;

namespace Domain;

public enum ECardColor
{
    [Description("🟥")]
    Red,
    
    [Description("🟦")]
    Blue,
    
    [Description("🟩️")]
    Green,
    
    [Description("🟨️")]
    Yellow,
    
    [Description("⬛️️")]
    Wild,
    
    Null
}
