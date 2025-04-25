using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [ForeignKey("paired_id")]
        public virtual paired Pair { get; set; }

    }
}
