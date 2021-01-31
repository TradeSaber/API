using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TradeSaber.Authorization;
using TradeSaber.Models;

namespace TradeSaber.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Scopes.UploadFile)]
    public class MediaController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IAuthService _authService;
        private readonly TradeContext _tradeContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MediaController(ILogger<MediaController> logger, IAuthService authService, TradeContext tradeContext, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _authService = authService;
            _tradeContext = tradeContext;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IAsyncEnumerable<Media> AllUploadedFiles()
        {
            return _tradeContext.Media.Include(m => m.Uploader).ThenInclude(u => u.Role).AsAsyncEnumerable();
        }

        [HttpPost("create")]
        public async Task<ActionResult<Media>> UploadFile(IFormFile file)
        {
            string saveFolder = _webHostEnvironment.WebRootPath;
            User user = (await _authService.GetUser(User.GetID()))!;
            string saveMediaFolder = Path.Combine(saveFolder, nameof(Media));
            if (!Directory.Exists(saveMediaFolder)) Directory.CreateDirectory(saveMediaFolder);

            if (file == null || file.Length == 0)
                return BadRequest(Error.Create("Missing or malformed file."));

            if (!MimeTypes.TryGetMimeType(file.FileName, out string? mimeType))
                return BadRequest(Error.Create("Invalid or Unknown MIME Type"));

            Guid id = Guid.NewGuid();
            using SHA256 sha = SHA256.Create();
            byte[] fileHash = await sha.ComputeHashAsync(file.OpenReadStream());

            string savePath = Path.Combine(nameof(Media), $"{id}{Path.GetExtension(file.FileName)}");
            using Stream sfile = System.IO.File.Create(Path.Combine(saveFolder, savePath));
            await file.CopyToAsync(sfile);

            Media media = new Media
            {
                ID = id,
                Uploader = user,
                MimeType = mimeType,
                FileSize = file.Length,
                FileHash = SerializeBytes(fileHash),
                Path = $"/{savePath.Replace("\\", "/").ToLower()}",
            };

            _tradeContext.Media.Add(media);
            await _tradeContext.SaveChangesAsync();
            media = await _tradeContext.Media.AsNoTracking().FirstOrDefaultAsync(m => m.ID == id);
            media.Uploader = null!;
            return Ok(media);
        }

        public static string SerializeBytes(byte[] value)
        {
            StringBuilder Sb = new StringBuilder();
            foreach (byte b in value)
                Sb.Append(b.ToString("x2"));
            return Sb.ToString();
        }
    }
}