using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using GameStore.Domain.Abstract;
using System.Collections.Generic;
using GameStore.Domain.Entities;
using GameStore.WebUI.Controllers;
using System.Web.Mvc;
using System.Linq;

namespace GameStore.UnitTests {
    [TestClass]
    public class AdminTests {
        [TestMethod]
        public void Index_Contains_All_Games() {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game> {
                new Game { GameId = 1 },
                new Game { GameId = 2 },
                new Game { GameId = 3 },
                new Game { GameId = 4 }
            });

            AdminController adminController = new AdminController(mock.Object);

            ViewResult result = adminController.Index();

            List<Game> list = ((IEnumerable<Game>)result.Model).ToList();

            Assert.AreEqual(4, list.Count());
            Assert.AreEqual(1, list[0].GameId);
            Assert.AreEqual(2, list[1].GameId);
            Assert.AreEqual(3, list[2].GameId);
        }

        [TestMethod]
        public void Can_Edit_Game() {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game> {
                new Game { GameId = 1, Name = "Игра1" },
                new Game { GameId = 2, Name = "Игра2" },
                new Game { GameId = 3, Name = "Игра3" },
                new Game { GameId = 4, Name = "Игра4" },
                new Game { GameId = 5, Name = "Игра5" }
            });

            AdminController adminController = new AdminController(mock.Object);

            Game game1 = (Game)adminController.Edit(1).ViewData.Model;
            Game game2 = (Game)adminController.Edit(2).ViewData.Model;
            Game game3 = (Game)adminController.Edit(3).ViewData.Model;

            Assert.AreEqual(1, game1.GameId);
            Assert.AreEqual(2, game2.GameId);
            Assert.AreEqual(3, game3.GameId);
        }

        [TestMethod]
        public void Cannot_Edit_Nonexistent_Game() {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game> {
                new Game { GameId = 1, Name = "Игра1" },
                new Game { GameId = 2, Name = "Игра2" },
                new Game { GameId = 3, Name = "Игра3" },
                new Game { GameId = 4, Name = "Игра4" },
                new Game { GameId = 5, Name = "Игра5" }
            });

            AdminController adminController = new AdminController(mock.Object);

            Game game = (Game)adminController.Edit(6).ViewData.Model;

            Assert.AreEqual(null, game);
        }

        [TestMethod]
        public void Can_Save_Valid_Changes() {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();

            AdminController adminController = new AdminController(mock.Object);

            Game game = new Game {
                GameId = 1
            };

            ActionResult result = adminController.Edit(game);

            mock.Verify(m => m.SaveGame(game));
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void Cannot_Save_Invalid_Changes() {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();

            AdminController adminController = new AdminController(mock.Object);

            Game game = new Game {
                GameId = 1
            };

            adminController.ModelState.AddModelError("error", "error");

            ActionResult result = adminController.Edit(game);

            mock.Verify(m => m.SaveGame(It.IsAny<Game>()), Times.Never);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void Can_Delete_Valid_Games() {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();

            mock.Setup(m => m.Games).Returns( new List<Game> {
                new Game { GameId = 1 },
                new Game { GameId = 2 },
                new Game { GameId = 3 },
                new Game { GameId = 4 },
                new Game { GameId = 5 }
            });

            AdminController adminController = new AdminController(mock.Object);

            Game game = new Game { GameId = 2 };

            ActionResult result = adminController.Delete(game.GameId);

            mock.Verify(m => m.DeleteGame(game.GameId));
        }
    }
}
