using System;

namespace TradeSaber.Models
{
    [Flags]
    public enum TradeSaberRole
    {
        None = 0,
        Verified = 1,
        Trusted = 2,
        Supporter = 4,
        Admin = 8,
        Owner = 16
    }
}