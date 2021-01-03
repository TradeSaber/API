using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using TradeSaber.Models;
using TradeSaber.Settings;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TradeSaber.Services
{
    public class HTIService
    {
        private readonly HttpClient _httpClient;
        private readonly HTISettings _htiSettings;
        private readonly ILogger<HTIService> _logger;
        private readonly RaritySettings _raritySettings;

        // subColor
        // mainColor
        // cardBase64
        // rarityColor
        // cardName
        // cardDesc
        // cardRarity

        public HTIService(ILogger<HTIService> logger, HttpClient httpClient, HTISettings htiSettings, RaritySettings raritySettings)
        {
            _logger = logger;
            _httpClient = httpClient;
            _htiSettings = htiSettings;
            _raritySettings = raritySettings;
        }

        public async Task<byte[]?> Generate(string cardName, string cardDesc, string mainColor, string subColor, Rarity rarity, Stream stream, string extension)
        {
            string cardRarity = rarity.ToString();
            string rarityColor = ColorForRarity(rarity);

            _logger.LogDebug("Generating memory stream.");
            stream.Position = 0;
            using MemoryStream ms = new MemoryStream();
            await stream.CopyToAsync(ms);

            _logger.LogInformation ("Converting stream to Base64.");
            string cardBase64 = $"data:image/{extension.ToLower().Replace(".", "")};base64," + Convert.ToBase64String(ms.ToArray());

            _logger.LogDebug("Formatting HTI HTML Template.");
            string formattedHTML = _htiSettings.Template.Replace("[NAME]", cardName).Replace("[DESC]", cardDesc)
                .Replace("[RARITY]", cardRarity).Replace("[MAIN]", mainColor).Replace("[SUB]", subColor)
                .Replace("[RARITYCOLOR]", rarityColor).Replace("[IMAGEBASE64]", cardBase64).Replace("[FONT]", _htiSettings.Font);
            HTIBody hti = new HTIBody(872, 1272, formattedHTML);


            _logger.LogInformation("Sending request to external HTI service.");
            HttpResponseMessage response = await _httpClient.PostAsync(_htiSettings.URL, new StringContent(JsonSerializer.Serialize(hti), Encoding.UTF8, "application/json"));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                _logger.LogDebug("Converting HTI response to a byte array.");
                byte[] bytes = await response.Content.ReadAsByteArrayAsync();
                return bytes;
            }

            _logger.LogWarning("Could not generate HTI graphic from template: {StatusCode} - {Content}", response.StatusCode, response.Content);
            return null;
        }

        private string ColorForRarity(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.Legendary => _raritySettings.Legendary.Color,
                Rarity.Uncommon => _raritySettings.Uncommon.Color,
                Rarity.Rare => _raritySettings.Rare.Color,
                _ => _raritySettings.Common.Color,
            };
        }

        #pragma warning disable IDE1006 // Naming Styles
        private struct HTIBody
        {
            public int width { get; }
            public int height { get;}
            public string html { get; }

            public HTIBody(int width, int height, string html)
            {
                this.width = width;
                this.height = height;
                this.html = html;
            }
        }
        #pragma warning restore IDE1006 // Naming Styles
    }
}
