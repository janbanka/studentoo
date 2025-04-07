using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;

namespace studentoo
{
    public partial class UserPage : Page
    {
        private bool isEditMode = false;

        private User currentUser;
        private List<string> interests = new List<string>() {};
        private List<photos> userPhotos = new();

        public UserPage(int userId)
        {
            InitializeComponent();
            LoadUserProfile(userId);
            LoadUserProfile(userId);

        }

        private void LoadUserProfile(int userId)
        {
            try
            {
                using (var db = new UserDataContext())
                {
                    currentUser = db.Users
                        .AsNoTracking()
                        .FirstOrDefault(u => u.id == userId);

                    if (currentUser != null)
                    {
                        txtUserName.Text = $"{currentUser.name} {currentUser.surname}, {currentUser.age}";
                        txtBio.Text = currentUser.description ?? "Brak dodatkowych informacji";

                        LoadUserPhotos(userId);

                        if (!string.IsNullOrWhiteSpace(currentUser.interests))
                        {
                            interests = currentUser.interests
                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                .Select(i => i.Trim())
                                .ToList();
                        }
                        RenderInterests();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania profilu: {ex.Message}");
            }
        }
        private void LoadUserPhotos(int userId)
        {
            try
            {
                using (var db = new UserDataContext())
                {
                    userPhotos = db.photos
                        .Where(p => p.user_id == userId)
                        .OrderByDescending(p => p.uploaded_at)
                        .AsNoTracking()
                        .ToList();

                    RefreshPhotos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd ładowania zdjęć: {ex.Message}");
            }
        }
        private void btnAddPhoto_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true && currentUser != null)
            {
                try
                {
                    string filePath = dialog.FileName;
                    var imageUri = new Uri(filePath, UriKind.Absolute);

                    using (var db = new UserDataContext())
                    {
                        var photo = new photos
                        {
                            user_id = currentUser.id,
                            url = imageUri.ToString(),
                            uploaded_at = DateTime.Now
                        };

                        db.photos.Add(photo);
                        db.SaveChanges();
                        userPhotos.Insert(0, photo);
                    }

                    RefreshPhotos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd dodawania zdjęcia: {ex.Message}");
                }
            }
        }
        private void DeletePhoto(string url)
        {
            using (var db = new UserDataContext())
            {
                var photo = db.photos.FirstOrDefault(p => p.url == url && p.user_id == currentUser.id);
                if (photo != null)
                {
                    db.photos.Remove(photo);
                    db.SaveChanges();
                }
            }
            LoadUserPhotos(currentUser.id);
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
            isEditMode = true;
           

            txtBioEdit.Text = txtBio.Text;
            txtBio.Visibility = Visibility.Collapsed;
            txtBioEdit.Visibility = Visibility.Visible;

            btnEdit.Visibility = Visibility.Collapsed;
            btnSave.Visibility = Visibility.Visible;

            panelTagsView.Visibility = Visibility.Collapsed;
            panelTagsEdit.Visibility = Visibility.Visible;
            addTagPanel.Visibility = Visibility.Visible;

            btnAddPhoto.Visibility = Visibility.Visible;
            
            RefreshPhotos();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            isEditMode = false;

            txtBio.Text = txtBioEdit.Text;
            txtBio.Visibility = Visibility.Visible;
            txtBioEdit.Visibility = Visibility.Collapsed;

            btnEdit.Visibility = Visibility.Visible;
            btnSave.Visibility = Visibility.Collapsed;

            panelTagsView.Visibility = Visibility.Visible;
            panelTagsEdit.Visibility = Visibility.Collapsed;
            addTagPanel.Visibility = Visibility.Collapsed;

            btnAddPhoto.Visibility = Visibility.Collapsed;
            RefreshPhotos();

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

        private void RefreshPhotos()
        {
            if (photoCarousel == null) return;

            // Clear existing items and event handlers
            if (photoCarousel.ItemsSource != null)
            {
                photoCarousel.ItemsSource = null;
            }
            else
            {
                photoCarousel.Items.Clear();
            }

            var photoList = new List<UIElement>();

            foreach (var photo in userPhotos)
            {
                try
                {
                    var image = new Image
                    {
                        Source = new BitmapImage(new Uri(photo.url, UriKind.Absolute))
                        {
                            CacheOption = BitmapCacheOption.OnLoad,
                            CreateOptions = BitmapCreateOptions.IgnoreImageCache
                        },
                        Width = 450,
                        Height = 300,
                        Stretch = Stretch.UniformToFill,
                        Margin = new Thickness(5)
                    };

                    var grid = new Grid();
                    grid.Children.Add(image);

                    if (isEditMode)
                    {
                        var deleteButton = new Button
                        {
                            Content = "❌",
                            Width = 24,
                            Height = 24,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Top,
                            Margin = new Thickness(0, 5, 5, 0),
                            Tag = photo,
                            Style = (Style)FindResource("IconButton")
                        };
                        deleteButton.Click += DeletePhoto_Click;
                        grid.Children.Add(deleteButton);
                    }
                

                    photoList.Add(grid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading photo {photo.url}: {ex.Message}");
                }
            }

            photoCarousel.ItemsSource = photoList;
        }


        private void DeletePhoto_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is photos photoToDelete)
            {
                try
                {
                    using (var db = new UserDataContext())
                    {
                        var photoInDb = db.photos.FirstOrDefault(p => p.id == photoToDelete.id);
                        if (photoInDb != null)
                        {
                            db.photos.Remove(photoInDb);
                            db.SaveChanges();
                        }
                    }

                    userPhotos.Remove(photoToDelete);
                    RefreshPhotos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd usuwania zdjęcia: {ex.Message}");
                }
            }
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
                if (interests.Count < 5)
                {
                    interests.Add(newTag);
                    txtNewTag.Text = string.Empty;
                    RenderInterests();
                }
                else
                {
                    MessageBox.Show("Możesz dodać maksymalnie 5 zainteresowań");
                }
            }
        }
        private void RemoveInterest_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string tag)
            {
                interests.Remove(tag);
                RenderInterests();
            }
        }
        private void InitializePhotoCarousel()
        {
            if (photoCarousel == null)
            {
                photoCarousel = new ListBox
                {
                    ItemsPanel = new ItemsPanelTemplate(new FrameworkElementFactory(typeof(WrapPanel))),
                    ItemTemplate = new DataTemplate()
                };
            }
        }


    }
}