using System.IO;
using System.Net.Http;
using System.Text.Json;
using TradeSaber.Settings;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using TradeSaber.Models.Discord;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace TradeSaber.Services
{
    public class DiscordService
    {
        private readonly HttpClient _client;
        private readonly ILogger<DiscordService> _logger;
        private readonly DiscordSettings _discordSettings;
    
        public DiscordService(HttpClient client, ILogger<DiscordService> logger, DiscordSettings discordSettings)
        {
            _client = client;
            _logger = logger;
            _discordSettings = discordSettings;
        }

        public async Task<string> GetAccessToken(string code)
        {
            _logger.LogDebug("Fetching Access Token");
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
                _logger.LogDebug("Received Access Token");
                Stream responseStream = await response.Content.ReadAsStreamAsync();
                AccessTokenResponse accessTokenResponse = await JsonSerializer.DeserializeAsync<AccessTokenResponse>(responseStream);
                return accessTokenResponse.AccessToken;
            }
            else
            {
                _logger.LogWarning("Could not get access token. {ReasonPhrase}", response.ReasonPhrase);
            }
            return null;
        }

        public async Task<DiscordUser> GetProfile(string accessToken)
        {
            _logger.LogDebug("Getting active user profile.");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            HttpResponseMessage response = await _client.GetAsync(_discordSettings.URL + "/users/@me");
            if (response.IsSuccessStatusCode)
            {
                string responseString = await response.Content.ReadAsStringAsync();
                DiscordUser discordUser = JsonSerializer.Deserialize<DiscordUser>(responseString);
                _logger.LogDebug("User Profile {Username}#{Discriminator} Found", discordUser.Username, discordUser.Discriminator);
                return discordUser;
            }
            else
            {
                _logger.LogWarning("Could not get user profile. {ReasonPhrase}", response.ReasonPhrase);
            }
            return null;
        }
    }
}