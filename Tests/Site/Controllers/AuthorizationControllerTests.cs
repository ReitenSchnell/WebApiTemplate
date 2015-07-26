using System;
using System.Net.Http;
using Common;
using Common.Contracts;
using FluentAssertions;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Repository.DataServices;
using Repository.Models;
using Site.Controllers;
using Xunit;
using Xunit.Extensions;

namespace Tests.Site.Controllers
{
    public class AuthorizationControllerTests
    {
        private readonly IUserService userService = Substitute.For<IUserService>();
        private readonly ITokenService tokenService = Substitute.For<ITokenService>();
        private readonly ICryptographyService cryptographyService = Substitute.For<ICryptographyService>();
        private readonly AuthorizationController controller;

        public AuthorizationControllerTests()
        {
            controller = new AuthorizationController(userService, tokenService, cryptographyService);
        }

        [Theory, AutoData]
        public void should_return_unknown_user_when_user_is_not_found_on_signin(Credentials credentials)
        {
            User user = null;
            userService.GetUserByLogin(credentials.Login).Returns(user);

            var result = controller.Post(credentials);
            var message = result as HttpResponseMessage;

            message.ReasonPhrase.Should().Be(AuthorizationController.UnknownUserMessage);
        }

        [Theory, AutoData]
        public void should_return_unknown_user_when_user_is_found_but_password_not_recognized_on_signin(Credentials credentials, User user)
        {
            userService.GetUserByLogin(credentials.Login).Returns(user);
            cryptographyService.CheckPassword(user.PasswordHash, user.PasswordSalt, credentials.Password).Returns(false);

            var result = controller.Post(credentials);
            var message = result as HttpResponseMessage;

            message.ReasonPhrase.Should().Be(AuthorizationController.UnknownUserMessage);
        }

        [Theory, AutoData]
        public void should_return_response_with_new_token_on_signin(Credentials credentials, User user, string token)
        {
            userService.GetUserByLogin(credentials.Login).Returns(user);
            cryptographyService.CheckPassword(user.PasswordHash, user.PasswordSalt, credentials.Password).Returns(true);
            tokenService.CreateToken(user.Id, user.Login).Returns(token);

            var result = controller.Post(credentials);
            var response = result as TokenResponse;

            response.Token.Should().Be(token);
            response.Id.Should().Be(user.Id);
            response.Login.Should().Be(credentials.Login);
        }

        [Theory, AutoData]
        public void should_return_conflict_message_when_user_exists_on_signup(RegistrationInfo registrationInfo, User user)
        {
            userService.GetUserByLogin(registrationInfo.Login).Returns(user);

            var result = controller.Post(registrationInfo);
            var message = result as HttpResponseMessage;

            message.ReasonPhrase.Should().Be(AuthorizationController.UserAlreadyExistsMessage);
        }

        [Theory, AutoData]
        public void should_return_response_with_new_token_on_signup(RegistrationInfo registrationInfo, Guid userId, string token)
        {
            User user = null;
            userService.GetUserByLogin(registrationInfo.Login).Returns(user);
            userService.CreateUser(registrationInfo).Returns(userId);
            tokenService.CreateToken(userId, registrationInfo.Login).Returns(token);

            var result = controller.Post(registrationInfo);
            var response = result as TokenResponse;

            response.Id.Should().Be(userId);
            response.Token.Should().Be(token);
            response.Login.Should().Be(registrationInfo.Login);
        }
    }
}