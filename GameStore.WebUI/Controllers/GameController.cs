﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameStore.Domain.Abstract;
using GameStore.Domain.Entities;
using GameStore.WebUI.Models;

namespace GameStore.WebUI.Controllers
{
    public class GameController : Controller
    {
        private IGameRepository repository;
        public int pageSize = 4;

        public GameController(IGameRepository repo) {
            repository = repo;
        }

        public ViewResult List(string category, int page = 1) {
            /*return View(repository.Games
                .OrderBy(g => g.GameId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize));*/
            return View(new GamesListViewModel {
                Games = repository.Games
                .Where(g => category == null || g.Category == category)
                .OrderBy(g => g.GameId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize),
                PagingInfo = new PagingInfo {
                    CurrentPage = page,
                    ItemsPerPage = pageSize,
                    TotalItems = category == null ? repository.Games.Count() : repository.Games.Where(g => g.Category == category).Count()
                },
                CurrentCategory = category
            });
        }

        public FileContentResult GetImage(int gameId) {
            Game game = repository.Games.FirstOrDefault(g => g.GameId == gameId);

            if (game != null) {
                return File(game.ImageData, game.ImageMimeType);
            } else return null;
        }
    }
}