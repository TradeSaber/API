using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TradeSaber.Models;
using TradeSaber.Settings;

namespace TradeSaber.Services
{
    public class HTIService
    {
        private readonly ILogger _logger;
        private readonly HTILoader _htiLoader;
        private readonly HttpClient _httpClient;
        private readonly HTISettings _htiSettings;
        private readonly TradeContext _tradeContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HTIService(ILogger<HTIService> logger, HTILoader htiLoader, HttpClient httpClient, HTISettings htiSettings, TradeContext tradeContext, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _htiLoader = htiLoader;
            _httpClient = httpClient;
            _htiSettings = htiSettings;
            _tradeContext = tradeContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Media?> Generate(string name, string desc, Rarity rarity, Series series, Media baseImage, User uploader)
        {
            string cardRarity = rarity.Name;
            string rarityColor = rarity.Color;

            _logger.LogInformation("Looking for base image file...");
            IFileInfo file = _webHostEnvironment.WebRootFileProvider.GetFileInfo(baseImage.Path);
            if (!file.Exists)
            {
                _logger.LogError("Could not find base image on local disk.");
                return null;
            }

            _logger.LogInformation("Deserializing base image.");
            using Stream baseStream = file.CreateReadStream();
            using MemoryStream baseMemory = new MemoryStream();
            await baseStream.CopyToAsync(baseMemory);

            string extension = baseImage.MimeType.Split('/').Last();
            _logger.LogInformation("Creating Base64 string for base image.");
            string cardBase64 = $"data:image/{extension};base64,{Convert.ToBase64String(baseMemory.ToArray())}";

            _logger.LogInformation("Getting HTI Template Data");
            (string htmlTemplate, string fontTemplate) = await _htiLoader.GetHTITemplate();
            
            _logger.LogDebug("Formatting HTI HTML Template.");
            string formattedHTML = htmlTemplate
                .Replace("[NAME]", name)
                .Replace("[DESC]", desc)
                .Replace("[RARITY]", cardRarity)
                .Replace("[MAIN]", series.Theme.Main)
                .Replace("[SUB]", series.Theme.Highlight ?? "white")
                .Replace("[RARITYCOLOR]", rarityColor)
                .Replace("[IMAGEBASE64]", cardBase64)
                .Replace("[FONT]", fontTemplate);

            HTIBody hti = new HTIBody(872, 1272, formattedHTML);

            _logger.LogInformation("Sending request to external HTI service.");
            HttpResponseMessage response = await _httpClient.PostAsync(_htiSettings.URL, new StringContent(JsonSerializer.Serialize(hti), Encoding.UTF8, "application/json"));
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                _logger.LogWarning("Could not generate HTI graphic from template: {StatusCode} - {Content}", response.StatusCode, response.Content);
                return null;
            }

            using Stream cardStream = await response.Content.ReadAsStreamAsync();

            if (cardStream.Length == 0)
            {
                _logger.LogWarning("Invalid generated card.");
                return null;
            }

            string saveFolder = _webHostEnvironment.WebRootPath;
            string saveMediaFolder = Path.Combine(saveFolder, nameof(Media));
            if (!Directory.Exists(saveMediaFolder)) Directory.CreateDirectory(saveMediaFolder);

            Guid id = Guid.NewGuid();
            using SHA256 sha = SHA256.Create();
            byte[] fileHash = await sha.ComputeHashAsync(cardStream);

            cardStream.Position = 0;
            string savePath = Path.Combine(nameof(Media), $"{id}.{extension}");
            using Stream sfile = File.Create(Path.Combine(saveFolder, savePath));
            await cardStream.CopyToAsync(sfile);

            Guid mid = Guid.NewGuid();
            Media media = new Media
            {
                ID = mid,
                Uploader = uploader,
                MimeType = baseImage.MimeType,
                FileSize = file.Length,
                FileHash = Controllers.MediaController.SerializeBytes(fileHash),
                Path = $"/{savePath.Replace("\\", "/").ToLower()}",
            };
            _tradeContext.Media.Add(media);
            await _tradeContext.SaveChangesAsync();

            return media;
        }

        public class HTIBody
        {
            [JsonPropertyName("width")]
            public int Width { get; }

            [JsonPropertyName("height")]
            public int Height { get; }

            [JsonPropertyName("html")]
            public string HTML { get; }

            public HTIBody(int width, int height, string html)
            {
                Width = width;
                Height = height;
                HTML = html;
            }
        }
    }
}
