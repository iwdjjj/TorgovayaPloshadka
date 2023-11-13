using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TorgovayaPloshadka.Models
{
    public class Order
    {
        [HiddenInput(DisplayValue = false)]
        public int OrderId { get; set; }

        [Display(Name = "Покупатель")]
        public int? CustomerId { get; set; }
        [Display(Name = "Покупатель")]
        public virtual Customer? Customer { get; set; }

        [Display(Name = "Товар")]
        public int? ProductId { get; set; }
        [Display(Name = "Товар")]
        public virtual Product? Product { get; set; }

        [Required(ErrorMessage = "Укажите дату заказа")]
        [Display(Name = "Дата заказа")]
        public DateTime? Date_Ordered { get; set; }

        [Required(ErrorMessage = "Укажите количество")]
        [Display(Name = "Количество")]
        public int? Count { get; set; }
    }
}
