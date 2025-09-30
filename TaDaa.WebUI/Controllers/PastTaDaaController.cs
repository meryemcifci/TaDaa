using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaDaa.DataAccessLayer.Concrete.Context;
using TaDaa.EntityLayer.Concrete;
using TaDaa.WebUI.ViewModels;

namespace TaDaa.WebUI.Controllers
{
    
    public class PastTaDaaController : Controller
    {
        private readonly Context _context;
        private readonly UserManager<AppUser> _userManager;

        public PastTaDaaController(Context context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendarData(int year, int month)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString))
                {
                    return Json(new { error = "Kullanıcı bulunamadı" });
                }

                var userId = int.Parse(userIdString);

                // Ayın başlangıç ve bitiş tarihlerini belirle
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                // O ay için tüm TaDaa'ları ve Emojileri getir
                var taskEntries = await _context.TaskEntry
                    .Where(t => t.UserId == userId &&
                           t.Date >= startDate &&
                           t.Date <= endDate)
                    .OrderBy(t => t.Date)
                    .ToListAsync();

                var dailyEmojis = await _context.DailyEmoji
                    .Where(e => e.UserId == userId &&
                           e.CreatedAt >= startDate &&
                           e.CreatedAt <= endDate)
                    .ToListAsync();

                // Günlere göre grupla
                var groupedData = taskEntries
                    .GroupBy(t => t.Date.Date)
                    .Select(g => new
                    {
                        date = g.Key.ToString("yyyy-MM-dd"),
                        hasData = true
                    })
                    .ToList();

                return Json(groupedData);
            }
            catch (Exception ex)
            {
                // Loglama için
                Console.WriteLine($"GetCalendarData Error: {ex.Message}");
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDayDetail(string date)
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdString))
                {
                    return Json(new { hasData = false, error = "Kullanıcı bulunamadı" });
                }

                var userId = int.Parse(userIdString);
                var selectedDate = DateTime.Parse(date);

                // O gün için emoji
                var emoji = await _context.DailyEmoji
                    .Where(e => e.UserId == userId && e.CreatedAt.Date == selectedDate.Date)
                    .Select(e => e.Emoji)
                    .FirstOrDefaultAsync();

                // O gün için tüm görevler
                var tasks = await _context.TaskEntry
                    .Where(t => t.UserId == userId && t.Date.Date == selectedDate.Date)
                    .Select(t => new TaskEntryViewModel
                    {
                        TaskDescription = t.TaskDescription,
                        Rating = t.Rating,
                        MoodNote = t.MoodNote,
                        IsCompleted = true
                    })
                    .ToListAsync();

                if (!tasks.Any() && string.IsNullOrEmpty(emoji))
                {
                    return Json(new { hasData = false });
                }

                // Ortalama rating hesapla
                var avgRating = tasks.Any() ? tasks.Average(t => t.Rating) : 0;

                // Günün adını Türkçe olarak al
                var dayName = GetTurkishDayName(selectedDate);

                var result = new DayDetailViewModel
                {
                    Date = selectedDate,
                    DayName = dayName,
                    Emoji = emoji ?? "📅",
                    Tasks = tasks,
                    AverageRating = Math.Round(avgRating, 1),
                    HasData = true
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                // Loglama için
                Console.WriteLine($"GetDayDetail Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return Json(new { hasData = false, error = ex.Message });
            }
        }

        private string GetTurkishDayName(DateTime date)
        {
            return date.DayOfWeek switch
            {
                DayOfWeek.Monday => "Pazartesi",
                DayOfWeek.Tuesday => "Salı",
                DayOfWeek.Wednesday => "Çarşamba",
                DayOfWeek.Thursday => "Perşembe",
                DayOfWeek.Friday => "Cuma",
                DayOfWeek.Saturday => "Cumartesi",
                DayOfWeek.Sunday => "Pazar",
                _ => ""
            };
        }

        [HttpGet]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userId = int.Parse(userIdString ?? "0");

                var taskCount = await _context.TaskEntry.CountAsync();
                var emojiCount = await _context.DailyEmoji.CountAsync();
                var userTaskCount = await _context.TaskEntry.Where(t => t.UserId == userId).CountAsync();

                return Json(new
                {
                    userId = userId,
                    isAuthenticated = User.Identity.IsAuthenticated,
                    totalTasks = taskCount,
                    totalEmojis = emojiCount,
                    userTasks = userTaskCount,
                    message = "Bağlantı başarılı!"
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }

}
