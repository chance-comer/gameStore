using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Domain.Entities {
    public class Game {
        [HiddenInput(DisplayValue = false)]
        public int GameId { get; set; }

        [Display(Name = "Название")]
        [Required(ErrorMessage = "Введите название игры")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Введите описание игры")]
        public string Description { get; set; }

        [Display(Name = "Категория")]
        [Required(ErrorMessage = "Укажите категорию игры")]
        public string Category { get; set; }

        [Display(Name = "Цена (руб)")]
        [Required(ErrorMessage = "Введите цену игры")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0.01 рубля")]
        public decimal Price { get; set; }
    }
}
