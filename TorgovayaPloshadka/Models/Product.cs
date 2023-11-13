using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TorgovayaPloshadka.Models
{
    public class Product
    {
        [HiddenInput(DisplayValue = false)]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Укажите название товара")]
        [Display(Name = "Название товара")]
        public string? ProductName { get; set; }

        [Display(Name = "Категория товара")]
        public int? CategoryId { get; set; }
        [Display(Name = "Категория товара")]
        public virtual Category? Category { get; set; }

        [Display(Name = "Производитель")]
        public int? ManufacturerId { get; set; }
        [Display(Name = "Производитель")]
        public virtual Manufacturer? Manufacturer { get; set; }

        [Display(Name = "Поставщик")]
        public int? SupplierId { get; set; }
        [Display(Name = "Поставщик")]
        public virtual Supplier? Supplier { get; set; }

        [Required(ErrorMessage = "Укажите цену товара")]
        [Display(Name = "Цена товара")]
        public int? Price { get; set; }

        [Required(ErrorMessage = "Укажите вес товара")]
        [Display(Name = "Вес товара")]
        public int? Weight { get; set; }

        [Required(ErrorMessage = "Укажите размеры товара")]
        [Display(Name = "Размеры товара")]
        public string? Proportions { get; set; }

        public virtual ICollection<Order>? Order { get; set; }
    }
}
