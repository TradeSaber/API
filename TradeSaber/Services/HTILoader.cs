using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace TradeSaber.Services
{
    public class HTILoader
    {
        private readonly ILogger _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private string? _cachedHTMLTemplate;
        private string? _cachedFontData;

        public HTILoader(ILogger<HTILoader> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<(string, string)> GetHTITemplate()
        {
            if (_cachedHTMLTemplate is null)
            {
                IFileInfo cardFile = _webHostEnvironment.ContentRootFileProvider.GetFileInfo("HTI\\card.tsct");
                IFileInfo fontFile = _webHostEnvironment.ContentRootFileProvider.GetFileInfo("HTI\\font.tsft");

                _logger.LogInformation("Deserialzing Card Image Template");
                if (!cardFile.Exists)
                    throw new FileNotFoundException("Could not find card template.");
                if (!fontFile.Exists)
                    throw new FileNotFoundException("Could not find font template.");

                using Stream cardStream = cardFile.CreateReadStream();
                using StreamReader cardSR = new StreamReader(cardStream);
                _cachedHTMLTemplate = await cardSR.ReadToEndAsync();

                using Stream fontStream = fontFile.CreateReadStream();
                using StreamReader fontSR = new StreamReader(fontStream);
                _cachedFontData = await fontSR.ReadToEndAsync();
            }
            return (_cachedHTMLTemplate ?? "", _cachedFontData ?? "");
        }
    }
}
