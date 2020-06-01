using System.Linq;
using MongoDB.Driver;
using TradeSaber.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TradeSaber.Models.Settings;
using System.Collections.Generic;

namespace TradeSaber.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UserCollection);
        }

        public User[] UsersWithPack(string packId)
            => _users.Find(user => user.UnopenedPacks.Contains(packId)).ToEnumerable().ToArray();

        public User[] UsersWithCard(string cardId)
            => _users.Find(user => user.Inventory.Contains(cardId)).ToEnumerable().ToArray();

        public User Get(string discordID)
            => _users.Find(user => user.DiscordID == discordID).FirstOrDefault();

        public User Create(User user)
        {
            _users.InsertOne(user);
            return user;
        }

        public void Update(string discordID, User user)
            => _users.ReplaceOne(u => u.DiscordID == discordID, user);

        public void Update(User user)
            => Update(user.DiscordID, user);

        public int ActiveCardCount(string cardId)
            => (int)_users.Find(u => u.Inventory.Any(c => c == cardId)).CountDocuments();
        

        public User UserFromContext(HttpContext context)
        {
            var identity = context.User.Identity as ClaimsIdentity;
            IList<Claim> claim = identity.Claims.ToList();
            var discordID = claim.Where(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault();
            return Get(discordID.Value);
        }
    }
}