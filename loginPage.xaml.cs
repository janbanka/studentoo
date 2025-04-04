
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

                    bool userFound = context.Users
                    .Any(user => user.login == login);

                    if (userFound)
                    {
                        var user = context.Users.First(u => u.login == login);
                        if (BCrypt.Net.BCrypt.Verify(password, user.password_hash))
                        {
                            grantAccess();
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("Nieprawidłowe hasło");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Błąd bazy danych: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd: {ex.Message}");
            }
        }
        // Przykładowa metoda hashująca (w rzeczywistości użyj biblioteki np. BCrypt.Net)
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
        public void grantAccess()
        {
            MainWindow main = new MainWindow();
            main.Show();
        }
    }
}
