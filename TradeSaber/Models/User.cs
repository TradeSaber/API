using System;
using NodaTime;
using TradeSaber.Models.Discord;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TradeSaber.Models
{
    public class User : IEquatable<User>
    {
        #region User Info

        public Guid ID { get; set; }

        [Column(TypeName = "jsonb")]
        public DiscordUser Profile { get; set; } = null!;
        public UserState State { get; set; }
        public Role Role { get; set; }

        #endregion

        #region Statistics

        public int Level { get; set; } = 0;
        public float TirCoin { get; set; } = 0f;
        public long Experience { get; set; } = 0;
        public IList<Session> Sessions { get; set; } = null!;

        #endregion

        #region Time 

        public Instant Created { get; set; }
        public Instant LastLoggedIn { get; set; }

        #endregion

        #region Inventory

        public IList<Card> Cards { get; set; } = null!;
        public IList<Pack> Packs { get; set; } = null!;

        #endregion

        #region Equatables 

        public bool Equals(User? other)
        {
            return ID == other?.ID;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as User);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public static bool operator !=(User u1, User u2)
        {
            return !(u1 == u2);
        }

        public static bool operator ==(User u1, User u2)
        {
            return u1 == u2 || u1.Equals(u2);
        }

        #endregion
    }
}