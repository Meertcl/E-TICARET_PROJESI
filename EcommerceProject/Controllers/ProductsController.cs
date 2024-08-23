using ECommerceProject.Models;
using ECommerceProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ECommerceProject.Controllers
{
    [Authorize (Roles ="admin")]
    [Route("/Admin/[controller]/{action=Index}/{id?}")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDBContext context;
        private readonly IWebHostEnvironment environment;
        private readonly int PageSize =5;
        private object validOrderBy;

        public ProductsController(ApplicationDBContext context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        public IActionResult Index(int pageIndex, string? search, string? column, string? orderBy)
        {
            IQueryable<Product> query = context.Products;


            if(search != null)
            {
                query =query.Where(p=>p.Name.Contains(search) || p.Brand.Contains(search));
            }

            //sıralama fonksiyonu
            string[] validColumns = { "Id", "Name", "Brand", "Category", "Price", "CreatedAT" };
            string[] ValidOrderBy = { "desc", "asc" };


            if (!validColumns.Contains(column))
            {
                column = "Id";
            }

            if (!ValidOrderBy.Contains(orderBy))
            {
                orderBy = "desc";
            }

            if (column == "Name")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Name);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Name);
                }
            }
            else if (column == "Brand")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Brand);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Brand);
                }
            }
            else if (column == "Category")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Category);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Category);
                }
            }
            else if (column == "Price")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Price);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Price);
                }
            }
            else if (column == "CreatedAT")
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.CreatedAT);
                }
                else
                {
                    query = query.OrderByDescending(p => p.CreatedAT);
                }
            }
            else
            {
                if (orderBy == "asc")
                {
                    query = query.OrderBy(p => p.Id);
                }
                else
                {
                    query = query.OrderByDescending(p => p.Id);
                }
            }



            // query = query.OrderByDescending(p=>p.Id);

            //sayfalama işlemleri

            if (pageIndex<1)
            {
                pageIndex = 1;
            }

            decimal count = query.Count();
            int totalPages =(int)Math.Ceiling(count / PageSize);
            query = query.Skip((pageIndex - 1 ) * PageSize).Take(PageSize);

            var products = query.ToList();

            ViewData["PageIndex"] = pageIndex;
            ViewData["TotalPages"] = totalPages;

            ViewData["Search"] = search ?? "";

            ViewData["Column"] = column;
            ViewData["OrderBy"] = orderBy;
            
            return View(products);
        }

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public IActionResult Create(ProductDto productDto)
        {
            //ÜRÜN EKLEME İŞLEMLERİ
            if (productDto.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile","the image file is required");

            }
            if (!ModelState.IsValid)
            {
                return View(productDto);
            }

            //RESİMİ KAYDETME İŞLEMLERİ 
            string NewFileName = DateTime.Now.ToString("yyyyMMddHHmmssff");
            NewFileName += Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/products/" + NewFileName;
            using (var stream = System.IO.File.Create(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }
            //ÜRÜNÜ VERİ TABANINA KAYDETME İŞLEMLERİ

            Product product = new Product()
            {
                Name = productDto.Name,
                Brand = productDto.Brand,
                Price = productDto.Price,
                Category = productDto.Category,
                Description = productDto.Description,
                ImageFileName = NewFileName,
                CreatedAT = DateTime.Now,

            };
            context.Products.Add(product);
            context.SaveChanges();

            return RedirectToAction("Index","Products");
        }

        public IActionResult Edit(int id)
        {
            var product = context.Products.Find(id);

            if (product == null)
            {
                RedirectToAction("Index", "Products");
            }
            var ProducDto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Description = product.Description,
                
            };
            ViewData["ProductID"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAT"] = product.CreatedAT.ToString("MM/dd/yyyy");

            return View(ProducDto);

        }

        [HttpPost]
        public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = context.Products.Find(id);
            if(product == null)
            {
                RedirectToAction("Index","Products");
            }

            if(!ModelState.IsValid)
            {
                ViewData["ProductID"] = product.Id;
                ViewData["ImageFileName"] = product.ImageFileName;
                ViewData["CreatedAT"] = product.CreatedAT.ToString("MM/dd/yyyy");

                return View(productDto);

            }

            //YENİ BİR RESİM DAHA EKLENDİYSE YAPILMASI GEREKENLER

            string newFileName = product.ImageFileName;
            if(productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);

                string imageFullPath = environment.WebRootPath + "/products/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                //ESKİ RESİMİ SİSTEMDEN SİLME
                string OldImageFullPath = environment.WebRootPath + "/products" + product.ImageFileName;
                System.IO.File.Delete(OldImageFullPath);
            }

            //veri tabanını güncellemek 
            product.Name = productDto.Name;
            product.Brand = productDto.Brand;
            product.Price = productDto.Price;
            product.Category = productDto.Category;
            product.Description = productDto.Description;
            product.ImageFileName = newFileName;

            context .SaveChanges();
            return RedirectToAction("Index","Products");
        }

        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index","Products");
            }
            string imageFullPath = environment.WebRootPath + "/products/" + product.ImageFileName;
            System.IO.File.Delete(imageFullPath);
            context.Products.Remove(product);
            context.SaveChanges(true);
            return RedirectToAction("Index","Products");
        }

    }
}
