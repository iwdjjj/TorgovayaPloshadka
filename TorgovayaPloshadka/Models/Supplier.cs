using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TorgovayaPloshadka.Models
{
    public class Supplier
    {
        [HiddenInput(DisplayValue = false)]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Укажите название поставщика")]
        [Display(Name = "Название поставщика")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Укажите адрес")]
        [Display(Name = "Адрес")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Укажите номер телефона")]
        [Display(Name = "Номер телефона")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Укажите фамилию")]
        [Display(Name = "Фамилия")]
        public string? Surname { get; set; }

        [Required(ErrorMessage = "Укажите имя")]
        [Display(Name = "Имя")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Укажите отчество")]
        [Display(Name = "Отчество")]
        public string? Midname { get; set; }

        public string? FIO
        {
            get
            {
                return Surname + " " + Name + " " + Midname;
            }
        }

        public virtual ICollection<Product>? Product { get; set; }
    }
}
