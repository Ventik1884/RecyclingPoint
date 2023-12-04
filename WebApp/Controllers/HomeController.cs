using Microsoft.AspNetCore.Mvc;
using WebApp.Data;
using WebApp.Models;
using System.Diagnostics;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly RecPointContext _db;
        public HomeController(RecPointContext db)
        {
            _db = db;
        }

        //[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 294)]
        public IActionResult Index()
        {
            HomeViewModel homeViewModel = new HomeViewModel();
            return View(homeViewModel);
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}