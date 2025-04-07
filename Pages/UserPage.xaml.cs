using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;

namespace studentoo
{
    public partial class UserPage : Page
    {
        private User currentUser;
        private List<string> interests = new List<string>() {};
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
                   
                    if (!string.IsNullOrWhiteSpace(currentUser.interests))
                    {
                        interests = currentUser.interests.Split(',').Select(i => i.Trim()).ToList();
                    }
                    RenderInterests();

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
            txtBioEdit.Text = txtBio.Text;
            txtBio.Visibility = Visibility.Collapsed;
            txtBioEdit.Visibility = Visibility.Visible;

            btnEdit.Visibility = Visibility.Collapsed;
            btnSave.Visibility = Visibility.Visible;

            panelTagsView.Visibility = Visibility.Collapsed;
            panelTagsEdit.Visibility = Visibility.Visible;
            addTagPanel.Visibility = Visibility.Visible;

            RenderInterests();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
{
            txtBio.Text = txtBioEdit.Text;

            txtBioEdit.Visibility = Visibility.Collapsed;
            txtBio.Visibility = Visibility.Visible;

            btnSave.Visibility = Visibility.Collapsed;
            btnEdit.Visibility = Visibility.Visible;

            panelTagsEdit.Visibility = Visibility.Collapsed;
            panelTagsView.Visibility = Visibility.Visible;
            addTagPanel.Visibility = Visibility.Collapsed;

            if (currentUser != null)
            {
                try
                {
                    using (var db = new UserDataContext())
            {
                var userToUpdate = db.Users.FirstOrDefault(u => u.id == currentUser.id);
                if (userToUpdate != null)
                {
                    userToUpdate.description = txtBioEdit.Text;
                    userToUpdate.interests = string.Join(",", interests);
                    db.SaveChanges();

                    currentUser.description = userToUpdate.description;
                    currentUser.interests = userToUpdate.interests;
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Błąd zapisu: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    RenderInterests();
}


      

        private void RenderInterests()
        {
            panelTagsView.Children.Clear();
            panelTagsEdit.Children.Clear();

            foreach (var interest in interests)
            {
                // Widok 
                var tagView = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFE3C72")),
                    CornerRadius = new CornerRadius(15),
                    Padding = new Thickness(8, 5, 8, 5),
                    Margin = new Thickness(0, 0, 5, 5),
                    Child = new TextBlock { Text = interest, Foreground = Brushes.White, FontSize = 12 }
                };
                panelTagsView.Children.Add(tagView);

                // Edycja
                var btnRemove = new Button
                {
                    Content = "❌",
                    FontSize = 12,
                    Padding = new Thickness(2),
                    Margin = new Thickness(5, 0, 0, 0),
                    Tag = interest
                };
                btnRemove.Click += RemoveInterest_Click;

                var tagEditPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
            {
                new TextBlock { Text = interest, FontSize = 12, VerticalAlignment = VerticalAlignment.Center },
                btnRemove
            }
                };

                var tagEditBorder = new Border
                {
                    Background = Brushes.LightGray,
                    CornerRadius = new CornerRadius(15),
                    Padding = new Thickness(8, 5, 8, 5),
                    Margin = new Thickness(0, 0, 5, 5),
                    Child = tagEditPanel
                };

                panelTagsEdit.Children.Add(tagEditBorder);
            }
        }
        private void btnAddTag_Click(object sender, RoutedEventArgs e)
        {
            var newTag = txtNewTag.Text.Trim();

            if (!string.IsNullOrEmpty(newTag) && !interests.Contains(newTag))
            {
                interests.Add(newTag);
                txtNewTag.Text = "";
                if (interests.Count <= 4)
                    RenderInterests();
                else
                    MessageBox.Show("Osiągnięto limit zainteresowań");
            }
        }
        private void RemoveInterest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag && interests.Contains(tag))
            {
                interests.Remove(tag);
                RenderInterests();
            }
        }



    }
}