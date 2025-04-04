
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace studentoo
{
    /// <summary>
    /// Logika interakcji dla klasy loginPage.xaml
    /// </summary>
    public partial class loginPage : Window
    {
        public loginPage()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = txtLogin.Text;
            var password = txtHaslo.Password;
            try
            {
                using (UserDataContext context = new UserDataContext())
                {
                    // Hashowanie hasła przed porównaniem (np. SHA256)
                    string hashedPassword = password;

                    bool userFound = context.Users.AsEnumerable()
                        .Any(user =>user.login == login && user.password_hash == hashedPassword);

                    if (userFound)
                    {
                        grantAccess();
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Nieprawidłowy login lub hasło");
                    }
                }
            }
            catch(SqlException ex)
{
                MessageBox.Show($"Błąd bazy danych: {ex.Message}");
            }
            catch (Exception ex)
{
                MessageBox.Show($"Nieoczekiwany błąd: {ex.Message}");
            }
        }
        
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        public void grantAccess()
        {
            MainWindow main = new MainWindow();
            main.Show();
        }
    }
}
