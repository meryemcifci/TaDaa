using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaDaa.EntityLayer.Concrete
{
    public class BaseEntity<T>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        public DateTime? LastModifiedAt { get; set; } 
        public bool IsDeleted { get; set; } = false; 
    }
}
