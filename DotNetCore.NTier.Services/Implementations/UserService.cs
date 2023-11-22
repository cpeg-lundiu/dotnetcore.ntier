using DotNetCore.NTier.Models.Dao;
using Microsoft.EntityFrameworkCore;

namespace DotNetCore.NTier.Services
{
    public class UserService : IUserService
    {
        private readonly AppDataContext _context;

        public UserService(AppDataContext context)
        {
            _context = context;
        }

        public IAsyncEnumerable<User> GetUsers()
        {
            return _context.Users.AsAsyncEnumerable();
        }
    }
}
