using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TorgovayaPloshadka.Models
{
    public class Category
    {
        [HiddenInput(DisplayValue = false)]
        public int CategoryId { get; set; }

        //[Required(ErrorMessage = "Укажите название категория товаров")]
        [Display(Name = "Категория товаров")]
        public string? CategoryName { get; set; }

        [Display(Name = "Описание категория товаров")]
        public string? Description { get; set; }

        [Display(Name = "Количество товаров")]
        public int? Products_Count { get; set; }

        public virtual ICollection<Product>? Product { get; set; }
    }
}
