using System;
using TradeSaber.Models;

namespace TradeSaber.Authorization
{
    public interface IAuthService
    {
        User? GetUser(Guid? id);
        string Sign(Guid id, float lengthInHours = 1344f, params string[] scopes);
    }
}