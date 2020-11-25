﻿using System;
using NodaTime;
using System.Collections.Generic;

namespace TradeSaber.Models
{
    public class Transaction
    {
        public Guid ID { get; set; }
        public float? Tir { get; set; }
        public Instant Time { get; set; }
        public TStatus Status { get; set; }
        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;
        public IList<Card> Cards { get; set; } = null!;
        public IList<Pack> Packs { get; set; } = null!;

        public enum TStatus
        {
            Void,
            Pending,
            Affirmed
        }
    }
}