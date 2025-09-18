using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaDaa.EntityLayer.Concrete
    //Bugünün TaDaa'ları
{
    public class TaskEntry: BaseEntity<int>
    {
        [Key]
        public int TaskEntryId { get; set; }

        [Required]
        [MaxLength(500)] 
        public string TaskDescription { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Range(1, 5)] 
        public int Rating { get; set; } 

        [MaxLength(255)] 
        public string MoodNote { get; set; }
        
        [ForeignKey("DailySummary")]
        public int DailySummaryId { get; set; }
        public DailySummary DailySummary { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public AppUser User { get; set; }
    }
}
