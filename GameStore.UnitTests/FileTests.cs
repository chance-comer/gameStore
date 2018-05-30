using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Web.Mvc;
using GameStore.Domain.Entities;
using GameStore.Domain.Abstract;
using GameStore.WebUI.Controllers;
using System.Collections.Generic;

namespace GameStore.UnitTests {
    /// <summary>
    /// Summary description for FileTests
    /// </summary>
    [TestClass]
    public class FileTests {

        [TestMethod]
        public void Can_Retrieve_Image_Data() {
            GameController gameController = null;
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game> {
                new Game { GameId = 1 },
                new Game { GameId = 2, ImageData = new byte[20], ImageMimeType = "img" },
                new Game { GameId = 3 },
            });
            gameController = new GameController(mock.Object);
            ActionResult file = gameController.GetImage(2);
            Assert.IsNotNull(file);
            Assert.IsInstanceOfType(file, typeof(FileContentResult));
            Assert.AreEqual(((FileResult)file).ContentType, "img");
        }

        [TestMethod]
        public void Cannot_Retrieve_Image_Data() {
            GameController gameController = null;
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game> {
                new Game { GameId = 1 },
                new Game { GameId = 2, ImageData = new byte[20], ImageMimeType = "img" },
                new Game { GameId = 3 },
            });
            gameController = new GameController(mock.Object);
            ActionResult file = gameController.GetImage(25);
            Assert.IsNull(file);
        }
    }
}
