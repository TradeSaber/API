using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeSaber.Models;
using TradeSaber.Models.Settings;

namespace TradeSaber.Services
{
    public class CardDispatcher
    {
        public static Random random = new Random();

        private readonly IMongoCollection<Card> _cards;
        private readonly IMongoCollection<Pack> _packs;
        private readonly IMongoCollection<Series> _series;

        private readonly UserService _userService;

        public CardDispatcher(IDatabaseSettings settings, UserService user)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _cards = database.GetCollection<Card>(settings.CardCollection);
            _packs = database.GetCollection<Pack>(settings.PackCollection);
            _series = database.GetCollection<Series>(settings.SeriesCollection);

            _userService = user;
        }

        public Card GetCard(string id)
            => _cards.Find(card => card.Id == id).FirstOrDefault();

        public Card[] GetCardsFromSeries(string seriesId)
            => _cards.Find(c => c.Series == seriesId).ToEnumerable().ToArray();

        public void Create(Card card)
            => _cards.InsertOne(card);

        public void Update(string id, Card card)
            => _cards.ReplaceOne(c => c.Id == id, card);

        public void Update(Card card)
            => Update(card.Id, card);

        public void Delete(Card card)
            => _cards.DeleteOne(c => c.Id == card.Id);

        public Pack GetPack(string id)
            => _packs.Find(pack => pack.Id == id).FirstOrDefault();

        public Pack[] PacksContainingGuaranteedCard(string cardId)
            => _packs.Find(p => p.GuaranteedCards.Contains(cardId)).ToEnumerable().ToArray();

        public Pack[] PacksContainingLockedCard(string cardId)
            => _packs.Find(p => p.LockedCardPool.Any(pd => pd.Id == cardId)).ToEnumerable().ToArray();

        public Pack[] GetAllPacks()
            => _packs.Find(p => true).ToEnumerable().ToArray();

        public void Create(Pack pack)
            => _packs.InsertOne(pack);

        public void Update(string id, Pack pack)
            => _packs.ReplaceOne(p => p.Id == id, pack);

        public void Update(Pack pack)
            => Update(pack.Id, pack);

        public void Delete(Pack pack)
            => _packs.DeleteOne(p => p.Id == pack.Id);

        public Series GetSeries(string id)
            => _series.Find(series => series.Id == id).FirstOrDefault();

        public Series[] GetAllSeries()
            => _series.Find(series => true).ToEnumerable().ToArray();

        public void Create(Series series)
            => _series.InsertOne(series);

        public void Update(string id, Series series)
            => _series.ReplaceOne(s => s.Id == id, series);

        public void Update(Series series)
            => Update(series.Id, series);

        public void Delete(Series series)
            => _series.DeleteOne(s => s.Id == series.Id);

        /// <summary>
        /// Rolls a random a deck based on a pack's parameters
        /// </summary>
        /// <param name="pack">The pack to get the roll settings from.</param>
        /// <returns></returns>
        public Card[] RollFromPack(Pack pack)
        {
            List<Card> cards = new List<Card>();
            
            foreach (string cardId in pack.GuaranteedCards)
            {
                Card c = GetCard(cardId);
                cards.Add(c);
            }

            foreach (Rarity rarity in pack.GuaranteedRarities)
            {
                Card c = RollRandom(rarity);
                cards.Add(c);
            }

            List<Card> cardColl = _cards.Find(c => c.Locked == false).ToList();
            
            foreach (var locked in pack.LockedCardPool)
            {
                Card lockedCard = cardColl.FirstOrDefault(c => c.Id == locked.Id);
                if (lockedCard == null)
                {
                    Card c = GetCard(locked.Id);
                    if (c != null)
                    {
                        c.BaseProbability += locked.ProbabilityBoost;
                        cardColl.Add(c);
                    }
                }
                else
                {
                    lockedCard.BaseProbability += locked.ProbabilityBoost;
                }
            }

            Card[] cardPool = cardColl.ToArray();
            while (pack.Count > cards.Count())
            {
                cards.Add(RollRandom(cardPool));
            }

            return cards.ToArray();
        }

        /// <summary>
        /// Rolls a completely random deck from the generic card pool.
        /// </summary>
        /// <param name="count">The number of random cards to return.</param>
        /// <returns></returns>
        public Card[] RollDeckRandom(int count)
        {
            Card[] cards = new Card[count];
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = RollRandom();
            }
            return cards;
        }

        /// <summary>
        /// Rolls a random card from the generic card pool.
        /// </summary>
        /// <returns></returns>
        public Card RollRandom()
        {
            Card[] cards = _cards.Find(c => c.Locked == false).ToEnumerable().ToArray();
            return RollRandom(cards);
        }

        /// <summary>
        /// Rolls a random card from a rarity pool.
        /// </summary>
        /// <param name="rarity"></param>
        /// <returns></returns>
        public Card RollRandom(Rarity rarity)
        {
            Card[] cards = _cards.Find(c => c.Locked == false && c.Rarity == rarity).ToEnumerable().ToArray();
            return RollRandom(cards);
        }

        /// <summary>
        /// Rolls a random card from a pool of cards.
        /// </summary>
        /// <param name="cards"></param>
        /// <returns></returns>
        public Card RollRandom(Card[] cards)
        {
            List<string> cardLottery = new List<string>();
            int totalCardCount = cards.Length;
            foreach (Card card in cards)
            {
                double representations = card.BaseProbability * totalCardCount;
                for (int i = 0; i < representations; i++)
                {
                    cardLottery.Add(card.Id);
                }
            }
            int selected = random.Next(0, cardLottery.Count);

            Card selectedCard = cards.First(c => c.Id == cardLottery[selected]);
            if (CanPrintCard(selectedCard))
            {
                return selectedCard;
            }
            return RollRandom();
        }


        public Card[] ServeNaturalDeck(int legendaryCount = 0, int rareCount = 0, int uncommonCount = 0, int commonCount = 0)
        {
            List<Card> cards = new List<Card>();
            for (int i = 0; i < legendaryCount; i++)
            {
                cards.Add(RollRandom(Rarity.Legendary));
            }
            for (int i = 0; i < rareCount; i++)
            {
                cards.Add(RollRandom(Rarity.Rare));
            }
            for (int i = 0; i < uncommonCount; i++)
            {
                cards.Add(RollRandom(Rarity.Uncommon));
            }
            for (int i = 0; i < commonCount; i++)
            {
                cards.Add(RollRandom(Rarity.Common));
            }
            return cards.ToArray();
        }

        /// <summary>
        /// Checks to see if a card can be printed.
        /// </summary>
        /// <param name="card">The card to check.</param>
        /// <returns></returns>
        private bool CanPrintCard(Card card)
        {
            if (card.MaxPrints == -1)
                return true;
            int count = _userService.ActiveCardCount(card.Id);
            return card.MaxPrints >= count;
        }
    }
}
