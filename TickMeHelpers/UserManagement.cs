using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace TickMeHelpers
{
    public class UserManagement : ManagementBase<User>
    {
        public UserManagement(IConfiguration config):base(config, "user")
        {

        }
        
        public async Task<User> GetUserByAuthId(User user)
        {
            var cosmos = await GetClient();
            IQueryable<User> query = cosmos.CreateDocumentQuery<User>(CollectionLink).Where(e => e.AuthId.ToString() == user.AuthId.ToString());
            var res = query.ToList().FirstOrDefault();
            return res;
        }
    }
}
