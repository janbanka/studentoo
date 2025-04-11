using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
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
        public virtual ICollection<photos> zdj { get; set; } = new List<photos>();
        [NotMapped]
        public BitmapImage PhotoImage
        {
            get
            {
                try
                {
                    // Sprawdź czy zdjęcia są załadowane
                    if (zdj == null || zdj.Count == 0)
                        return LoadDefaultImage();

                    // Weź pierwsze zdjęcie z niepustymi danymi
                    var firstPhoto = zdj.FirstOrDefault(p => p?.photo_data?.Length > 0);
                    if (firstPhoto == null)
                        return LoadDefaultImage();

                    // Utwórz obrazek z danych binarnych
                    return ByteArrayToBitmapImage(firstPhoto.photo_data);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Błąd ładowania zdjęcia: {ex.Message}");
                    return LoadDefaultImage();
                }
            }
        }

        private BitmapImage ByteArrayToBitmapImage(byte[] imageData)
        {
            var image = new BitmapImage();
            using (var ms = new MemoryStream(imageData))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
            }
            image.Freeze();
            return image;
        }

        private BitmapImage LoadDefaultImage()
        {


            var uri = new Uri("pack://application:,,,/assets/defaultImage.jpg");
            var image = new BitmapImage(uri);
            image.Freeze();
            return image;

        }

    }
}