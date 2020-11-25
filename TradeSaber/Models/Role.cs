using System;

namespace TradeSaber.Models
{
    [Flags]
    public enum Role
    {
        None = 0,
        Root = 1,
        Owner = 2,
        Admin = 4,
        Supporter = 8,
        Trusted = 16
    }
}