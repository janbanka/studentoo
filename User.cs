using System.ComponentModel.DataAnnotations;

namespace studentoo
{
    public class User
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string login { get; set; }
        public string email { get; set; }
        public string password_hash { get; set; }
        public int age { get; set; }
        public string description { get; set; } = "";
        public string interests { get; set; } = "";
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}