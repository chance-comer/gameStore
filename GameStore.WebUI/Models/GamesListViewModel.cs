using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GameStore.Domain.Entities;
using GameStore.WebUI.Models;
//using GameStore.WebUI.HtmlHelpers;

namespace GameStore.WebUI.Models {
    public class GamesListViewModel {
        public IEnumerable<Game> Games { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public string CurrentCategory { get; set; }
    }
}