using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace studentoo
{
    class Pairs
    {
        [Key]
        public int PairID { get; set; }
        public int userID1 { get; set; }
        public int userID2 { get; set; }
    }
}
