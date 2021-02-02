using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeSaber.Models;

namespace TradeSaber.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeriesController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly TradeContext _tradeContext;

        public SeriesController(ILogger<SeriesController> logger, TradeContext tradeContext)
        {
            _logger = logger;
            _tradeContext = tradeContext;
        }

        [HttpGet]
        public IAsyncEnumerable<Series> GetAllSeries()
        {
            return _tradeContext.Series.Include(s => s.Cards).Include(s => s.Icon).Include(s => s.Banner).AsAsyncEnumerable();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Series>> GetSeries(Guid id)
        {
            Series? series = await _tradeContext.Series.Include(s => s.Cards).Include(s => s.Icon).Include(s => s.Banner).FirstOrDefaultAsync(s => s.ID == id);
            if (series is null)
            {
                return NotFound(Error.Create("Series not found."));
            }
            return series;
        }

        [HttpPost]
        [Authorize(Scopes.CreateSeries)]
        public async Task<ActionResult<Series>> CreateSeries([FromBody] CreateSeriesBody body)
        {
            Series? series = await _tradeContext.Series.FirstOrDefaultAsync(s => s.Name.ToLower() == body.Name.ToLower());
            if (series is not null)
            {
                return NotFound(Error.Create("Series with name already exists."));
            }
            Media? iconMedia = await _tradeContext.Media.FindAsync(body.IconID);
            if (iconMedia is null)
            {
                return NotFound(Error.Create("Could not find icon media element."));
            }
            Media? bannerMedia = await _tradeContext.Media.FindAsync(body.BannerID);
            if (bannerMedia is null)
            {
                return NotFound(Error.Create("Could not find banner media element."));
            }
            _logger.LogInformation("Creating new series, {name}", body.Name);
            series = new Series
            {
                Name = body.Name,
                Icon = iconMedia,
                Theme = body.Theme,
                ID = Guid.NewGuid(),
                Banner = bannerMedia,
                Description = body.Description,
            };
            _tradeContext.Series.Add(series);
            await _tradeContext.SaveChangesAsync();
            return Ok(series);
        }

        public class CreateSeriesBody
        {
            public string Name { get; set; } = null!;
            public string Description { get; set; } = null!;
            public Guid IconID { get; set; }
            public Guid BannerID { get; set; }
            public ColorTheme Theme { get; set; } = null!;
        }
    }
}
