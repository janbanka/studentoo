using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace studentoo
{
    public partial class Rejestracja1 : Page
    {
        public Rejestracja1()
        {
            InitializeComponent();
            SnackbarService.Initialize(SnackbarContainer, SnackbarMessage);
            dtpDataUro.SelectedDateChanged += dtpDataUro_SelectedDateChanged;
        }

        private int age = 0; // Pole do przechowywania wieku

        private void btnRejestracja_Click(object sender, RoutedEventArgs e)
        {
            string name = txtImie.Text.Trim();
            string surname = txtNazwisko.Text.Trim();
            string plec = null;
            string login = txtLogin.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtHaslo.Password;
            string Opis = "Brak opisu";

            // Walidacja haseł
            if (txtHaslo.Password != txtPowHaslo.Password)
            {
                SnackbarService.Show("Hasła muszą być identyczne!!!");
                return;
            }

            // Walidacja płci
            if (rbMezczyzna.IsChecked == true)
            {
                plec = "M";
            }
            else if (rbKobieta.IsChecked == true)
            {
                plec = "K";
            }
            else
            {
                SnackbarService.Show("Wybierz płeć");
                return;
            }

            // Walidacja wieku
            if (dtpDataUro.SelectedDate == null)
            {
                SnackbarService.Show("Wybierz datę urodzenia");
                return;
            }

            if (!SprawdzWiek(dtpDataUro.SelectedDate.Value))
            {
                return;
            }

            // Walidacja pustych pól
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(surname) ||
                string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                SnackbarService.Show("Uzupełnij wszystkie pola.");
                return;
            }

            // Walidacja email
            if (!email.Contains("@") || !email.Contains("."))
            {
                SnackbarService.Show("Podaj poprawny adres email");
                return;
            }

            using (var db = new UserDataContext())
            {
                // Sprawdzenie unikalności loginu i emaila
                if (db.Users.Any(u => u.login == login))
                {
                    SnackbarService.Show("Login już istnieje.");
                    return;
                }

                if (db.Users.Any(u => u.email == email))
                {
                    SnackbarService.Show("Email już istnieje.");
                    return;
                }

                // Hashowanie hasła
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                // Tworzenie nowego użytkownika
                var newUser = new User
                {
                    name = name,
                    surname = surname,
                    login = login,
                    email = email,
                    password_hash = passwordHash,
                    age = this.age, // Używamy obliczonego wieku
                    description = Opis,
                    created_at = DateTime.Now,
                    plec = plec
                };

                db.Users.Add(newUser);
                db.SaveChanges();

                SnackbarService.Show("Rejestracja zakończona sukcesem!");
                PowrotDoLogowania();
            }
        }

        private void dtpDataUro_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dtpDataUro.SelectedDate.HasValue)
            {
                this.age = ObliczWiek(dtpDataUro.SelectedDate.Value);
                // Możesz wyświetlić wiek np. w ToolTip lub dodatkowym TextBlock
                dtpDataUro.ToolTip = $"Wiek: {this.age} lat";
            }
        }

        private bool SprawdzWiek(DateTime dataUrodzenia, int minimalnyWiek = 18)
        {
            this.age = ObliczWiek(dataUrodzenia);

            if (this.age < minimalnyWiek)
            {
                SnackbarService.Show($"Minimalny wiek to {minimalnyWiek} lat");
                return false;
            }

            return true;
        }

        public static int ObliczWiek(DateTime dataUrodzenia)
        {
            var dzisiaj = DateTime.Today;
            var wiek = dzisiaj.Year - dataUrodzenia.Year;

            // Sprawdź czy urodziny już były w tym roku
            if (dataUrodzenia.Date > dzisiaj.AddYears(-wiek))
            {
                wiek--;
            }

            return wiek;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            PowrotDoLogowania();
        }

        void PowrotDoLogowania()
        {
            this.NavigationService?.Navigate(new loginPage());
        }
    }
}