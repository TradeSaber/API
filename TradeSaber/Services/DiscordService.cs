using System.IO;
using System.Net.Http;
using System.Text.Json;
using TradeSaber.Settings;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using TradeSaber.Models.Discord;
using System.Collections.Generic;

namespace TradeSaber.Services
{
    public class DiscordService
    {
        private readonly HttpClient _client;
        private readonly DiscordSettings _discordSettings;
    
        public DiscordService(HttpClient client, DiscordSettings discordSettings)
        {
            _client = client;
            _discordSettings = discordSettings;
        }

        public async Task<string> GetAccessToken(string code)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>
            {
                { "client_id", _discordSettings.ID },
                { "client_secret", _discordSettings.Secret },
                { "grant_type", "authorization_code" },
                { "code", code },
                { "redirect_uri", _discordSettings.RedirectURL }
            };
            FormUrlEncodedContent content = new FormUrlEncodedContent(parameters);
            HttpResponseMessage response = await _client.PostAsync(_discordSettings.URL + "/oauth2/token", content);
            if (response.IsSuccessStatusCode)
            {
                Stream responseStream = await response.Content.ReadAsStreamAsync();
                AccessTokenResponse accessTokenResponse = await JsonSerializer.DeserializeAsync<AccessTokenResponse>(responseStream);
                return accessTokenResponse.AccessToken;
            }
            return null;
        }

        public async Task<DiscordUser> GetProfile(string accessToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await _client.GetAsync(_discordSettings.URL + "/users/@me");
            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                DiscordUser discordUser = JsonSerializer.Deserialize<DiscordUser>(responseString);
                return discordUser;
            }
            return null;
        }
    }
}