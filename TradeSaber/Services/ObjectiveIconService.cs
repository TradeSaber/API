using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TradeSaber.Models;

namespace TradeSaber.Services
{
    public class ObjectiveIconService
    {
        private readonly ILogger _logger;
        private readonly TradeContext _tradeContext;
        private readonly MascotService _mascotService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static readonly Dictionary<Objective.Type, Media> _objectiveMediaTypes = new Dictionary<Objective.Type, Media>();

        public ObjectiveIconService(ILogger<ObjectiveIconService> logger, TradeContext tradeContext, MascotService mascotService, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _tradeContext = tradeContext;
            _mascotService = mascotService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Media> Get(Objective.Type type)
        {
            if (!_objectiveMediaTypes.TryGetValue(type, out Media? media))
            {
                string saveFolder = _webHostEnvironment.WebRootPath;
                string fetchFolder = Path.Combine(_webHostEnvironment.ContentRootPath, "Icons");
                FileInfo file = new DirectoryInfo(fetchFolder).EnumerateFiles().First(f => f.Name.ToLower().StartsWith(type.ToString().ToLower()));

                string saveIconFolder = Path.Combine(saveFolder, "Icons");
                if (!Directory.Exists(saveIconFolder)) Directory.CreateDirectory(saveIconFolder);

                // Hash Construction
                Guid id = Guid.NewGuid();
                using SHA256 sha = SHA256.Create();
                _logger.LogInformation("Opening Icon Stream...");
                using Stream iconStream = file.OpenRead();
                _logger.LogInformation("Hashing and Serializing...");
                string fileHash = Controllers.MediaController.SerializeBytes(await sha.ComputeHashAsync(iconStream));
                var tempMedia = await _tradeContext.Media.FirstOrDefaultAsync(m => m.FileHash == fileHash);
                if (tempMedia is null)
                {
                    _logger.LogInformation("Generating Media...");
                    string savePath = Path.Combine("Icons", $"{id}{Path.GetExtension(file.Name)}");
                    using Stream sfile = File.Create(Path.Combine(saveFolder, savePath));
                    
                    iconStream.Position = 0;
                    await iconStream.CopyToAsync(sfile);
                    User mascot = await _mascotService.GetMascot();
                    media = new Media
                    {
                        ID = id,
                        Uploader = mascot,
                        FileHash = fileHash,
                        FileSize = iconStream.Length,
                        MimeType = MimeTypes.GetMimeType(file.Name),
                        Path = $"/{savePath.Replace("\\", "/").ToLower()}"
                    };
                    _tradeContext.Media.Add(media);
                    await _tradeContext.SaveChangesAsync();
                }
                else
                {
                    media = tempMedia;
                }
                _objectiveMediaTypes.TryAdd(type, media);
            }

            return media;
        }
    }
}
