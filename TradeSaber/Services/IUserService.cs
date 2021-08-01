using System;
using System.Threading.Tasks;
using TradeSaber.Models;

namespace TradeSaber.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Gets a user from the TradeSaber database.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="full">Whether or not to include all of their relations.</param>
        /// <returns></returns>
        Task<User?> GetUser(Guid id, bool full = false);

        /// <summary>
        /// Creates a new user and adds them to the database.
        /// </summary>
        /// <param name="id">The ID to assign to the user.</param>
        /// <returns></returns>
        Task<User> CreateNewUser(Guid id);
    }
}