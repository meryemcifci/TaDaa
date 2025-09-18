using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaDaa.EntityLayer.Concrete;

namespace TaDaa.DataAccessLayer.Concrete
{
    public class Context : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<TaskEntry> TaskEntry { get; set; }
        public DbSet<DailyEmoji> DailyEmoji { get; set; }


    }
}
