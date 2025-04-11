using System;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace studentoo
{
    public partial class loginPage : Page
    {
        public int LoggedInUserId { get; private set; }
        public bool IsLoggedIn { get; private set; }

        public loginPage()
        {
            InitializeComponent();
            SnackbarService.Initialize(SnackbarContainer, SnackbarMessage);
        }
        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                button.RenderTransform = new ScaleTransform(0.95, 0.95);
            }
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                button.RenderTransform = null;
            }
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
                            App.LoggedInUser = user;

                            if (App.MainF != null)
                            {
                                var mw = (MainWindow)Application.Current.MainWindow;
                                mw.UpdateLoginStateUI();
                                App.MainF.Navigate(new HomePage());
                            }

                        }
                        else
                        {
                            SnackbarService.Show("Nieprawidłowe hasło");
                        }
                    }
                    else
                    {
                        SnackbarService.Show("Użytkownik o podanym loginie nie istnieje");
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
