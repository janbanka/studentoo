using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace studentoo
{
    /// <summary>
    /// Logika interakcji dla klasy Rejestracja1.xaml
    /// </summary>
    public partial class Rejestracja1 : Page
    {
        public Rejestracja1()
        {
            InitializeComponent();
        }
        private void btnRejestracja_Click(object sender, RoutedEventArgs e)
        {
            string name = txtImie.Text.Trim();
            string surname = txtNazwisko.Text.Trim();
            string login = txtLogin.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtHaslo.Password;
            string ageText = txtDataUro.Text.Trim();
            string Opis="Brak opisu";
            if (txtHaslo.Password != txtPowHaslo.Password)
            {
                MessageBox.Show("Hasła muszą być identyczne!!!");
            }
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname) ||
                string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(ageText))
            {
                MessageBox.Show("Uzupełnij wszystkie pola.");
                return;
            }

            if (!int.TryParse(ageText, out int age))
            {
                MessageBox.Show("Wiek musi być liczbą.");
                return;
            }

            using (var db = new UserDataContext())
            {
                bool loginExists = db.Users.Any(u => u.login == login);
                bool emailExists = db.Users.Any(u => u.email == email);

                if (loginExists)
                {
                    MessageBox.Show("Login już istnieje.");
                    return;
                }

                if (emailExists)
                {
                    MessageBox.Show("Email już istnieje.");
                    return;
                }

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                var newUser = new User
                {
                    name = name,
                    surname = surname,
                    login = login,
                    email = email,
                    password_hash = passwordHash,
                    age = age,
                    description=Opis,
                    created_at = DateTime.Now
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                MessageBox.Show("Rejestracja zakończona sukcesem.");
                if (NavigationService != null)
                {
                    NavigationService.Navigate(new loginPage());
                }
                // Opcja 2: Otwórz loginPage jako Window (jeśli to Window)
                else
                {
                    new loginPage().Show();
                    Window.GetWindow(this)?.Close(); // Zamknij bieżące okno
                }
            }
        }

    }
}
