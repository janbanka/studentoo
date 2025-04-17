using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studentoo
{
    public class chats
    {
        [Key]
        public int id { get; set; }
    
        [Required]
        public int paired_id { get; set; }
        [Required]
        public DateTime created_at { get; set; } = DateTime.Now;

        public virtual chats chat { get; set; }
        
    }
}
