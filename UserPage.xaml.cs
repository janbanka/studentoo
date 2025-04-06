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
    }
}