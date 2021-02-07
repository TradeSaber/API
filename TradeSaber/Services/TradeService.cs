using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeSaber.Models;

namespace TradeSaber.Services
{
    public class TradeService
    {
        private readonly ILogger _logger;
        private readonly TradeContext _tradeContext;

        public TradeService(ILogger<TradeService> logger, TradeContext tradeContext)
        {
            _logger = logger;
            _tradeContext = tradeContext;
        }

        public async Task<Transaction?> RequestTrade(Packet from, Packet to)
        {
            if (!(InventoryCanMakeTrade(from.User.Inventory, from.Tir, from.Cards, from.Packs) && InventoryCanMakeTrade(to.User.Inventory, to.Tir, to.Cards, to.Packs)))
            {
                return null;
            }
            _logger.LogInformation($"Constructing trade request from {from.User.Profile.FormattedName()} to {to.User.Profile.FormattedName()}");
            Transaction transaction = new Transaction
            {
                Tir = from.Tir,
                Sender = from.User,
                Receiver = to.User,
                ID = Guid.NewGuid(),
                RequestedTir = to.Tir,
                TimeSent = DateTime.UtcNow,
                State = Transaction.Status.Pending
            };
            if (from.Cards is not null)
            {
                foreach (var card in from.Cards)
                {
                    transaction.Cards.Add(new Card.TradeableReference { Card = card });
                }
            }
            if (from.Packs is not null)
            {
                foreach (var pack in from.Packs)
                {
                    transaction.Packs.Add(new Pack.TradeableReference { Pack = pack });
                }
            }
            if (to.Cards is not null)
            {
                foreach (var card in to.Cards)
                {
                    transaction.RequestedCards.Add(new Card.TradeableRReference { Card = card });
                }
            }
            if (to.Packs is not null)
            {
                foreach (var pack in to.Packs)
                {
                    transaction.RequestedPacks.Add(new Pack.TradeableRReference { Pack = pack });
                }
            }
            _tradeContext.Transactions.Add(transaction);
            await _tradeContext.SaveChangesAsync();
            return transaction;
        }

        public async Task<bool> AcceptTrade(Transaction transaction)
        {
            if (transaction.State != Transaction.Status.Pending)
            {
                return false;
            }
            await CheckTransactionValidity(transaction);
            if (transaction.State != Transaction.Status.Pending)
            {
                return false;
            }
            transaction.State = Transaction.Status.Affirmed;
            transaction.TimeActed = DateTime.Now;
            if (transaction.Tir is not null)
            {
                transaction.Sender.Inventory.TirCoin -= transaction.Tir!.Value;
                transaction.Receiver.Inventory.TirCoin += transaction.Tir!.Value;
            }
            if (transaction.RequestedTir is not null)
            {
                transaction.Sender.Inventory.TirCoin += transaction.Tir!.Value;
                transaction.Receiver.Inventory.TirCoin -= transaction.Tir!.Value;
            }
            if (transaction.Cards is not null)
            {
                foreach (var card in transaction.Cards)
                {
                    var reference = transaction.Sender.Inventory.Cards.FirstOrDefault(c => c.ID == card.CardID);
                    if (reference is not null)
                    {
                        transaction.Sender.Inventory.Cards.Remove(reference);
                        transaction.Receiver.Inventory.Cards.Add(reference);
                    }
                }
            }
            if (transaction.Packs is not null)
            {
                foreach (var pack in transaction.Packs)
                {
                    var reference = transaction.Sender.Inventory.Packs.FirstOrDefault(p => p.ID == pack.PackID);
                    if (reference is not null)
                    {
                        transaction.Sender.Inventory.Packs.Remove(reference);
                        transaction.Receiver.Inventory.Packs.Add(reference);
                    }
                }
            }
            if (transaction.RequestedCards is not null)
            {
                foreach (var card in transaction.RequestedCards)
                {
                    var reference = transaction.Receiver.Inventory.Cards.FirstOrDefault(c => c.ID == card.CardID);
                    if (reference is not null)
                    {
                        transaction.Receiver.Inventory.Cards.Remove(reference);
                        transaction.Sender.Inventory.Cards.Add(reference);
                    }
                }
            }
            if (transaction.RequestedPacks is not null)
            {
                foreach (var pack in transaction.RequestedPacks)
                {
                    var reference = transaction.Receiver.Inventory.Packs.FirstOrDefault(p => p.ID == pack.PackID);
                    if (reference is not null)
                    {
                        transaction.Receiver.Inventory.Packs.Remove(reference);
                        transaction.Sender.Inventory.Packs.Add(reference);
                    }
                }
            }
            return true;
        }

        public async Task SetTradeState(Transaction transaction, Transaction.Status status)
        {
            if (transaction.State == Transaction.Status.Pending)
            {
                await CheckTransactionValidity(transaction, true);
                if (transaction.State == Transaction.Status.Pending)
                {
                    transaction.State = status;
                    transaction.TimeActed = DateTime.UtcNow;
                    await _tradeContext.SaveChangesAsync();
                }
            }
        }

        public async Task<Transaction.Status> CheckTransactionValidity(Transaction transaction, bool updateDatabase = true, bool checkTir = true)
        {
            _logger.LogInformation("Checking transaction validity on {ID}", transaction.ID);
            if (!(InventoryCanMakeTrade(transaction.Sender.Inventory, transaction.Tir, transaction.RequestedCards.Select(tr => tr.Card), transaction.Packs.Select(pr => pr.Pack), checkTir)
                && InventoryCanMakeTrade(transaction.Receiver.Inventory, transaction.RequestedTir, transaction.RequestedCards.Select(tr => tr.Card), transaction.RequestedPacks.Select(pr => pr.Pack), checkTir)))
            {
                if (updateDatabase)
                {
                    _logger.LogWarning("Invalid Trade Detected");
                    transaction.State = Transaction.Status.Invalidated;
                    transaction.TimeActed = DateTime.UtcNow;
                    await _tradeContext.SaveChangesAsync();
                }
                return Transaction.Status.Invalidated;
            }
            return transaction.State;
        }

        private static bool InventoryCanMakeTrade(Inventory inventory, float? tir, IEnumerable<Card>? cards, IEnumerable<Pack>? packs, bool checkTir = true)
        {
            if (checkTir && tir is not null)
            {
                if (tir > inventory.TirCoin)
                {
                    return false;
                }
            }
            if (cards is not null)
            {
                List<Card> cardDup = inventory.Cards.Select(c => c.Card).ToList();
                foreach (var card in cards)
                {
                    if (!cardDup.Any(cr => cr.ID == card.ID))
                    {
                        return false;
                    }
                    var cy = cardDup.First(c => c.ID == card.ID);
                    cardDup.Remove(cy);
                }
            }
            if (packs is not null)
            {
                List<Pack> packDup = inventory.Packs.Select(c => c.Pack).ToList();
                foreach (var pack in packs)
                {
                    if (!packDup.Any(pr => pr.ID == pack.ID))
                    {
                        return false;
                    }
                    var py = packDup.First(p => p.ID == pack.ID);
                    packDup.Remove(py);
                }
            }
            return true;
        }

        public class Packet
        {
            public User User { get; }
            public float? Tir { get; }
            public IEnumerable<Card>? Cards { get; }
            public IEnumerable<Pack>? Packs { get; }

            public Packet(User user, float? tir, IEnumerable<Card>? cards, IEnumerable<Pack>? packs)
            {
                Tir = tir;
                User = user;
                Cards = cards;
                Packs = packs;
            }
        }
    }
}