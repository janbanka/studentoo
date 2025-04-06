using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace studentoo
{
    public partial class loginPage : Page
    {
        public int LoggedInUserId { get; private set; }
        public bool IsLoggedIn { get; private set; }

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
                    var user = context.Users.FirstOrDefault(u => u.login == login);

                    if (user != null)
                    {
                        if (BCrypt.Net.BCrypt.Verify(password, user.password_hash))
                        {
                            IsLoggedIn = true;
                            LoggedInUserId = user.id;
                            var mainWindow = new MainWindow(LoggedInUserId);
                            mainWindow.Show();

                            Window.GetWindow(this)?.Close();

                        }
                        else
                        {
                            MessageBox.Show("Nieprawidłowe hasło");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Użytkownik o podanym loginie nie istnieje");
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

        private void btnRejestracja_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService?.Navigate(new Rejestracja1());

        }
    }
}
