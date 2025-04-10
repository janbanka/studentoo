using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace studentoo
{
    public class paired
    {
        [Key]
        public int PairID { get; set; }
        public int user_id { get; set; }
        public int user_id2 { get; set; }
        public string Action {  get; set; }
        public DateTime timestamp { get; set; }

    }
}
