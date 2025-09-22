using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Reflection.Metadata;
using TaDaa.DataAccessLayer.Concrete;
using TaDaa.EntityLayer.Concrete;
using TaDaa.WebUI.ViewModels;

namespace TaDaa.WebUI.Controllers
{
   
    public class ProfileController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;
        private readonly Context _context;

        public ProfileController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, Context context)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            _context = context;
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

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var user = await userManager.GetUserAsync(User);

            var model = new SettingsViewModel
            {
                Username = user.UserName,
                ReminderTime = user.ReminderTime
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSettings(SettingsViewModel model)
        {
            var user = await userManager.GetUserAsync(User);

            if (user != null)
            {                
                user.ReminderTime = model.ReminderTime;
                var result = await userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Hatirlaticiniz kaydedildi. ";
                }
            }

            return RedirectToAction("Settings");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                await signInManager.SignOutAsync();
                await userManager.DeleteAsync(user);
            }

            return RedirectToAction("Settings", "Profile");
        }
        [HttpGet]
        public async Task<IActionResult> DownloadPdf()
        {
            // Lisansı ayarla
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            var userId = int.Parse(userManager.GetUserId(User));
            var user = await _context.Users
                .Include(u => u.TaskEntries)
                .Include(u => u.DailyEmojis)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound();

            var pdfBytes = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    page.Header().AlignCenter().Column(col =>
                    {
                        // Logo
                        col.Item().Width(60).Height(60)
                        .Image("wwwroot/images/TaDaa_Logo_Ampul.png", ImageScaling.FitArea);


                        // Başlık
                        col.Item().Text("Kullanıcı Verileri")
                               .FontSize(18)
                               .Bold();
                           
                    });


                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Email: {user.UserName ?? "-"}");
                        string reminderText;

                        if (user.ReminderTime.HasValue)
                        {
                            var ts = user.ReminderTime.Value;
                            reminderText = $"{ts.Hours:D2}:{ts.Minutes:D2}"; // HH:mm format
                        }
                        else
                        {
                            reminderText = "Hatırlatma oluşturulmamıştır.";
                        }

                        col.Item().Text($"Hatırlatma Saati: {reminderText}");
                        col.Item().Text("Emojiler:");
                        if (user.DailyEmojis.Any())
                        {
                            foreach (var emoji in user.DailyEmojis)
                                col.Item().Text($"• {emoji.Emoji}");
                        }
                        else col.Item().Text("• Yok");

                        col.Item().Text("TaDaalarım:");
                      
                        if (user.TaskEntries.Any())
                        {
                            foreach (var task in user.TaskEntries)
                                col.Item().Text($"• [{task.Date:yyyy-MM-dd}] {task.TaskDescription}");
                        }
                        else col.Item().Text("• Yok");
                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", "TaDaa_DaaTa.pdf");
        }

    }
}
