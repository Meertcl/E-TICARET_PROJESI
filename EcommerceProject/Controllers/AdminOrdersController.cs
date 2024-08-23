using ECommerceProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Core;
using NuGet.Protocol.Plugins;

namespace ECommerceProject.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("/Admin/Orders/{action=Index}/{id?}")]
    public class AdminOrdersController : Controller
    {
        private readonly ApplicationDBContext Context;
        public AdminOrdersController(ApplicationDBContext context)
        {
            this.Context = context;
        }

        public IActionResult Index()
        {
            var orders = Context.Orders.Include(x=>x.Client).Include(x=>x.Items).OrderByDescending(x=>x.Id).ToList();
            ViewBag.Orders = orders; 

            return View();
        }

        public IActionResult Details(int id)
        {
            var order = Context.Orders.Include(x => x.Client).Include(x => x.Items).ThenInclude(oi=>oi.Product).FirstOrDefault(x=>x.Id==id);

            if(order == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.NumOrders = Context.Orders.Where(x=>x.ClientId==order.ClientId).Count();

            return View(order);
        }

        public IActionResult Edit(int id, string? payment_status, string? order_status)
        {
            var order = Context.Orders.Find(id);
            if (order == null)
            {
                return RedirectToAction("Index");
            }


            if (payment_status == null && order_status == null)
            {
                return RedirectToAction("Details", new { id });
            }

            if (payment_status != null)
            {
                order.PaymentStatus = payment_status;
            }

            if (order_status != null)
            {
                order.OrderStatus = order_status;
            }

            Context.SaveChanges();


            return RedirectToAction("Details", new { id });
        }
    }
}
