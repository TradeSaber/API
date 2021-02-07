using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeSaber.Authorization;
using TradeSaber.Models;
using TradeSaber.Services;

namespace TradeSaber.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]s")]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IAuthService _authService;
        private readonly TradeContext _tradeContext;
        private readonly TradeService _tradeService;

        public TransactionController(ILogger<TransactionController> logger, IAuthService authService, TradeContext tradeContext, TradeService tradeService)
        {
            _logger = logger;
            _authService = authService;
            _tradeContext = tradeContext;
            _tradeService = tradeService;
        }

        [HttpGet("validate/{id}")]
        public async Task<ActionResult<StatusBody>> ValidateTransaction(Guid id)
        {
            Transaction? transaction = await TransactionQuery().FirstOrDefaultAsync(t => t.ID == id);
            if (transaction is null)
            {
                return NotFound(Error.Create("Transaction does not exist."));
            }
            return Ok(new StatusBody { State = await _tradeService.CheckTransactionValidity(transaction, false) });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(Guid id)
        {
            Transaction? transaction = await TransactionQuery().FirstOrDefaultAsync(t => t.ID == id);
            if (transaction is null)
            {
                return NotFound(Error.Create("Transaction does not exist."));
            }
            return Ok(transaction);
        }

        [HttpGet("@me")]
        public async Task<IAsyncEnumerable<Transaction>> GetSelfTransactions()
        {
            User user = (await _authService.GetUser(User.GetID()))!;
            return TransactionQuery().Where(t => t.Sender.ID == user.ID).AsAsyncEnumerable();
        }

        [HttpPost("accept/{id}")]
        public async Task<ActionResult<Transaction>> AcceptTransaction(Guid id)
        {
            Transaction? transaction = await TransactionQuery().FirstOrDefaultAsync(t => t.ID == id);
            if (transaction is null)
            {
                return NotFound(Error.Create("Transaction does not exist."));
            }
            User user = (await _authService.GetUser(User.GetID()))!;
            if (transaction.Receiver.ID != user.ID)
            {
                return Unauthorized(Error.Create("You cannot accept this transaction."));
            }
            _logger.LogInformation($"{user.Profile.FormattedName()} is accepting transaction ({transaction.ID}).");
            await _tradeService.AcceptTrade(transaction);
            return Ok(transaction);
        }

        [HttpPost("cancel/{id}")]
        public async Task<ActionResult<Transaction>> CancelTransaction(Guid id)
        {
            Transaction? transaction = await TransactionQuery().FirstOrDefaultAsync(t => t.ID == id);
            if (transaction is null)
            {
                return NotFound(Error.Create("Transaction does not exist."));
            }
            User user = (await _authService.GetUser(User.GetID()))!;
            if (transaction.Sender.ID != user.ID)
            {
                return Unauthorized(Error.Create("You cannot cancel this transaction."));
            }
            _logger.LogInformation($"{user.Profile.FormattedName()} is cancelling transaction ({transaction.ID}).");
            await _tradeService.SetTradeState(transaction, Transaction.Status.Cancelled);
            return Ok(transaction);
        }

        [HttpPost("decline/{id}")]
        public async Task<ActionResult<Transaction>> DeclineTransaction(Guid id)
        {
            Transaction? transaction = await TransactionQuery().FirstOrDefaultAsync(t => t.ID == id);
            if (transaction is null)
            {
                return NotFound(Error.Create("Transaction does not exist."));
            }
            User user = (await _authService.GetUser(User.GetID()))!;
            if (transaction.Receiver.ID != user.ID)
            {
                return Unauthorized(Error.Create("You cannot decline this transaction."));
            }
            _logger.LogInformation($"{user.Profile.FormattedName()} is declining transaction ({transaction.ID}).");
            await _tradeService.SetTradeState(transaction, Transaction.Status.Declined);
            return Ok(transaction);
        }

        [HttpPost]
        public async Task<ActionResult<Transaction>> CreateTradeRequest([FromBody] CreateTradeRequestBody body)
        {
            if (body.Tir is null && body.RequestedTir is null &&
                ArrayIsNullOrEmpty(body.Cards) && ArrayIsNullOrEmpty(body.RequestedCards) &&
                ArrayIsNullOrEmpty(body.Packs) && ArrayIsNullOrEmpty(body.RequestedPacks))
            {
                return BadRequest(Error.Create("Invalid Trade Request. Empty."));
            }
            if ((body.Tir is not null && body.Tir.Value <= 0) || (body.RequestedTir is not null && body.RequestedTir.Value <= 0))
            {
                return BadRequest(Error.Create("Tir cannot be at or below zero."));
            }

            User sender = (await _authService.GetUser(User.GetID()))!;
            User? receiver = await _authService.GetUser(body.Receiver);
            if (receiver is null)
            {
                return BadRequest(Error.Create("User does not exist."));
            }
            if (receiver.ID == sender.ID)
            {
                return BadRequest(Error.Create("You cannot trade with yourself."));
            }
            List<Card> myCards = new List<Card>();
            List<Pack> myPacks = new List<Pack>();
            List<Card> theirCards = new List<Card>();
            List<Pack> thierPacks = new List<Pack>();
            if (body.Cards is not null)
            {
                foreach (var cardID in body.Cards)
                {
                    Card? card = await _tradeContext.Cards.Include(c => c.Cover).Include(c => c.Base).FirstOrDefaultAsync(c => c.ID == cardID);
                    if (card is not null)
                    {
                        myCards.Add(card);
                    }
                }
            }
            if (body.Packs is not null)
            {
                foreach (var packID in body.Packs)
                {
                    Pack? pack = await _tradeContext.Packs.Include(p => p.Cover).FirstOrDefaultAsync(p => p.ID == packID);
                    if (pack is not null)
                    {
                        myPacks.Add(pack);
                    }
                }
            }
            if (body.RequestedCards is not null)
            {
                foreach (var cardID in body.RequestedCards)
                {
                    Card? card = await _tradeContext.Cards.Include(c => c.Cover).Include(c => c.Base).FirstOrDefaultAsync(c => c.ID == cardID);
                    if (card is not null)
                    {
                        theirCards.Add(card);
                    }
                }
            }
            if (body.RequestedPacks is not null)
            {
                foreach (var packID in body.RequestedPacks)
                {
                    Pack? pack = await _tradeContext.Packs.Include(p => p.Cover).FirstOrDefaultAsync(p => p.ID == packID);
                    if (pack is not null)
                    {
                        thierPacks.Add(pack);
                    }
                }
            }
            Transaction? transaction = await _tradeService.RequestTrade(
                new TradeService.Packet(sender, body.Tir, myCards.Count == 0 ? null : myCards, myPacks.Count == 0 ? null : myPacks),
                new TradeService.Packet(receiver, body.RequestedTir, theirCards.Count == 0 ? null : theirCards, thierPacks.Count == 0 ? null : thierPacks
            ));
            if (transaction is null)
            {
                _logger.LogWarning($"PANIC! Transaction did not go through. ({sender.Profile.FormattedName()}) ({sender.Profile.FormattedName()})");
                throw new Exception();
            }
            return Ok(transaction);
        }

        private IQueryable<Transaction> TransactionQuery()
        {
            return _tradeContext.Transactions
                .Include(t => t.Cards)
                .Include(t => t.Packs)
                .Include(t => t.RequestedCards)
                .Include(t => t.RequestedPacks)
                .Include(t => t.Sender).ThenInclude(u => u.Inventory)
                .Include(t => t.Sender).ThenInclude(u => u.Role)
                .Include(t => t.Receiver).ThenInclude(u => u.Inventory)
                .Include(t => t.Receiver).ThenInclude(u => u.Role);
        }

        private static bool ArrayIsNullOrEmpty<T>(T[]? array)
        {
            return array is null || array.Length == 0;
        }

        public class StatusBody
        {
            public Transaction.Status State { get; set; }
        }

        public class CreateTradeRequestBody
        {
            public float? Tir { get; set; }
            public float? RequestedTir { get; set; }
            public Guid Receiver { get; set; }
            public Guid[]? Cards { get; set; }
            public Guid[]? Packs { get; set; }
            public Guid[]? RequestedCards { get; set; }
            public Guid[]? RequestedPacks { get; set; }
        }
    }
}