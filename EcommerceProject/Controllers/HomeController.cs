using ECommerceProject.Models;
using ECommerceProject.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ECommerceProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDBContext Context;

        public HomeController(ApplicationDBContext context)
        {
            this.Context = context; 
        }

        public IActionResult Index()
        {
            var products = Context.Products.OrderByDescending(p => p.Id).Take(4).ToList();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
