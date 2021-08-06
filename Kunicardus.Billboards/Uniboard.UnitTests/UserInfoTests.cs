using Kunicardus.Billboards.Core.Models;
using Kunicardus.Billboards.Core.Services.Abstract;
using Kunicardus.Billboards.Core.Services.Concrete;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uniboard.UnitTests.Fakes.Providers;

namespace Uniboard.UnitTests
{
    [TestFixture]
    public class UserInfoTests
    {
        IUserService _userService;

        [SetUp]
        public void BeforeEachTest()
        {

        }

        [Test]
        public void IsValidUserBalance_Valid_ReturnsUserBalanceModel()
        {
            _userService = new UserService(new ValidUserBalanceFakeUnicardApiProvider());
            BaseActionResult<UserBalanceModel> result = _userService.GetUserBalance("8941");
            Assert.True(result != null && result.Result!=null);
            Assert.True(result.Success);
        }

        [Test]
        public void IsValidUserBalance_InValid_ReturnsUserBalanceModel()
        {
            _userService = new UserService(new InValidUserBalanceFakeUnicardApiProvider());
            BaseActionResult<UserBalanceModel> result = _userService.GetUserBalance("8941");
            Assert.True(result != null && result.Result != null);
            Assert.False(result.Success);
        }
    }
}
