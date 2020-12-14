using System;
using TradeSaber.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TradeSaber.Authorization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class TauthAttribute : Attribute, IAuthorizationFilter
    {
        public Role Role { get; set; } = Role.None;

        public TauthAttribute(Role role = Role.None) => Role = role;
        
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userItem = (User?)context.HttpContext.Items["User"];
            if (userItem is null || !userItem.Role.HasFlag(Role))
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}