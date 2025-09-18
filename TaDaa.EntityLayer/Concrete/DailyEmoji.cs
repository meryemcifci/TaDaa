using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaDaa.EntityLayer.Concrete
{
    public class DailyEmoji: BaseEntity<int>
    {
        [Key]
        public int EmojiId { get; set; }

        [MaxLength(200)]
        public string Emoji { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public AppUser User { get; set; }
    }
}
