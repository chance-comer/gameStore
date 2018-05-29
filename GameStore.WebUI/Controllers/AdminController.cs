using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;

namespace GameStore.WebUI.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        IGameRepository repository = null;

        public AdminController(IGameRepository repository) {
            this.repository = repository;
        }
        // GET: Admin
        public ViewResult Index()
        {
            return View(repository.Games);
        }

        public ViewResult Edit(int gameId) {
            Game game = repository.Games.FirstOrDefault(m => m.GameId == gameId);
            return View(game);
        }

        [HttpPost]
        public ActionResult Edit(Game game) {
            if (ModelState.IsValid) {
                repository.SaveGame(game);
                TempData["message"] = string.Format("Изменения в игре \"{0}\" были сохранены", game.Name);
                return RedirectToAction("Index");
            } else return View(game);
        }
        
        public ActionResult Create() {
            return View("Edit", new Game());
        }

        [HttpPost]
        public ActionResult Delete(int gameId) {
            Game game = repository.DeleteGame(gameId);

            if (game != null) {
                TempData["message"] = string.Format("Игра \"{0}\" была удалена", game.Name);
            }
            return RedirectToAction("Index");
        }

    }
}