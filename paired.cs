﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace studentoo
{
    public class paired
    {
        [Key]
        public int id { get; set; }
        public int user_id { get; set; }
        public int user_id2 { get; set; }
        public bool is_like { get; set; }

        public DateTime timestamp { get; set; }
        public bool is_matched { get; set; }

        [ForeignKey("user_id")]
        public virtual User User1 { get; set; }

        [ForeignKey("user_id2")]
        public virtual User User2 { get; set; }

       


    }
}
