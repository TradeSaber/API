using System;
using System.Threading.Tasks;
using TradeSaber.Models;

namespace TradeSaber.Authorization
{
    public interface IAuthService
    {
        Task<User?> GetUser(Guid? id);
        string Sign(Guid id, float lengthInHours = 1344f, params string[] scopes);
    }
}