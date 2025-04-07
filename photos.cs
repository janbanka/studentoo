using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace studentoo
{
    public class photos
    {
        [Key]
        public int id { get; set; }
        public int user_id { get; set; }
        public byte[] photo_data { get; set; }

        public DateTime uploaded_at { get; set; } = DateTime.Now;

    }
}
