using Kunicardus.Billboards.Core.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kunicardus.Billboards.Core;
using Kunicardus.Billboards.Core.Services.Abstract;
using Kunicardus.Billboards.Core.ViewModels;
using Kunicardus.Billboards.Core.Models;
using Autofac;
using Kunicardus.Billboards.Core.UnicardApiProvider;
using Kunicardus.Billboards.Core.Plugins;
using Uniboard.UnitTests.Fakes;
using NSubstitute;
using Kunicardus.Billboards.Core.Models.DataTransferObjects;
using System.Collections;

namespace Uniboard.UnitTests
{
    [TestFixture]
    public class AuthorizationTests
    {
        IAuthService _authService;

        [SetUp]
        public void BeforeEachTest()
        {
            //IConnectivityPlugin networkService = new AlwaysAvailableFakeConnectivityPlugin();
        }

        //        [Test]
        //        public void IsValidLoginData_ValidData_ReturnsTrue()
        //        {
        //            IUnicardApiProvider apiProvider = new ValidAuthorizationFakeUnicardApiProvider();
        //            _authService = new AuthService(apiProvider);
        //            BaseActionResult<UserModel> result = _authService.Auth("smamuchishvili@gmail.com", "bulvari111", "");
        //            Assert.True(result.Result != null);
        //            Assert.True(result.Success);
        //            Assert.True(!string.IsNullOrEmpty(result.Result.UserId));
        //            Assert.True(!string.IsNullOrEmpty(result.Result.SessionId));
        //        }

        [Test]
        public void IsValidLoginData_ValidData_ReturnsTrue()
        {
            IUnicardApiProvider apiProvider = Substitute.For<IUnicardApiProvider>();

            _authService = new AuthService(apiProvider);

            apiProvider.Post<AuthResponse>(Arg.Any<string>(), null, Arg.Any<string>()).Returns(new AuthResponse
                {
                    DisplayMessage = "",
                    ErrorMessage = "",
                    ResultCode = "200",
                    SessionID = "123456789",
                    UserId = "8491"
                });

            BaseActionResult<UserModel> result = _authService.Auth("smamuchishvili@gmail.com", "bulvari111", "");
            Assert.True(result.Result != null);
            Assert.True(!string.IsNullOrEmpty(result.Result.UserId));
            Assert.True(!string.IsNullOrEmpty(result.Result.SessionId));
        }

        [Test]
        public void Test()
        {
            
        }

        [Test]
        public void IsValidLoginData_InvalidData_ReturnsFalse()
        {
            IUnicardApiProvider apiProvider = new InValidAuthorizationFakeUnicardApiProvider();
            _authService = new AuthService(apiProvider);
            BaseActionResult<UserModel> result = _authService.Auth("smamuchishvili@gmail.com", "bulvari111", "");
            Assert.True(result.Result != null);
            Assert.False(result.Success);
            Assert.True(string.IsNullOrEmpty(result.Result.UserId));
            Assert.True(string.IsNullOrEmpty(result.Result.SessionId));
        }
    }
}
