using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

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
        public string plec { get; set; }
        [NotMapped]
        public List<photos> zdj { get; set; }

        [NotMapped]
        public BitmapImage PhotoImage
        {
            get
            {
                if (zdj == null || zdj.Count == 0)
                    return GetDefaultImage();

                try
                {
                    var firstPhoto = zdj.FirstOrDefault();
                    if (firstPhoto?.photo_data == null || firstPhoto.photo_data.Length == 0)
                        return GetDefaultImage();

                    var image = new BitmapImage();
                    using (var ms = new MemoryStream(firstPhoto.photo_data))
                    {
                        image.BeginInit();
                        image.CacheOption = BitmapCacheOption.OnLoad;
                        image.StreamSource = ms;
                        image.EndInit();
                    }
                    image.Freeze();
                    return image;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading image: {ex.Message}");
                    return GetDefaultImage();
                }
            }
        }

        private BitmapImage GetDefaultImage()
        {
            var defaultImage = new BitmapImage();
            defaultImage.BeginInit();
            defaultImage.UriSource = new Uri("pack://application:,,,/assets/defaultImage.jpg");
            defaultImage.CacheOption = BitmapCacheOption.OnLoad;
            defaultImage.EndInit();
            defaultImage.Freeze();
            return defaultImage;
        }

    }
}