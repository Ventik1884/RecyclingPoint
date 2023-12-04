using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.Users
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; }

        [Display(Name = "Роль")]
        public string UserRole { get; set; }
        public EditUserViewModel()
        {
            UserRole = "user";
        }
    }
}
