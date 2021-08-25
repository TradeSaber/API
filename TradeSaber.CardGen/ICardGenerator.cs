using System;
using System.IO;
using System.Threading.Tasks;

namespace TradeSaber.CardGen
{
    public interface ICardGenerator
    {
        Task<Stream> Create(string title, string description, string rarity);
    }
}