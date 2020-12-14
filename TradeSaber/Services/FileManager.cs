using System;
using System.IO;
using SixLabors.ImageSharp;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Processing;

namespace TradeSaber.Services
{
    public class FileManager
    {
        private const string PathURL = "Files";
        private const string ImageURL = "Images";

        public async Task<string?> SaveImage(string container, Guid id, string fileName, Stream stream)
        {
            string extension = Path.GetExtension(fileName);
            /*if (!Utilities.VerifyImageFileExtension(stream, extension))
            {
                return null;
            }*/
            var savePath = Path.Combine(PathURL, ImageURL, container);
            var saveLocation = Path.Combine(savePath, $"{id}{extension}");
            Directory.CreateDirectory(savePath);
            using Stream fileStream = File.Create(saveLocation);
            await stream.CopyToAsync(fileStream);
            return savePath;
        }

        public async Task<byte[]?> LoadImage(string container, Guid id, int? width, int? height)
        {
            var pngFolder = Path.Combine(PathURL, ImageURL, container);
            var pngPath = Path.Combine(pngFolder, $"{id}.png");
            if (!File.Exists(pngPath))
            {
                return null;
            }
            if (width.HasValue && height.HasValue)
            {
                var imageSizePath = $"{pngPath}?width={width}&size={height}";
                if (!File.Exists(imageSizePath))
                {
                    using Image image = await Image.LoadAsync(pngPath);
                    image.Mutate(x => x.Resize(width.Value, height.Value));
                    await image.SaveAsPngAsync(imageSizePath);

                    using MemoryStream ms = new MemoryStream();
                    await image.SaveAsPngAsync(ms);
                    return ms.ToArray();
                }
                else
                {
                    using Image image = await Image.LoadAsync(imageSizePath);
                    using MemoryStream ms = new MemoryStream();
                    await image.SaveAsPngAsync(ms);
                    return ms.ToArray();
                }
            }
            using Image mainImage = await Image.LoadAsync(pngPath);
            using MemoryStream mss = new MemoryStream();
            await mainImage.SaveAsPngAsync(mss);
            return mss.ToArray();
        }
    }
}