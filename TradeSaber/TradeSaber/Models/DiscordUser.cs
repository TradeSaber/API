using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradeSaber.Models
{
    public class DiscordUser
    {
        private string _avatar;
        public string ID { get; set; }
        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string Avatar
        {
            get => _avatar;
            set => _avatar = value.StartsWith("http") ? value : ("https://cdn.discordapp.com/avatars/" + ID + "/" + value + (value.Substring(0, 2) == "a_" ? ".gif" : ".png"));
        }
    }
}
