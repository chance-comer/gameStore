using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameStore.Domain.Entities;
using GameStore.Domain.Abstract;
using GameStore.WebUI.Controllers;
using Moq;
using System.Linq;
using System.Web.Mvc;
using GameStore.WebUI.Models;

namespace GameStore.UnitTests {
    /// <summary>
    /// Summary description for CartTests
    /// </summary>
    [TestClass]
    public class CartTests {
        [TestMethod]
        public void Can_Add_New_Lines() {
            Game game1 = new Game { Name = "game1", Price = 100, GameId = 1 };
            Game game2 = new Game { Name = "game2", Price = 110, GameId = 2 };

            Cart cart = new Cart();
            cart.AddItem(game1, 1);
            cart.AddItem(game2, 2);

            List<CartLine> result = cart.Lines.ToList();

            Assert.IsTrue(result.Count == 2);
            Assert.AreEqual(result[0].Game, game1);
            Assert.AreEqual(result[1].Game, game2);
        }

        [TestMethod]
        public void Can_Add_Quantity_For_Existing_Lines() {
            // Организация - создание нескольких тестовых игр
            Game game1 = new Game { GameId = 1, Name = "Игра1" };
            Game game2 = new Game { GameId = 2, Name = "Игра2" };

            // Организация - создание корзины
            Cart cart = new Cart();

            // Действие
            cart.AddItem(game1, 1);
            cart.AddItem(game2, 1);
            cart.AddItem(game1, 5);
            List<CartLine> results = cart.Lines.OrderBy(c => c.Game.GameId).ToList();

            // Утверждение
            Assert.AreEqual(results.Count(), 2);
            Assert.AreEqual(results[0].Quantity, 6);    // 6 экземпляров добавлено в корзину
            Assert.AreEqual(results[1].Quantity, 1);
        }

        [TestMethod]
        public void Can_Remove_Line() {
            // Организация - создание нескольких тестовых игр
            Game game1 = new Game { GameId = 1, Name = "Игра1" };
            Game game2 = new Game { GameId = 2, Name = "Игра2" };
            Game game3 = new Game { GameId = 3, Name = "Игра3" };

            Cart cart = new Cart();
            cart.AddItem(game1, 1);
            cart.AddItem(game2, 2);
            cart.AddItem(game3, 1);

            cart.RemoveLine(game1);

            List<CartLine> res = cart.Lines.ToList();

            Assert.AreEqual(2, res.Count);
            Assert.AreEqual(0, cart.Lines.Where(g => g.Game == game1).Count());
            Assert.AreEqual(res[0].Game, game2);
            Assert.AreEqual(res[1].Game, game3);
        }

        [TestMethod]
        public void Calculate_Total_Price() {
            Game game1 = new Game { Name = "game1", Price = 100, GameId = 1 };
            Game game2 = new Game { Name = "game2", Price = 110, GameId = 2 };

            Cart cart = new Cart();

            cart.AddItem(game1, 1);
            cart.AddItem(game2, 2);
            cart.AddItem(game1, 4);

            decimal totalPrice = cart.ComputeTotalValue();

            Assert.AreEqual(720, totalPrice);
        }

        [TestMethod]
        public void Can_Clear_Contents() {
            // Организация - создание нескольких тестовых игр
            Game game1 = new Game { GameId = 1, Name = "Игра1", Price = 100 };
            Game game2 = new Game { GameId = 2, Name = "Игра2", Price = 55 };

            // Организация - создание корзины
            Cart cart = new Cart();

            // Действие
            cart.AddItem(game1, 1);
            cart.AddItem(game2, 1);
            cart.AddItem(game1, 5);
            cart.Clear();

            // Утверждение
            Assert.AreEqual(cart.Lines.Count(), 0);
        }

        [TestMethod]
        public void Can_Add_To_Cart() {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns(new List<Game> {
               new Game { GameId = 1, Name = "Игра1", Price = 100 }
            });

            CartController cartController = new CartController(mock.Object, null);
            Cart cart = new Cart();

            cartController.AddToCart(cart, 1, null);

            Assert.AreEqual(cart.Lines.Count(), 1);
            Assert.AreEqual(cart.Lines.ToList()[0].Game.GameId, 1);
        }

        [TestMethod]
        public void Adding_Game_To_Cart_Goes_To_Cart_Screen() {
            Mock<IGameRepository> mock = new Mock<IGameRepository>();
            mock.Setup(m => m.Games).Returns( new List<Game> {
                new Game { GameId = 1, Name = "Игра1", Price = 100 }
            });
            
            CartController cartController = new CartController(mock.Object, null);
            Cart cart = new Cart();

            RedirectToRouteResult result = cartController.AddToCart(cart, 1, "myUrl");

            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("myUrl", result.RouteValues["returnUrl"]);
        }

        [TestMethod]
        public void Can_View_Cart_Contents() {
            CartController cartController = new CartController(null, null);
            Cart cart = new Cart();
            CartIndexViewModel cartIndexViewModel =  (CartIndexViewModel)cartController.Index(cart, "myUrl").ViewData.Model;

            Assert.AreEqual("myUrl", cartIndexViewModel.ReturnUrl);
            Assert.AreSame(cart, cartIndexViewModel.Cart);
        }

        [TestMethod]
        public void Cannot_Checkout_Empty_Cart() {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();
            Cart cart = new Cart();
            ShippingDetails shippingDetails = new ShippingDetails();

            CartController cartController = new CartController(null, mock.Object);
            ViewResult result = cartController.Checkout(cart, shippingDetails);
             
            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), shippingDetails), Times.Never);

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Cannot_Checkout_Invalid_ShippingDetails() {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            CartController cartController = new CartController(null, mock.Object);
            ShippingDetails shippingDetails = new ShippingDetails();

            Cart cart = new Cart();
            cart.AddItem(new Game(), 1);

            cartController.ModelState.AddModelError("error", "error");

            ViewResult result = cartController.Checkout(cart, shippingDetails);

            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), shippingDetails), Times.Never);

            Assert.AreEqual("", result.ViewName);
            Assert.AreEqual(false, result.ViewData.ModelState.IsValid);
        }

        [TestMethod]
        public void Can_Checkout_And_Submit_Order() {
            Mock<IOrderProcessor> mock = new Mock<IOrderProcessor>();

            CartController cartController = new CartController(null, mock.Object);
            ShippingDetails shippingDetails = new ShippingDetails();

            Cart cart = new Cart();
            cart.AddItem(new Game(), 1);

            ViewResult result = cartController.Checkout(cart, shippingDetails);

            mock.Verify(m => m.ProcessOrder(It.IsAny<Cart>(), shippingDetails), Times.Once);

            Assert.AreEqual("Completed", result.ViewName);
            Assert.AreEqual(true, result.ViewData.ModelState.IsValid);
        }
    }
}
