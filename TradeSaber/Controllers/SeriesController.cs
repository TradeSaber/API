using System;
using TradeSaber.Models;
using TradeSaber.Services;
using System.Threading.Tasks;
using TradeSaber.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TradeSaber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeriesController : ControllerBase
    {
        private readonly FileManager _fileManager;
        private readonly TradeContext _tradeContext;
        private readonly ILogger<SeriesController> _logger;

        public SeriesController(ILogger<SeriesController> logger, FileManager fileManager, TradeContext tradeContext)
        {
            _logger = logger;
            _fileManager = fileManager;
            _tradeContext = tradeContext;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        public async Task<ActionResult<Series>> GetSeries(Guid id)
        {
            Series? series = await _tradeContext.Series.FindAsync(id);
            if (series == null)
            {
                return NotFound();
            }
            return series;
        }

        [HttpPost]
        [Tauth(Role.Admin)]
        [RequestSizeLimit(15000000)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        public async Task<IActionResult> UploadSeries([FromForm] Upload upload)
        {
            User user = (HttpContext.Items["User"] as User)!;
            _logger.LogInformation("Series Creation Initiated by {Username}#{Discriminator}", user.Profile.Username, user.Profile.Discriminator);
            if (upload.Cover.Length == 0)
            {
                string error = "Invalid File Upload";
                return BadRequest(new { error });
            }

            Guid id = Guid.NewGuid();
            _logger.LogInformation("Processing Cover Image");
            string? path = await _fileManager.SaveImage(nameof(Series), id, upload.Cover.FileName, upload.Cover.OpenReadStream());
            if (path == null)
            {
                string error = "Error occurred when uploading cover image.";
                return BadRequest(new { error });
            }
            Series series = new Series
            {
                ID = id,
                Name = upload.Name,
                SubColor = upload.SubColor,
                MainColor = upload.MainColor,
                Description = upload.Description,
                CoverURL = $"/{(path.Replace("\\", "/").ToLower())}",
            };
            _logger.LogDebug("Adding new series {Name} to the database.", series.Name);
            _tradeContext.Series.Add(series);
            await _tradeContext.SaveChangesAsync();
            return Ok(series);
        }

        [Tauth(Role.Admin)]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteSeries(Guid id)
        {
            User user = (HttpContext.Items["User"] as User)!;
            _logger.LogInformation("Series Deletion Initiated by {Username}#{Discriminator}", user.Profile.Username, user.Profile.Discriminator);
            Series? series = await _tradeContext.Series.FindAsync(id);
            if (series is not null)
            {
                _logger.LogInformation("Deleting {Name}", series.Name);
                _tradeContext.Series.Remove(series);
                
                await _tradeContext.SaveChangesAsync();
                return NoContent();
            }
            return NotFound();
        }

        public class Upload
        {
            public string Name { get; set; } = null!;
            public string Description { get; set; } = null!;
            public string MainColor { get; set; } = null!;
            public string? SubColor { get; set; } = null;
            public IFormFile Cover { get; set; } = null!;
        }
    }
}