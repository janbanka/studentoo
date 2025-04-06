using System;
using System.Data.SqlClient;
using System.Windows;

namespace studentoo
{
    public partial class loginPage : Window
    {
        // Publiczna właściwość do przechowywania ID zalogowanego użytkownika
        public int LoggedInUserId { get; private set; }

        public loginPage()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = txtLogin.Text;
            var password = txtHaslo.Password;

            try
            {
                using (UserDataContext context = new UserDataContext())
                {
                    bool userFound = context.Users.Any(user => user.login == login);

                    if (userFound)
                    {
                        var user = context.Users.First(u => u.login == login);
                        if (BCrypt.Net.BCrypt.Verify(password, user.password_hash))
                        {
                            
                            LoggedInUserId = user.id;

                            
                            var mainWindow = new MainWindow(LoggedInUserId);
                            mainWindow.Show();

                            this.Close();
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
            var registrationPage = new Rejestracja1();
            var registrationWindow = new Window
            {
                Title = "Rejestracja",
                Content = registrationPage,
                Width = 600,
                Height = 500,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            registrationWindow.Show();
            this.Close();
        }
    }
}