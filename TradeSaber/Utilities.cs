using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace TradeSaber
{
    public static class Utilities
    {
        private static readonly Dictionary<string, List<byte[]>> _fileSignatures = new Dictionary<string, List<byte[]>>
        {
            {
                ".jpeg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
                }
            },
            {
                ".jpg", new List<byte[]>
                {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
                }
            },
            {
                ".png", new List<byte[]>
                {
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
                }
            },
            {
                ".gif", new List<byte[]>
                {
                    new byte[] { 0x47, 0x49, 0x36, 0x38 }
                }
            }
        };

        public static async Task SaveIFormToFile(IFormFile file, string path)
        {
            using Stream stream = File.Create(path);
            await file.CopyToAsync(stream);
        }

        public static async Task<string> SaveImageToRoot(this IFormFile file, HashType type = HashType.SHA256)
        {
            var endPath = Path.Combine("Images");
            var frontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var fileName = $"{ComputeHash(file.OpenReadStream(), type)}.png";
            var savePath = Path.Combine(frontPath, endPath);
            var fullPath = Path.Combine(savePath, fileName);

            Directory.CreateDirectory(savePath);
            using Stream stream = File.Create(fullPath);
            await file.CopyToAsync(stream);

            return Path.Combine(endPath, fileName);
        }

        public static bool VerifyImageFileExtension(Stream stream, string extension)
        {
            if (!_fileSignatures.TryGetValue(extension, out List<byte[]>? signatures))
                return false;

            using BinaryReader reader = new BinaryReader(stream);
            var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
            return signatures.Any(sig => headerBytes.Take(sig.Length).SequenceEqual(sig));
        }

        public static string ComputeHash(this Stream stream, HashType type = HashType.SHA256)
        {
            if (type == HashType.SHA256)
            {
                using SHA256 sha256 = SHA256.Create();
                byte[] hash = sha256.ComputeHash(stream);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                return builder.ToString().ToLower();
            }
            if (type == HashType.MD5)
            {
                using MD5 md5 = MD5.Create();
                byte[] hash = md5.ComputeHash(stream);
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                return builder.ToString().ToLower();
            }
            return "empty";
        }

        public enum HashType
        {
            MD5,
            SHA256
        }
    }
}