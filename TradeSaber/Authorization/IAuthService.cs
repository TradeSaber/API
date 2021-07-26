using System;

namespace TradeSaber.Authorization
{
    public interface IAuthService
    {
        string Sign(Guid id, float lengthInHours = 4, params string[] scopes);
    }
}