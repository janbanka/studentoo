using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace studentoo
{
    public class messages
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int chat_id { get; set; }

        [Required]
        public int sender_id { get; set; }

       
       

        [Required]
        [MaxLength(1000)]
        public string content { get; set; }

        public DateTime sent_at { get; set; } = DateTime.Now;
        [NotMapped]
        public bool is_read { get; set; } = false;
        [NotMapped]
        public int receiver_id { get; set; }
        [NotMapped]
        public virtual User Sender { get; set; }

        [NotMapped]
        public virtual User Receiver { get; set; }
        [NotMapped]
        public bool IsCurrentUser { get; set; }
        [NotMapped]
        public string FormattedTime => sent_at.ToString("HH:mm");
        [NotMapped]
        public virtual chats chat { get; set; }

    }
}
