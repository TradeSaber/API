using System.Text.Json.Serialization;

namespace TradeSaber.Models.Discord
{
    public record DiscordUser(string Id, string Username, string Discriminator, string Avatar)
    {
        private string _avatar;

        [JsonPropertyName("avatar")]
        public string Avatar
        {
            get => _avatar;
            set => _avatar = value.StartsWith("http") ? value : ("https://cdn.discordapp.com/avatars/" + Id + "/" + value + (value.Substring(0, 2) == "a_" ? ".gif" : ".png"));
        }
    }
}