using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Web;
using SixLabors.ImageSharp.Web.Commands;
using SixLabors.ImageSharp.Web.Processors;
using System.Collections.Generic;
using System.Globalization;

namespace TradeSaber.Processors
{
    public class SimpleReizeWebProcessor : IImageWebProcessor
    {
        public const string Width = "width";
        public const string Height = "height";

        private static readonly IEnumerable<string> ResizeCommands = new[] { Width, Height };

        public IEnumerable<string> Commands => ResizeCommands;

        public FormattedImage Process(FormattedImage image, ILogger logger, IDictionary<string, string> commands, CommandParser parser, CultureInfo culture)
        {
            Size size = ParseSize(commands, parser, culture);

            if (size.Width == 0 || size.Height == 0 || size.Width > 2048 || size.Height > 2048)
            {
                return image;
            }
            image.Image.Mutate(x => x.Resize(size));
            return image;
        }

        private static Size ParseSize(
            IDictionary<string, string> commands,
            CommandParser parser,
            CultureInfo culture)
        {
            uint width = parser.ParseValue<uint>(commands.GetValueOrDefault(Width), culture);
            uint height = parser.ParseValue<uint>(commands.GetValueOrDefault(Height), culture);

            return new Size((int)width, (int)height);
        }
    }
}