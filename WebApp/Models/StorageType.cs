using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApp.Models;

namespace WebApp.Models
{
    public class StorageType
    {
        [Key]
        [Display(Name = "Код типа склада")]
        public int Id { get; set; }
        [Display(Name = "Тип склада")]
        [Required(ErrorMessage = "Не указан тип склада")]
        public string Name { get; set; }
        [Display(Name = "Температура")]
        [Required(ErrorMessage = "Не указана температура")]
        [Range(-50, 50, ErrorMessage = "Некорректное значение")]
        public int Temperature { get; set; }
        [Display(Name = "Влажность")]
        [Required(ErrorMessage = "Не указана влажность")]
        [Range(0, 100, ErrorMessage = "Некорректное значение")]
        public int Humidity { get; set; }
        [Display(Name = "Пожарная безопасность")]
        [Required(ErrorMessage = "Не заполнено поле")]
        public string Requirement { get; set; }
        [Display(Name = "Оборудование")]
        [Required(ErrorMessage = "Не заполнено поле")]
        public string Equipment { get; set; }
        public ICollection<Storage> Storages { get; set; }
        public StorageType()
        {
            Storages = new List<Storage>();
        }
    }
}