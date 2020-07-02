using System;
using System.IO;
using MongoDB.Bson;
using System.Drawing;
using TradeSaber.Models;
using TradeSaber.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

namespace TradeSaber.Controllers
{
    [Route("cards")]
    [ApiController]
    public class CardController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly CardGenerator _cardGenerator;
        private readonly CardDispatcher _cardDispatcher;

        public CardController(UserService user, CardGenerator cardGenerator, CardDispatcher cardDispatcher)
        {
            _userService = user;
            _cardGenerator = cardGenerator;
            _cardDispatcher = cardDispatcher;
        }

        /*[HttpGet("test/updateimages")]
        public IActionResult UpdateImageTest()
        {
            Progress<float> progress = new Progress<float>();
            progress.ProgressChanged += (object sender, float pro) => 
            {
                Console.WriteLine("Progress: " + pro.ToString("P"));
            };
            _cardGenerator.UpdateAllCardHTML(progress, Startup.APPPATH);
            return Ok(Startup.APPPATH);
        }*/

        [HttpGet("{id}")]
        public IActionResult GetCard(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return NotFound();

            Card card = _cardDispatcher.GetCard(id);
            if (card == null)
            {
                return NotFound();
            }
            return Ok(card);
        }

        [HttpGet("series/{id}")]
        public IActionResult GetCardsFromSeries(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return NotFound();

            return Ok(_cardDispatcher.GetCardsFromSeries(id));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateCard([FromForm] UploadCard body)
        {
            User user = _userService.UserFromContext(HttpContext);
            if (!user.Role.HasFlag(TradeSaberRole.Admin))
                return Unauthorized();

            if (body.Cover == null)
                return BadRequest();

            long size = body.Cover.Length;

            if (size > 15000000 || size == 0)
                return BadRequest();

            Card card = new Card
            {
                Series = body.Series,
                Name = body.Name,
                Description = body.Description,
                Master = body.Master,
                MaxPrints = body.MaxPrints,
                Rarity = body.Rarity,
                Locked = body.Locked,
                BaseProbability = body.BaseProbability
            };
            _cardDispatcher.Create(card);

            if (!ObjectId.TryParse(card.Series, out _))
                return NotFound();

            Series series = _cardDispatcher.GetSeries(card.Series);
            if (series == null)
            {
                return NotFound();
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "Images");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            try
            {
                var extension = Path.GetExtension(body.Cover.FileName);
                string newPath = path + "/" + card.Id + "_base" + extension;
                using StreamReader sr = new StreamReader(body.Cover.OpenReadStream());
                using Stream stream = System.IO.File.Create(newPath);
                await body.Cover.CopyToAsync(stream);
                card.BaseURL = "/images/" + card.Id + "_base" + extension;
                await stream.DisposeAsync();
                stream.Close();
                Image image = Image.FromFile(newPath);
                using MemoryStream ms = new MemoryStream();
                image.Save(ms, image.RawFormat);
                byte[] imageBytes = ms.ToArray();
                await ms.DisposeAsync();
                string b64 = $"data:image/{extension.ToLower().Replace(".", "")};base64," + Convert.ToBase64String(imageBytes);
                string newCoverPath = path + "/" + card.Id + "_cover.png";
                var bytes = await _cardGenerator.Generate(b64, card, series, newCoverPath);
                if (bytes.Length == 0) return Problem();
                await System.IO.File.WriteAllBytesAsync(newCoverPath, bytes);
                card.CoverURL = "/images/" + card.Id + "_cover.png";
                _cardDispatcher.Update(card);
                return Ok(card);
            }
            catch
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPatch("{id}")]
        public IActionResult UpdateCard(string id, [FromBody] JsonPatchDocument<Card> card)
        {
            if (!ObjectId.TryParse(id, out _))
                return NotFound();

            User user = _userService.UserFromContext(HttpContext);
            if (!user.Role.HasFlag(TradeSaberRole.Admin))
                return Unauthorized();
            Card c = _cardDispatcher.GetCard(id);
            if (c != null)
            {
                card.ApplyTo(c);
                _cardDispatcher.Update(c);
                return Ok(c);
            }
            return NotFound();
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteCard(string id)
        {
            if (!ObjectId.TryParse(id, out _))
                return NotFound();

            User user = _userService.UserFromContext(HttpContext);
            if (!user.Role.HasFlag(TradeSaberRole.Admin))
                return Unauthorized();
            Card card = _cardDispatcher.GetCard(id);
            if (card != null)
            {
                Pack[] guarantees = _cardDispatcher.PacksContainingGuaranteedCard(card.Id);
                Pack[] locked = _cardDispatcher.PacksContainingLockedCard(card.Id);
                for (int i = 0; i < guarantees.Length; i++)
                {
                    Pack pack = guarantees[i];
                    pack.GuaranteedCards.RemoveAll(x => x == card.Id);
                    _cardDispatcher.Update(pack);
                }
                for (int i = 0; i < locked.Length; i++)
                {
                    Pack pack = locked[i];
                    pack.LockedCardPool.RemoveAll(x => x.Id == card.Id);
                    _cardDispatcher.Update(pack);
                }
            }
            return NoContent();
        }

        public class CardUpload
        {
            public Card Card { get; set; }
            public IFormFile Cover { get; set; }
        }
    }
}