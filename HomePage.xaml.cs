using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace studentoo
{
    public partial class HomePage : Page
    {
        private List<User> potentialMatches;
        private int currentIndex = 0;
        private int loggedInUserId;

        public HomePage(int userId)
        {
            InitializeComponent();
            loggedInUserId = userId;
            LoadPotentialMatches();
        }

        private void LoadPotentialMatches()
        {
            using (var db = new UserDataContext())
            {
                potentialMatches = db.Users
                    .Where(u => u.id != loggedInUserId)
                    .ToList();

                UsersCardsContainer.ItemsSource = potentialMatches;
            }
        }

        public void LikeCurrentUser()
        {
            if (currentIndex < potentialMatches.Count)
            {
                var likedUser = potentialMatches[currentIndex];
            
                currentIndex++;
                //ShowNextUser();
            }
        }

     

        private void ViewProfile_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            int userId = (int)button.Tag;

            // Otwórz profil w głównym frame
            var mainWindow = (MainWindow)Window.GetWindow(this);
            mainWindow.MainFrame.Navigate(new UserPage(userId));
        }

        //private void SaveMatchAction(int targetUserId, string actionType)
        //{
        //    using (var db = new UserDataContext())
        //    {
        //        var match = new Match
        //        {
        //            UserId = loggedInUserId,
        //            TargetUserId = targetUserId,
        //            Action = actionType,
        //            Timestamp = DateTime.Now
        //        };
        //        db.Matches.Add(match);
        //        db.SaveChanges();
        //    }
        //}
    }
}