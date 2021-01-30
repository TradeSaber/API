using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TradeSaber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacksController : ControllerBase
    {
        private readonly TradeContext _tradeContext;
        private readonly ILogger<PacksController> _logger;

        public PacksController(ILogger<PacksController> logger, TradeContext tradeContext)
        {
            _logger = logger;
            _tradeContext = tradeContext;
        }


    }
}
