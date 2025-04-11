using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace studentoo
{
    public class photos
    {
        [Key]
        public int id { get; set; }
        public int user_id { get; set; }
        public byte[] photo_data { get; set; }

        public DateTime uploaded_at { get; set; } = DateTime.Now;
        
        [ForeignKey("user_id")]
        public virtual User User { get; set; }
    }
}
