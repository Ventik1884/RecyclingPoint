using System;
using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.Users
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [EmailAddress(ErrorMessage = "Некорректный адрес")]
        public string Email { get; set; }
        [Display(Name = "Роль")]
        public string RoleName { get; set; }

    }
}
