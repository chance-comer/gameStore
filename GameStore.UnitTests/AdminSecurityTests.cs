using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameStore.WebUI.Controllers;
using GameStore.WebUI.Infrastructure.Abstract;
using Moq;
using GameStore.WebUI.Models;
using System.Web.Mvc;

namespace GameStore.UnitTests {
    [TestClass]
    public class AdminSecurityTests {
        [TestMethod]
        public void Can_Login_With_Valid_Credentials() {
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("validName", "validPass")).Returns(true);

            AccountController accountController = new AccountController(mock.Object);

            LoginViewModel loginViewModel = new LoginViewModel {
                UserName = "validName",
                Password = "validPass"
            };

            ActionResult actionResult = accountController.Login(loginViewModel, "url");

            Assert.IsInstanceOfType(actionResult, typeof(RedirectResult));
            Assert.AreEqual("url", ((RedirectResult)actionResult).Url);
        }

        [TestMethod]
        public void Cannot_Login_With_Invalid_Credentials() {
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            mock.Setup(m => m.Authenticate("invalidName", "invalidPass")).Returns(false);

            AccountController accountController = new AccountController(mock.Object);

            LoginViewModel loginViewModel = new LoginViewModel {
                UserName = "invalidName",
                Password = "invalidPass"
            };
            
            ActionResult actionResult = accountController.Login(loginViewModel, "url");

            Assert.IsInstanceOfType(actionResult, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)actionResult).ViewData.ModelState.IsValid);
        }
    }
}
