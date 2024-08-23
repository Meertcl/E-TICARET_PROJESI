using ECommerceProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ECommerceProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> SignInManager)
        {
            this.userManager = userManager;
            this.signInManager = SignInManager;
        }
        public IActionResult Register()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {


            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }



            if (!ModelState.IsValid)
            {
                return View(registerDto);
            }

            var user = new ApplicationUser()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.EMail,
                Email = registerDto.EMail,
                PhoneNumber = registerDto.PhoneNumber,
                Address = registerDto.Address,
                CreatedAt = DateTime.Now,
            };

            var result = await userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "client");
                await signInManager.SignInAsync(user, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(registerDto);
        }

        public async Task<IActionResult> Logout()
        {
            if (signInManager.IsSignedIn(User))
            {
                await signInManager.SignOutAsync();
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Login()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!ModelState.IsValid)
            {
                return View(loginDto);
            }
            var result = await signInManager.PasswordSignInAsync(loginDto.EMail, loginDto.Password, loginDto.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid login attempt";
            }
            return View(loginDto);
        }


        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var appUser = await userManager.GetUserAsync(User);

            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var profileDto = new ProfileDto()
            {
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                EMail = appUser.Email ?? "",
                PhoneNumber = appUser.PhoneNumber,
                Address = appUser.Address,
            };

            return View(profileDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileDto profileDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please fill all the required fields with valid values";
                return View(profileDto);
            }

            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            appUser.FirstName = profileDto.FirstName;
            appUser.LastName = profileDto.LastName;
            appUser.UserName = profileDto.EMail;
            appUser.Email = profileDto.EMail;
            appUser.PhoneNumber = profileDto.PhoneNumber;
            appUser.Address = profileDto.Address;

            // Burada CreateAsync yerine UpdateAsync kullanılıyor
            var result = await userManager.UpdateAsync(appUser);

            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Profile updated successfully";
            }
            else
            {
                ViewBag.ErrorMessage = "Unable to update the profile: " + result.Errors.First().Description;
            }

            return View(profileDto);
        }


        [Authorize]
        public IActionResult Password()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Password(PasswordDto passwordDto)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var appUser = await userManager.GetUserAsync(User);
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var result = await userManager.ChangePasswordAsync(appUser, passwordDto.CurrentPassword, passwordDto.NewPassword);

            if (result.Succeeded)
            {
                ViewBag.SuccessMessage = "Password updated successfully";
            }
            else
            {
                ViewBag.ErrorMessage = "Error:" + result.Errors.First().Description;
            }


            return View();
        }
        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "Home");

        }

        public IActionResult ForgotPassword()
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");

            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ForgotPassword([Required, EmailAddress] string email)
        {
            if (signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");

            }

            ViewBag.Email = email;  

            if(!ModelState.IsValid)
            {
                ViewBag.EmailError = ModelState["email"]?.Errors.First().ErrorMessage ?? "Invalid Email Address";
                return View();
            }

            var user = await userManager.FindByEmailAsync(email);

            if(user != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                string resetUrl = Url.ActionLink("ResetPassword", "Account", new { token }) ?? "Url Error";
                
                Console.WriteLine("Password Reset Link :" + resetUrl);
            }

            ViewBag.SuccessMessage = "Please check your Email account and click on the password reset link";

            return View();
        }
    }
}
