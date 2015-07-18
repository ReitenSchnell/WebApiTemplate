using System;
using System.Linq;
using Common;
using Common.Contracts;
using PetaPoco;
using Repository.Models;

namespace Repository.DataServices
{
    public interface IUserService
    {
        User GetUserByLogin(string login);
        Guid CreateUser(RegistrationInfo registrationInfo);
    }

    public class UserService : IUserService
    {
        private readonly IStorage<User> storage;
        private readonly ICryptographyService cryptographyService;

        public UserService(IStorage<User> storage, ICryptographyService cryptographyService)
        {
            this.storage = storage;
            this.cryptographyService = cryptographyService;
        }

        public User GetUserByLogin(string login)
        {
            var pd = TableInfo.FromPoco(typeof(User));
            var user = storage.Query(new Sql("select * from " + pd.TableName + " where login = @0", login)).FirstOrDefault();
            return user;
        }

        public Guid CreateUser(RegistrationInfo registrationInfo)
        {
            var passwordSalt = Guid.NewGuid();
            var passwordHash = cryptographyService.GetPasswordHash(registrationInfo.Password, passwordSalt);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Login = registrationInfo.Login,
                Name = registrationInfo.Name,
                PasswordSalt = passwordSalt,
                PasswordHash = passwordHash
            };
            storage.Insert(user);
            return user.Id;
        }
    }
}