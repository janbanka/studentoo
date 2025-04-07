using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace studentoo
{
    public partial class UserPage : Page
    {
        private User currentUser;

        public UserPage(int userId)
        {
            InitializeComponent();
            LoadUserProfile(userId);
        }

        private void LoadUserProfile(int userId)
        {
            using (var db = new UserDataContext())
            {
                currentUser = db.Users.FirstOrDefault(u => u.id == userId);

                if (currentUser != null)
                {
                    txtUserName.Text = $"{currentUser.name} {currentUser.surname}, {currentUser.age}";
                   
                    txtBio.Text = currentUser.description ?? "Brak dodatkowych informacji";
                    
                }
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void btnMessage_Click(object sender, RoutedEventArgs e)
        {
            //przenosi na strone chatu z chatami
            
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            // Pokaż TextBox do edycji
            txtBioEdit.Text = txtBio.Text;
            txtBio.Visibility = Visibility.Collapsed;
            txtBioEdit.Visibility = Visibility.Visible;

            btnEdit.Visibility = Visibility.Collapsed;
            btnSave.Visibility = Visibility.Visible;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Zapisz zmianę
            txtBio.Text = txtBioEdit.Text;

            // Schowaj TextBox, pokaż TextBlock
            txtBioEdit.Visibility = Visibility.Collapsed;
            txtBio.Visibility = Visibility.Visible;

            btnSave.Visibility = Visibility.Collapsed;
            btnEdit.Visibility = Visibility.Visible;

            // Zapis do bazy
            if (currentUser != null)
            {
                using (var db = new UserDataContext())
                {
                    var userToUpdate = db.Users.FirstOrDefault(u => u.id == currentUser.id);
                    if (userToUpdate != null)
                    {
                        userToUpdate.description = txtBioEdit.Text;
                        db.SaveChanges();
                    }
                }

                currentUser.description = txtBioEdit.Text;
            }
        }


    }
}