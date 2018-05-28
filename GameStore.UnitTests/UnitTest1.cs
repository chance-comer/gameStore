using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameStore.WebUI.Controllers;
using System.Collections.Generic;
using GameStore.Domain.Abstract;
using Moq;
using GameStore.Domain.Entities;
using System.Web.Mvc;
using System.Linq;
using GameStore.WebUI.HtmlHelpers;
using GameStore.WebUI.Models;

namespace GameStore.UnitTests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void CanPaginate() {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();

            mock.Setup(m => m.Games).Returns(new List<Game> {
                new Game { GameId = 1, Name = "game1" },
                new Game { GameId = 2, Name = "game2" },
                new Game { GameId = 3, Name = "game3" },
                new Game { GameId = 4, Name = "game4" },
                new Game { GameId = 5, Name = "game5" }
            });

            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;

            IEnumerable<Game> result = ((GamesListViewModel)controller.List(null, 2).Model).Games;

            List<Game> result_list = result.ToList<Game>();
            Assert.AreEqual(2, result_list.Count);
            Assert.AreEqual("game4", result_list[0].Name);
            Assert.AreEqual("game5", result_list[1].Name);
        }

        [TestMethod]
        public void Can_Generate_Page_Links() {
            HtmlHelper helper = null;

            PagingInfo pagingInfo = new PagingInfo {
                CurrentPage = 2,
                TotalItems = 11,
                ItemsPerPage = 4
            };

            Func<int, string> funcDelegate = i => "Page" + i;

            MvcHtmlString result = helper.PageLinks(pagingInfo, funcDelegate);

            Assert.AreEqual(@"<a class=""btn btn-default"" href=""Page1"">1</a>"
                + @"<a class=""btn btn-default btn-primary selected"" href=""Page2"">2</a>"
                + @"<a class=""btn btn-default"" href=""Page3"">3</a>", result.ToString());
        }

        [TestMethod]
        public void Can_Send_Pagination_View_Model() {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game> {
                new Game { GameId = 1, Name = "game1" },
                new Game { GameId = 2, Name = "game2" },
                new Game { GameId = 3, Name = "game3" },
                new Game { GameId = 4, Name = "game4" },
                new Game { GameId = 5, Name = "game5" }
            });
            
            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;

            GamesListViewModel gamesListViewModel = (GamesListViewModel)controller.List(null, 2).Model;

            PagingInfo pagingInfo = gamesListViewModel.PagingInfo;

            Assert.AreEqual(2, pagingInfo.TotalPages);
            Assert.AreEqual(3, pagingInfo.ItemsPerPage);
            Assert.AreEqual(5, pagingInfo.TotalItems);
            Assert.AreEqual(2, pagingInfo.CurrentPage);
        }

        [TestMethod]
        public void Can_Filter_Games() {

            Mock<IGameRepository> mock = new Mock<IGameRepository>();

            mock.Setup(m => m.Games).Returns(new List<Game> {
                new Game { GameId = 1, Name = "game1", Category = "shooter" },
                new Game { GameId = 2, Name = "game2", Category = "action" },
                new Game { GameId = 3, Name = "game3", Category = "action" },
                new Game { GameId = 4, Name = "game4", Category = "rpg" },
                new Game { GameId = 5, Name = "game5", Category = "shooter" }
            });

            GameController controller = new GameController(mock.Object);

            GamesListViewModel gamesListViewModel = (GamesListViewModel)controller.List("shooter", 1).Model;
            List<Game> result = gamesListViewModel.Games.ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result[0].Category == "shooter" && "game1" == result[0].Name);
            Assert.IsTrue(result[1].Category == "shooter" && "game5" == result[1].Name);
        }

        [TestMethod]
        public void Can_Create_Categories() {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game> {
                new Game { GameId = 1, Name = "game1", Category = "shooter" },
                new Game { GameId = 2, Name = "game2", Category = "action" },
                new Game { GameId = 3, Name = "game3", Category = "action" },
                new Game { GameId = 4, Name = "game4", Category = "rpg" },
                new Game { GameId = 5, Name = "game5", Category = "shooter" }
            });

            NavController controller = new NavController(mock.Object);

            List<string> categories = ((IEnumerable<string>)controller.Menu().Model).ToList();

            Assert.AreEqual(3, categories.Count);
            Assert.IsTrue(categories[0] == "action");
            Assert.IsTrue(categories[1] == "rpg");
            Assert.IsTrue(categories[2] == "shooter");
        }

        [TestMethod]
        public void Indicates_Selected_Category() {
            // Организация - создание имитированного хранилища
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new Game[] {
              new Game { GameId = 1, Name = "Игра1", Category="Симулятор"},
              new Game { GameId = 2, Name = "Игра2", Category="Шутер"}
            });

            // Организация - создание контроллера
            NavController target = new NavController(mock.Object);

            // Организация - определение выбранной категории
            string categoryToSelect = "Шутер";

            // Действие
            string result = target.Menu(categoryToSelect).ViewBag.SelectedCategory;

            // Утверждение
            Assert.AreEqual(categoryToSelect, result);
        }

        [TestMethod]
        public void Generate_Category_Specific_Game_Count() {
            /// Организация (arrange)
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game>
            {
                new Game { GameId = 1, Name = "Игра1", Category="Cat1"},
                new Game { GameId = 2, Name = "Игра2", Category="Cat2"},
                new Game { GameId = 3, Name = "Игра3", Category="Cat1"},
                new Game { GameId = 4, Name = "Игра4", Category="Cat2"},
                new Game { GameId = 5, Name = "Игра5", Category="Cat3"}
            });
            GameController controller = new GameController(mock.Object);
            controller.pageSize = 3;

            // Действие - тестирование счетчиков товаров для различных категорий
            int res1 = ((GamesListViewModel)controller.List("Cat1").Model).PagingInfo.TotalItems;
            int res2 = ((GamesListViewModel)controller.List("Cat2").Model).PagingInfo.TotalItems;
            int res3 = ((GamesListViewModel)controller.List("Cat3").Model).PagingInfo.TotalItems;
            int resAll = ((GamesListViewModel)controller.List(null).Model).PagingInfo.TotalItems;

            // Утверждение
            Assert.AreEqual(res1, 2);
            Assert.AreEqual(res2, 2);
            Assert.AreEqual(res3, 1);
            Assert.AreEqual(resAll, 5);
        }
    }
}
