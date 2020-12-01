using System.Threading.Tasks;
using ABASim.api.Models;

namespace ABASim.api.Data
{
    public interface IAuthRepository
    {
         Task<User> Register(User user, string password);
         Task<User> Login(string username, string password);
         Task<bool> UserExists(string username);

         Task<bool> RegisterTeam(int userId, int teamId);
    }
}