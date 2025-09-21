using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaDaa.EntityLayer.Concrete;
using TaDaa.WebUI.ViewModels;

namespace TaDaa.WebUI.Controllers
{
    public class ProfileController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;

        public ProfileController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);

            if (result.Succeeded)
            {
                // Şifre değişince cookie refresh yapman lazım yoksa login state bozulur
                await signInManager.RefreshSignInAsync(user);

                ViewBag.SuccessMessage = "Şifre başarıyla güncellendi.";
                return View();
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }


        public IActionResult Settings()
        {
            return View();
        }



    }
}
