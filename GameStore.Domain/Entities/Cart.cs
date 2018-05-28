using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Domain.Entities {
    public class Cart {
        private List<CartLine> cart = new List<CartLine>();

        public void AddItem(Game game, int quantity) {
            CartLine cartLine = cart.Where(g => g.Game.GameId == game.GameId).FirstOrDefault();

            if (cartLine == null) {
                cart.Add(new CartLine { Game = game, Quantity = quantity });
            } else cartLine.Quantity += quantity;
        }

        public void RemoveLine(Game game) {
            cart.RemoveAll(g => g.Game.GameId == game.GameId);
        }

        public void Clear() {
            cart.Clear();
        }

        public decimal ComputeTotalValue() {
            return cart.Sum(l => l.Quantity * l.Game.Price);
        }

        public IEnumerable<CartLine> Lines {
            get { return cart; }
        }
    }

    public class CartLine {
        public Game Game { get; set; }
        public int Quantity { get; set; }
    }
}
