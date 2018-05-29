using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameStore.Domain.Entities;
using GameStore.Domain.Abstract;
using GameStore.WebUI.Models;

namespace GameStore.WebUI.Controllers
{
    public class CartController : Controller
    {
        IGameRepository repository = null;
        IOrderProcessor orderProcessor = null;

        public CartController(IGameRepository repo, IOrderProcessor orderProc) {
            repository = repo;
            orderProcessor = orderProc;
        }

        public RedirectToRouteResult AddToCart(Cart cart, int gameId, string returnUrl) {
            Game game = repository.Games.FirstOrDefault(g => g.GameId == gameId);

            if (game != null) {
                cart.AddItem(game, 1);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(Cart cart, int gameId, string returnUrl) {
            Game game = repository.Games
                .FirstOrDefault(g => g.GameId == gameId);

            if (game != null) {
                cart.RemoveLine(game);
            }
            return RedirectToAction("Index", new { returnUrl });
        }

        public ViewResult Index(Cart cart, string returnUrl) {
            return View(new CartIndexViewModel {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }

        public PartialViewResult Summary(Cart cart) {
            return PartialView(cart);
        }

        public ViewResult Checkout() {
            return View(new ShippingDetails());
        }

        [HttpPost]
        public ViewResult Checkout(Cart cart, ShippingDetails shippingDetails) {

            if (cart.Lines.Count() == 0) {
                ModelState.AddModelError("", "Корзина пуста");
            }

            if (ModelState.IsValid) {
                orderProcessor.ProcessOrder(cart, shippingDetails);
                cart.Clear();
                return View("Completed");
            } else {
                return View(shippingDetails);
            }
        }

        /*
        public Cart GetCart() {
            Cart cart = (Cart)Session["Cart"];

            if (cart == null) {
                cart = new Cart();
                Session["Cart"] = cart;
            }

            return cart;
        }*/
    }
}