using DotNetCore.NTier.Models.Dao;

namespace DotNetCore.NTier.Services
{
    public interface IUserService
    {
        IAsyncEnumerable<User> GetUsers();
    }
}
