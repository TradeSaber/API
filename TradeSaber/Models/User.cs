using System;
using NodaTime;
using TradeSaber.Models.Discord;

namespace TradeSaber.Models
{
    public class User : IEquatable<User>
    {
        #region User Info

        public Guid ID { get; set; }
        public DiscordUser Profile { get; set; }
        public UserState State { get; set; }
        public Role Role { get; set; }

        #endregion

        #region Statistics

        public int Level { get; set; } = 0;
        public float TirCoin { get; set; } = 0f;
        public long Experience { get; set; } = 0;

        #endregion

        #region Time 

        public Instant Created { get; set; }
        public Instant LastLoggedIn { get; set; }

        #endregion

        #region Equatables 

        public bool Equals(User other)
        {
            return ID == other.ID;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as User);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        #endregion
    }
}