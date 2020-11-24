using System;

namespace TradeSaber.Models
{
    [Flags]
    public enum Role
    {
        None = 0,
        Owner = 1,
        Admin = 2,
        Supporter = 4,
        Trusted = 8
    }
}