using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Position")]
        [Display(Name = "Должность")]
        public int PositionId { get; set; }
        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Не указано имя")]
        public string Name { get; set; }
        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Не указана фамилия")]
        public string Surname { get; set; }
        [Display(Name = "Отчество")]
        [Required(ErrorMessage = "Не указано отчетсво")]
        public string Patronymic { get; set; }
        [Display(Name = "Опыт")]
        [Required(ErrorMessage = "Не указан опыт")]
        [Range(1, 100, ErrorMessage = "Значение находится вне допустимых границ")]
        public int Experience { get; set; }
        [Display(Name = "Должность")]
        public Position? Position { get; set; }
        public ICollection<AcceptedRecyclable> AcceptedRecyclables { get; set; }
        public Employee()
        {
            AcceptedRecyclables = new List<AcceptedRecyclable>();
        }
    }
}