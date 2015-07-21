using System;
using System.Collections.Generic;
using System.ServiceModel.Security;
using Common;
using Common.Contracts;
using FluentAssertions;
using NSubstitute;
using PetaPoco;
using PetaPoco.Internal;
using Ploeh.AutoFixture.Xunit;
using Repository;
using Repository.DataServices;
using Repository.Models;
using Xunit;
using Xunit.Extensions;

namespace Tests.Repository.DataServices
{
    public class UserServiceTests
    {
        private readonly IStorage<User> storage = Substitute.For<IStorage<User>>();
        private readonly ICryptographyService cryptographyService = Substitute.For<ICryptographyService>();
        private readonly IIdentityGenerator identityGenerator = Substitute.For<IIdentityGenerator>();
        private readonly UserService userService;

        public UserServiceTests()
        {
            userService = new UserService(storage, cryptographyService, identityGenerator);
        }

        [Theory, AutoData]
        public void should_find_user_by_login(string login, User user)
        {
            storage.Query(null).ReturnsForAnyArgs(new List<User>{user});
            var result = userService.GetUserByLogin(login);
            result.Should().Be(user);
        }

        [Theory, AutoData]
        public void should_create_user_and_insert_it_to_storage(RegistrationInfo registrationInfo, Guid passwordSalt, string passwordHash, Guid userId)
        {
            identityGenerator.GetIdentity().Returns(passwordSalt, userId);
            cryptographyService.GetPasswordHash(registrationInfo.Password, passwordSalt).Returns(passwordHash);

            userService.CreateUser(registrationInfo);

            storage.Received().Insert(Arg.Is<User>(user => user.Id == userId && user.Name == registrationInfo.Name 
                && user.Login == registrationInfo.Login 
                && user.PasswordHash == passwordHash 
                && user.PasswordSalt == passwordSalt));
        }

        [Theory, AutoData]
        public void should_create_user_and_return_his_id(RegistrationInfo registrationInfo, Guid passwordSalt, string passwordHash, Guid userId)
        {
            identityGenerator.GetIdentity().Returns(passwordSalt, userId);
            cryptographyService.GetPasswordHash(registrationInfo.Password, passwordSalt).Returns(passwordHash);

            var result = userService.CreateUser(registrationInfo);

            result.Should().Be(userId);
        }
    }
}