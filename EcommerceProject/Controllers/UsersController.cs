using ECommerceProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerceProject.Controllers
{
    [Authorize(Roles ="admin")]
    [Route("/Admin/[Controller]/{action=Index}/{id?}")]
    public class UsersController : Controller
    {

        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly int pageSize = 5;


        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public IActionResult Index(int? pageIndex)
        {
            IQueryable<ApplicationUser> query = userManager.Users.OrderByDescending(x=>x.CreatedAt);

            if(pageIndex == null || pageIndex<1)
            {
                pageIndex= 1;
            }

            decimal count = query.Count();
            int totalPages = (int)Math.Ceiling(count / pageSize);
            query =query.Skip(((int)pageIndex-1)*pageSize).Take(pageSize);

            var users = query.ToList();

            ViewBag.pageIndex = pageIndex;  
            ViewBag.totalPages = totalPages;
            return View(users);
        }

        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Users");
            }

            var appUser = await userManager.FindByIdAsync(id);

            if(appUser == null)
            {
                return RedirectToAction("Index", "Users");
            }

            ViewBag.Roles = await userManager.GetRolesAsync(appUser);

            var avaibleRoles = roleManager.Roles.ToList();
            var items = new List<SelectListItem>();

            foreach (var role in avaibleRoles)
            {
                items.Add(
                    new SelectListItem
                    {
                        Text = role.NormalizedName,
                        Value = role.Name,
                        Selected = await userManager.IsInRoleAsync(appUser, role.Name!),
                    });
            }

            ViewBag.SelectItems = items;

            return View(appUser);
        }

        public async Task<IActionResult> EditRole(string? id, string? newRole)
        {
            if (id == null || newRole == null)
            {
                return RedirectToAction("Index", "Users");
            }

            var roleExists = await roleManager.RoleExistsAsync(newRole);
            var appUser = await userManager.FindByIdAsync(id);

            if (appUser == null || !roleExists)
            {
                return RedirectToAction("Index", "Users");
            }

            var currentUser = await userManager.GetUserAsync(User);

            if (currentUser!.Id == appUser.Id)
            {
                TempData["ErrorMessage"] = "you can't update your own role!";
                return RedirectToAction("Details", "Users", new { id });
            }

            var userRoles = await userManager.GetRolesAsync(appUser);
            await userManager.RemoveFromRolesAsync(appUser, userRoles); 
            await userManager.AddToRoleAsync(appUser, newRole);


            TempData["SuccessMessage"] = "User Role updated Successfully";
            return RedirectToAction("Details", "Users", new { id });
        }

        public async Task<IActionResult> DeleteAccount(string? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Users");
            }

            var appUser = await userManager.FindByIdAsync(id);

            if (appUser == null)
            {
                return RedirectToAction("Index", "Users");
            }
            
            var currentUser = await userManager.GetUserAsync (User);

            if(currentUser!.Id == appUser.Id)
            {
                TempData["ErrorMessage"] = "You cannot delete your own account!";
                return RedirectToAction("Details", "Users", new { id });
            }
            var result = await userManager.DeleteAsync(appUser);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Users");
            }

            TempData["ErrorMessage"] = "Unable to delete this account:" + result.Errors.First().Description;
            return RedirectToAction("Details", "Users", new { id });
        }
    }
}
