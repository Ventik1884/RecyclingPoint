using System.ComponentModel.DataAnnotations;
namespace WebApp.Models
{
    public class Position
    {
        [Key]
        [Display(Name = "Код должности")]
        public int Id { get; set; }
        [Required(ErrorMessage = "Не указана должность")]
        [Display(Name = "Должность")]
        public string Name { get; set; }
        public ICollection<Employee> Employees { get; set; }
        public Position()
        {
            Employees = new List<Employee>();
        }
        public override string ToString()
        {
            return "{ Код_должности = " + "Id" + ", Название = " + "Name" + "}";
        }
    }
}