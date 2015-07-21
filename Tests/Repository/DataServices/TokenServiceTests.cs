using System;
using Common;
using FluentAssertions;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Repository.DataServices;
using Repository.Models;
using Xunit;
using Xunit.Extensions;

namespace Tests.Repository.DataServices
{
    public class TokenServiceTests
    {
        private readonly IEncryptor encryptor = Substitute.For<IEncryptor>();
        private readonly IUserService userService = Substitute.For<IUserService>();
        private readonly TokenService tokenService;

        public TokenServiceTests()
        {
            tokenService = new TokenService(encryptor, userService);
        }

        [Theory, AutoData]
        public void should_create_token(Guid userId, string login, string encryptedToken)
        {
            encryptor.Encrypt(Arg.Is<AuthenticationToken>(token => token.Login == login && token.UserId == userId))
                .Returns(encryptedToken);

            var result = tokenService.CreateToken(userId, login);

            result.Should().Be(encryptedToken);
        }

        [Theory, AutoData]
        public void should_return_null_if_authentication_data_was_not_decrypted(string token)
        {
            AuthenticationToken authenticationData = null;
            encryptor.Decrypt<AuthenticationToken>(token).Returns(authenticationData);

            var result = tokenService.GetUser(token);

            result.Should().BeNull();
        }

        [Theory, AutoData]
        public void should_return_null_if_authentication_data_is_expired(string token)
        {
            var authenticationData = Substitute.For<AuthenticationToken>();
            authenticationData.IsExpired().Returns(true);
            encryptor.Decrypt<AuthenticationToken>(token).Returns(authenticationData);

            var result = tokenService.GetUser(token);

            result.Should().BeNull();
        }

        [Theory, AutoData]
        public void should_return_user_from_storage(string token, User user, string login)
        {
            var authenticationData = Substitute.For<AuthenticationToken>();
            authenticationData.IsExpired().Returns(false);
            authenticationData.Login.Returns(login);
            encryptor.Decrypt<AuthenticationToken>(token).Returns(authenticationData);
            userService.GetUserByLogin(login).Returns(user);

            var result = tokenService.GetUser(token);

            result.Should().Be(user);
        }

        [Theory, AutoData]
        public void should_return_null_if_authentication_data_was_not_decrypted_on_refresh_token(string token)
        {
            AuthenticationToken authenticationData = null;
            encryptor.Decrypt<AuthenticationToken>(token).Returns(authenticationData);

            var result = tokenService.RefreshToken(token);

            result.Should().BeNull();
        }

        [Theory, AutoData]
        public void should_return_same_token_if_it_wont_be_expired_on_refresh_token(string token)
        {
            var authenticationData = Substitute.For<AuthenticationToken>();
            authenticationData.Expired.Returns(DateTime.MaxValue);
            encryptor.Decrypt<AuthenticationToken>(token).Returns(authenticationData);

            var result = tokenService.RefreshToken(token);

            result.Should().Be(token);
        }

        [Theory, AutoData]
        public void should_return_new_token_on_refresh_token(string token, string newToken)
        {
            var authenticationData = Substitute.For<AuthenticationToken>();
            authenticationData.Expired.Returns(DateTime.MinValue);
            encryptor.Decrypt<AuthenticationToken>(token).Returns(authenticationData);
            encryptor.Encrypt(
                Arg.Is<AuthenticationToken>(
                    _ => _.Login == authenticationData.Login && _.UserId == authenticationData.UserId))
                .Returns(newToken);

            var result = tokenService.RefreshToken(token);

            result.Should().Be(newToken);
        }
    }
}