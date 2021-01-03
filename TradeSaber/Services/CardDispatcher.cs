using System;
using System.Linq;
using TradeSaber.Models;
using TradeSaber.Settings;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace TradeSaber.Services
{
    public class CardDispatcher
    {
        private readonly Random _random;
        private readonly TradeContext _tradeContext;
        private readonly RaritySettings _raritySettings;
        private readonly ILogger<CardDispatcher> _logger;

        public CardDispatcher(Random random, TradeContext tradeContext, RaritySettings raritySettings, ILogger<CardDispatcher> logger)
        {
            _logger = logger;
            _random = random;
            _tradeContext = tradeContext;
            _raritySettings = raritySettings;
        }

        /// <summary>
        /// Rolls a completely random card from the natural card pool.
        /// </summary>
        /// <returns></returns>
        public Card Roll(Rarity? rarity = null)
        {
            _logger.LogDebug("Rolling a random natural card");
            var cards = _tradeContext.Cards.Where(card => card.Locked == false);
            if (rarity.HasValue)
            {
                _logger.LogDebug("...with rarity {Rarity}.", rarity.Value);
                cards = cards.Where(card => card.Rarity == rarity.Value);
            }
            return RollFromPool(cards, RepresentationValueConstant(cards));
        }

        /// <summary>
        /// Rolls a card based on probability from a collection of cards.
        /// </summary>
        /// <param name="cards">The cards to use as the pool.</param>
        /// <param name="pulsey">The Pulsey value for this set of cards.</param>
        /// <param name="references">Any potential card references</param>
        /// <remarks>
        /// This can end up in an deadlocked recursive loop if every card in the inserted pool has a limited set of prints.
        /// The chance of this happening is so incredibly low, I'm not factoring in a fix for it.
        /// </remarks>
        /// <returns>The selected (probability-based) card.</returns>
        public Card RollFromPool(IEnumerable<Card> cards, int pulsey, IEnumerable<Card.Reference>? references = null)
        {
            // Calculate the representations for every card based on their probability.
            List<Card> lottery = new List<Card>();
            int maxCardCount = cards.Count();
            foreach (var card in cards)
            {
                double representations = ProbabilityForRarity(card.Rarity) * card.Probability * maxCardCount * pulsey;
                float? extraRef = references?.FirstOrDefault(refer => refer.Card == card)?.Boost;
                if (extraRef.HasValue)
                {
                    representations *= extraRef.Value;
                }
                for (int i = 0; i < representations; i++)
                {
                    lottery.Add(card);
                }
            }

            // Get our lucky card
            Card selected = lottery.ElementAt(_random.Next(0, lottery.Count));
            if (CanPrint(selected))
            {
                _logger.LogDebug("Printing {Name} ({ID})", selected.Name, selected.ID);
                return selected;
            }
            return RollFromPool(cards, pulsey, references);
        }

        public IEnumerable<Card> RollSet(Pack pack)
        {
            List<Card> set = new List<Card>();
            set.AddRange(pack.CardPool.Where(c => c.Guaranteed).Select(c => c.Card).AsEnumerable());
            pack.Rarities?.ToList().ForEach(rar => set.Add(Roll(rar)));

            var cards = _tradeContext.Cards.Where(card => card.Locked == false).ToList();
            cards.AddRange(pack.CardPool.Select(p => p.Card));
            int pulsey = RepresentationValueConstant(cards);
            while (pack.Count > set.Count)
            {
                set.Add(RollFromPool(cards, pulsey, pack.CardPool));
            }
            return set;
        }

        public IEnumerable<Card> RollSet(int legendaries = 0, int rares = 0, int uncommons = 0, int commons = 0)
        {
            IList<Card> cards = new List<Card>();
            for (int i = 0; i < legendaries; i++)
            {
                cards.Add(Roll(Rarity.Legendary));
            }
            for (int i = 0; i < rares; i++)
            {
                cards.Add(Roll(Rarity.Rare));
            }
            for (int i = 0; i < uncommons; i++)
            {
                cards.Add(Roll(Rarity.Uncommon));
            }
            for (int i = 0; i < commons; i++)
            {
                cards.Add(Roll(Rarity.Common));
            }
            return cards;
        }

        private float ProbabilityForRarity(Rarity rarity)
        {
            return rarity switch
            {
                Rarity.Legendary => _raritySettings.Legendary.Probability,
                Rarity.Uncommon => _raritySettings.Uncommon.Probability,
                Rarity.Rare => _raritySettings.Rare.Probability,
                _ => _raritySettings.Common.Probability,
            };
        }

        /// <summary>
        /// Checks to see if a card can be printed.
        /// </summary>
        /// <remarks>
        /// This can technically cause a singular pack to create more than the maximum number of a card since it doesn't
        /// factor in the other cards that are being generated. I'm leaving this in cause I think it'd be cool if an
        /// "impossible card" were to appear (although the chance is incredibly rare, it would make the card rare as well).
        /// </remarks>
        /// <param name="card">The card to check availability for.</param>
        /// <returns></returns>
        private bool CanPrint(Card card)
        {
            _logger.LogDebug("Checking printability for {Name} ({ID})", card.Name, card.ID);
            if (card.Maximum.HasValue)
            {
                var cardCount = _tradeContext.Users.SelectMany(u => u.Cards).Count();
                return card.Maximum.Value > cardCount;
            }
            return true;
        }

        /// <summary>
        /// Calculates what I'm calling the "Pulsey" number, the correct integer needed to multiply a set of
        /// float values to make them all integers without destroying their ratios.
        /// </summary>
        /// <param name="cards">The cards.</param>
        /// <param name="references">Card references for boost probability calculation.</param>
        /// <returns>The representation value.</returns>
        private int RepresentationValueConstant(IEnumerable<Card> cards, Card.Reference[]? references = null)
        {
            // Part 1 - Find every unique rarity in the card set.
            ISet<Rarity> uniqueRarities = new HashSet<Rarity>();
            var rarities = cards.Select(c => c.Rarity);
            for (int i = 0; i < rarities.Count(); i++)
            {
                uniqueRarities.Add(rarities.ElementAt(i));
            }

            // Part 2 - Get every single rarity probability
            IDictionary<Rarity, float> rarityTable = new Dictionary<Rarity, float>();
            foreach (var rarity in uniqueRarities)
            {
                rarityTable.Add(rarity, ProbabilityForRarity(rarity));
            }

            // Part 3 - Calculate the probability for every card
            IList<float> probabilityValues = new List<float>();
            foreach (var card in cards)
            {
                float probability = card.Probability * rarityTable[card.Rarity];
                float? boost = references?.FirstOrDefault(refer => refer.Card == card)?.Boost;
                if (boost.HasValue)
                {
                    probability *= boost.Value;
                }
                probabilityValues.Add(probability);
            }

            // Part 4 - Get the highest place count and return
            return MaximumPlaceCount(probabilityValues.AsEnumerable());
        }

        /// <summary>
        /// Gets the highest number of decimal places from a set of numbers.
        /// </summary>
        /// <param name="values">The float numbers.</param>
        /// <returns>The highest place count in the numbers.</returns>
        private static int MaximumPlaceCount(IEnumerable<float> values)
        {
            int highest = 0;
            foreach (var val in values)
            {
                var placeCount = Places(val);
                if (placeCount > highest)
                {
                    highest = placeCount;
                }
            }
            return highest;
        }

        /// <summary>
        /// Counts the amount of decimal places in a float.
        /// </summary>
        /// <param name="value">The float to count.</param>
        /// <returns>The amount of places in the float.</returns>
        private static int Places(float value)
        {
            return ((int)(value - Math.Truncate(value))).ToString().Length;
        }
    }
}