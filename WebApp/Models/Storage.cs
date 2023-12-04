using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class Storage
    {
        [Key]
        [Display(Name = "Код склада")]
        public int Id { get; set; }
        [ForeignKey("StorageType")]
        [Display(Name = "Тип склада")]
        public int StorageTypeId { get; set; }
        [Display(Name = "Название")]
        [Required(ErrorMessage = "Не указано имя склада")]
        public string Name { get; set; }
        [Display(Name = "Номер")]
        [Required(ErrorMessage = "Не указан номер склада")]
        [Range(1, 99999999999, ErrorMessage = "Некорректный номер")]
        public int Number { get; set; }
        [Display(Name = "Площадь")]
        [Required(ErrorMessage = "Не указана площадь")]
        [Range(1, 9999999, ErrorMessage = "Некорректное значение")]
        public int Square { get; set; }
        [Display(Name = "Вместимость")]
        [Required(ErrorMessage = "Не указана вместимость")]
        [Range(1, 9999999, ErrorMessage = "Некорректное значение")]
        public int Capacity { get; set; }
        [Display(Name = "Занятость")]
        [Required(ErrorMessage = "Не указана занятость")]
        [Range(1, 9999999, ErrorMessage = "Некорректное значение")]
        public int Occupancy { get; set; }
        [Display(Name = "Износ")]
        [Range(1, 100, ErrorMessage = "Некорректное значение")]
        [Required(ErrorMessage = "Не указан износ")]
        public int Depreciation { get; set; }
        [Display(Name = "Дата последней проверки")]
        [Required(ErrorMessage = "Не указана дата")]
        [DataType(DataType.Date)]
        public DateTime CheckDate { get; set; }
        [Display(Name = "Тип склада")]
        public StorageType? StorageType { get; set; }
        public ICollection<AcceptedRecyclable> AcceptedRecyclables { get; set; }
        public Storage()
        {
            AcceptedRecyclables = new List<AcceptedRecyclable>();
        }
    }
}