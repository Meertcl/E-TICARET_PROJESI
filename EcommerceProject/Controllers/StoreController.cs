using ECommerceProject.Models;
using ECommerceProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ECommerceProject.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDBContext context;
        private readonly int pageSize = 8;
        public StoreController(ApplicationDBContext context)
        {
            this.context=context;
        }
        public IActionResult Index(int pageIndex, string? search, string? brand, string? category, string? sort)
        {
            IQueryable<Product> query = context.Products;
            if(search!=null && search.Length>0)
            {
                query =query.Where(p=>p.Name.Contains(search));
            }

            //filter funcionality

            if(brand!=null && brand.Length>0)
            {
                query = query.Where(p=>p.Brand.Contains(brand));
            }
            if(category!=null && category.Length > 0)
            {
                query = query.Where(p=>p.Category.Contains(category));
            }

            if (string.Equals(sort, "price_asc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderBy(p => p.Price);
            }
            else if (string.Equals(sort, "price_desc", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderByDescending(p => p.Price);
            }
            else
            {
                // Newest products first
                query = query.OrderByDescending(p => p.Id);
            }


            //pagination functionality

            if (pageIndex < 1)
            {
                pageIndex = 1;  
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query =query.Skip((pageIndex-1) * pageSize).Take(pageSize);


            var products = query.ToList(); 

            ViewBag.Products = products;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageIndex = pageIndex;

            var storeSearchModel = new storeSearchModel()
            {
                Search = search,
                Brand = brand,
                Category = category,
                Sort = sort
            };

            return View(storeSearchModel);
        }
        public IActionResult Details(int id)
        {
            var product = context.Products.Find(id);
            if(product == null)
            {
                return RedirectToAction("Index","Store");
            }
            return View(product);
        }
    }
}
