using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameStore.Domain.Abstract;

namespace GameStore.WebUI.Controllers
{
    public class NavController : Controller
    {
        IGameRepository repository = null;

        public NavController(IGameRepository repo) {
            repository = repo;
        }
        // GET: NavControlle
        public PartialViewResult Menu(string category = null)
        {
            ViewBag.SelectedCategory = category;
            IEnumerable<string> categories = repository.Games.Select(x => x.Category).Distinct().OrderBy(x => x);
            return PartialView(categories);
        }
    }
}