using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaDaa.EntityLayer.Concrete
{
    public class AppUser:IdentityUser<int>
    {

        public TimeSpan? ReminderTime { get; set; }

        public ICollection<TaskEntry> TaskEntries { get; set; }
        public ICollection<DailyEmoji> DailyEmojis { get; set; }
    }
}
