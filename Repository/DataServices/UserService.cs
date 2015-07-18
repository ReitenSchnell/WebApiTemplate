using System.Linq;
using PetaPoco;
using Repository.Models;

namespace Repository.DataServices
{
    public interface IUserService
    {
        User GetUserByLogin(string login);
    }

    public class UserService : IUserService
    {
        private readonly IStorage<User> storage;

        public UserService(IStorage<User> storage)
        {
            this.storage = storage;
        }

        public User GetUserByLogin(string login)
        {
            var pd = TableInfo.FromPoco(typeof(User));
            var user = storage.Query(new Sql("select * from " + pd.TableName + " where login = @0", login)).FirstOrDefault();
            return user;
        }
    }
}