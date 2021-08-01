using System;

namespace TradeSaber.Models
{
    public class User
    {
        public Guid ID { get; set; }
        public Role? Role { get; set; }
    }
}