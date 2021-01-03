using System;
using System.IO;
using System.Linq;
using TradeSaber.Models;
using TradeSaber.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TradeSaber.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TradeSaber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly HTIService _htiService;
        private readonly TradeContext _tradeContext;
        private readonly ILogger<CardsController> _logger;

        public CardsController(ILogger<CardsController> logger, HTIService htiService, TradeContext tradeContext)
        {
            _logger = logger;
            _htiService = htiService;
            _tradeContext = tradeContext;
        }

        [HttpPost]
        [Tauth(Role.Admin)]
        [RequestSizeLimit(15000000)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status413PayloadTooLarge)]
        public async Task<IActionResult> UploadCard([FromForm] Upload upload)
        {
            User user = (HttpContext.Items["User"] as User)!;
            _logger.LogInformation("Series Creation Initiated by {Username}#{Discriminator}", user.Profile.Username, user.Profile.Discriminator);
            if (upload.Base.Length == 0)
            {
                string error = "Invalid File Upload";
                return BadRequest(new { error });
            }

            Series? series = await _tradeContext.Series.FindAsync(upload.Series);
            if (series is null)
            {
                string error = "Series does not exist.";
                return BadRequest(new { error });
            }

            if (series.Cards.Any(c => c.Name.ToLower() == upload.Name.ToLower()))
            {
                string error = "Series already contains a card with this name.";
                return BadRequest(new { error });
            }

            _logger.LogInformation("Processing Base Image");
            Stream baseStream = upload.Base.OpenReadStream();
            string? basePath = await baseStream.SaveImageToRoot();
            if (basePath is null)
            {
                string error = "Could not save base image.";
                return BadRequest(new { error });
            }

            _logger.LogInformation("Processing Card Graphic");
            byte[]? cardStream = await _htiService.Generate(upload.Name, upload.Description, series.MainColor, series.SubColor ?? "#000000",
                                                            upload.Rarity, baseStream, Path.GetExtension(upload.Base.FileName));
            if (cardStream is null)
            {
                string error = "Could not generate card graphic via HTI.";
                return BadRequest(new { error });
            }
            await baseStream.DisposeAsync();

            using MemoryStream ms = new MemoryStream(cardStream);
            string? coverPath = await ms.SaveImageToRoot();
            if (coverPath is null)
            {
                string error = "Could not save generated card.";
                return BadRequest(new { error });
            }

            Card card = new Card
            {
                Series = series,
                Name = upload.Name,
                ID = Guid.NewGuid(),
                Value = upload.Value,
                Rarity = upload.Rarity,
                Locked = upload.Locked,
                Maximum = upload.Maximum,
                Description = upload.Description,
                Probability = upload.Probability,
                Root = upload.Root ?? upload.Name,
                BaseURL = $"/{basePath.Replace("\\", "/").ToLower()}",
                CoverURL = $"/{coverPath.Replace("\\", "/").ToLower()}",
            };

            _logger.LogDebug("Adding a new card {Name} to the database.", card.Name);
            _tradeContext.Cards.Add(card);
            await _tradeContext.SaveChangesAsync();
            return Ok(new { card.ID });
        }


        [Tauth(Role.Admin)]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteSeries(Guid id)
        {
            User user = (HttpContext.Items["User"] as User)!;
            _logger.LogInformation("Card Deletion Initiated by {Username}#{Discriminator}", user.Profile.Username, user.Profile.Discriminator);
            Card? card = await _tradeContext.Cards.FindAsync(id);
            if (card is not null)
            {
                _logger.LogInformation("Deleting {Name}", card.Name);
                _tradeContext.Cards.Remove(card);

                await _tradeContext.SaveChangesAsync();
                return NoContent();
            }
            return NotFound();
        }

        public class Upload
        {
            public Guid Series { get; set; }
            public string Name { get; set; } = null!;
            public string Description { get; set; } = null!;
            public float Probability { get; set; }
            public Rarity Rarity { get; set; }
            public string? Root { get; set; }
            public int? Maximum { get; set; }
            public bool Locked { get; set; }
            public IFormFile Base { get; set; } = null!;
            public float? Value { get; set; }
        }
    }
}