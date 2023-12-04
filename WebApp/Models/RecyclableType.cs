using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class RecyclableType
    {
        [Key]
        [Display(Name = "Код вторсырья")]
        public int Id { get; set; }
        [Display(Name = "Вид вторсырья")]
        [Required(ErrorMessage = "Не указан вид вторсырья")]
        public string Name { get; set; }
        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Не указана цена")]
        [Range(0.0001, 10000, ErrorMessage = "Некорректная цена")]
        public double Price { get; set; }
        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Не указано описание")]
        public string Description { get; set; }
        [Display(Name = "Условия принятия")]
        [Required(ErrorMessage = "Не указаны условия принятия")]
        public string AcceptanceCondition { get; set; }
        [Display(Name = "Условия хранения")]
        [Required(ErrorMessage = "Не указаны условия хранения")]
        public string StorageCondition { get; set; }
        public ICollection<AcceptedRecyclable> AcceptedRecyclables { get; set; }
        public RecyclableType()
        {
            AcceptedRecyclables = new List<AcceptedRecyclable>();
        }
    }
}