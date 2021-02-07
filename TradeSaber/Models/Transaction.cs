using System;
using System.Collections.Generic;

namespace TradeSaber.Models
{
    public class Transaction
    {
        public Guid ID { get; set; }
        public float? Tir { get; set; }
        public Status State { get; set; }
        public DateTime TimeSent { get; set; }
        public DateTime TimeActed { get; set; }
        public float? RequestedTir { get; set; }
        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;
        public IList<Card.TradeableReference> Cards { get; set; } = new List<Card.TradeableReference>();
        public IList<Pack.TradeableReference> Packs { get; set; } = new List<Pack.TradeableReference>();
        public IList<Card.TradeableRReference> RequestedCards { get; set; } = new List<Card.TradeableRReference>();
        public IList<Pack.TradeableRReference> RequestedPacks { get; set; } = new List<Pack.TradeableRReference>();

        public enum Status
        {
            Expired,
            Pending,
            Affirmed,
            Declined,
            Cancelled,
            Invalidated
        }
    }
}