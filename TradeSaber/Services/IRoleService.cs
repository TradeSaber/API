using System.Threading.Tasks;
using TradeSaber.Models;

namespace TradeSaber.Services
{
    public interface IRoleService
    {
        Task<Role> GetRootRole();
        Task<Role?> CreateRole(string name, string[] scopes, bool root = false);
        Task<Role> SetScopes(Role role, params string[] newScopes);
        Task<Role?> GetRole(string roleName);
    }
}