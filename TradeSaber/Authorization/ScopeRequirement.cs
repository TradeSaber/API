using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TradeSaber.Authorization
{
    public class ScopeRequirement : IAuthorizationRequirement
    {
        public string Issuer { get; }
        public string Scope { get; }

        public ScopeRequirement(string issuer, string scope)
        {
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }
    }
}
