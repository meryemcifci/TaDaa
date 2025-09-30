using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaDaa.BusinessLayer.NewFolder;
using TaDaa.DataAccessLayer.Concrete.Context;
using TaDaa.EntityLayer.Concrete;

namespace TaDaa.WebUI.Controllers
{
    public class TaskController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<AppUser> _userManager;

        public TaskController(Context context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var tasks = await _context.TaskEntry
                .Where(t => t.UserId == user.Id && t.Date.Date == DateTime.Today)
                .OrderByDescending(t => t.TaskEntryId)
                .ToListAsync();

            // Bugünkü emoji'yi al
            var todayEmoji = await _context.DailyEmoji
                .Where(e => e.UserId == user.Id && e.CreatedAt.Date == DateTime.Today)
                .Select(e => e.Emoji)
                .FirstOrDefaultAsync();

            ViewBag.TodayEmoji = todayEmoji;

            return View(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> AddTask(string TaskDescription, int Rating, string MoodNote = "")
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Kullanıcı bulunamadı";
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(TaskDescription))
            {
                TempData["Error"] = "Görev açıklaması boş olamaz";
                return RedirectToAction("Index");
            }

            if (Rating < 1 || Rating > 5)
            {
                TempData["Error"] = "Geçersiz puan değeri";
                return RedirectToAction("Index");
            }

            var task = new TaskEntry
            {
                TaskDescription = TaskDescription.Trim(),
                Date = DateTime.Today,
                Rating = Rating,
                MoodNote = MoodNote?.Trim() ?? "",
                UserId = user.Id, 
                CreatedAt = DateTime.Now,
                IsDeleted = false
            };

            try
            {
                _context.TaskEntry.Add(task);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Başarı kaydedildi! 🎉";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Kayıt sırasında hata oluştu: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRating(int taskId, int rating)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı" });
            }

            var task = await _context.TaskEntry
                .FirstOrDefaultAsync(t => t.TaskEntryId == taskId && t.UserId == user.Id);

            if (task == null)
            {
                return Json(new { success = false, message = "Görev bulunamadı" });
            }

            if (rating < 1 || rating > 5)
            {
                return Json(new { success = false, message = "Geçersiz rating değeri" });
            }

            task.Rating = rating;

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMood(int taskId, string moodNote)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı" });
            }

            var task = await _context.TaskEntry
                .FirstOrDefaultAsync(t => t.TaskEntryId == taskId && t.UserId == user.Id);

            if (task == null)
            {
                return Json(new { success = false, message = "Görev bulunamadı" });
            }

            task.MoodNote = moodNote ?? "";

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> SetDailyEmoji(string emoji)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new { success = false, message = "Kullanıcı bulunamadı" });
            if (string.IsNullOrEmpty(emoji)) return Json(new { success = false, message = "Emoji boş geldi" });

            var today = DateTime.Today;
            var existingEmoji = await _context.DailyEmoji
                .FirstOrDefaultAsync(e => e.UserId == user.Id && e.CreatedAt.Date == today);

            if (existingEmoji != null) existingEmoji.Emoji = emoji;
            else _context.DailyEmoji.Add(new DailyEmoji { Emoji = emoji, UserId = user.Id, CreatedAt = DateTime.Now, IsDeleted = false });

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Emoji kaydedildi!" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı" });
            }

            var task = await _context.TaskEntry
                .FirstOrDefaultAsync(t => t.TaskEntryId == taskId && t.UserId == user.Id);

            if (task == null)
            {
                return Json(new { success = false, message = "Görev bulunamadı" });
            }

            _context.TaskEntry.Remove(task); 
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        
    }
}