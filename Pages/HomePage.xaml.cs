using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Threading;
namespace studentoo
{
    public partial class HomePage : Page
    {
        private List<User> potentialMatches;
        private int currentIndex = 0;
        private int loggedInUserId;
        private DateTime lastAnimation;
        public HomePage()
        {
            InitializeComponent();
            InitializeMatchSystem();
        }

        private void InitializeMatchSystem()
        {
            if (App.LoggedInUser != null)
            {
                loggedInUserId = App.LoggedInUser.id;
                LoadPotentialMatches();
                
            }
        }

        private async Task LoadPotentialMatches()
        {
            try
            {
                using (var db = new UserDataContext())
                {
                    var ratedUserIds = await db.paired
                        .Where(p => p.user_id == loggedInUserId)
                        .Select(p => p.user_id2)
                        .ToListAsync();

                    var usersWithPhotos = await db.Users
                        .Where(u => u.id != loggedInUserId)
                        .Where(u => !ratedUserIds.Contains(u.id)) 
                        .Where(u => db.photos.Any(p => p.user_id == u.id && p.photo_data != null))
                        .OrderByDescending(u => u.created_at)
                        .ToListAsync();

                    var userIds = usersWithPhotos.Select(u => u.id).ToList();
                    var userPhotos = await db.photos
                        .Where(p => userIds.Contains(p.user_id) && p.photo_data != null)
                        .GroupBy(p => p.user_id)
                        .ToDictionaryAsync(g => g.Key, g => g.ToList());

                    foreach (var user in usersWithPhotos)
                    {
                        if (userPhotos.TryGetValue(user.id, out var photos))
                        {
                            user.zdj = photos;
                        }
                        else
                        {
                            user.zdj = new List<photos>();
                        }
                    }

                    potentialMatches = usersWithPhotos;
                    UpdateUI();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd ładowania użytkowników: {ex}");
            }
        }
        public static bool IsImageDataValid(byte[] data)
        {
            if (data == null || data.Length < 8)
                return false;

            var headers = new Dictionary<byte[], string>
    {
        { new byte[] { 0xFF, 0xD8, 0xFF }, "jpg" }, // JPEG
        { new byte[] { 0x89, 0x50, 0x4E, 0x47 }, "png" }, // PNG
        { new byte[] { 0x47, 0x49, 0x46, 0x38 }, "gif" } // GIF
    };

            return headers.Any(header =>
                header.Key.SequenceEqual(data.Take(header.Key.Length)));
        }


        public void LikeCurrentUser()
        {
            if (currentIndex >= potentialMatches.Count) return;
            CreateFloatingHearts();
            var likedUser = potentialMatches[currentIndex];
            SaveMatchAction(likedUser.id, true);
            CheckForMatch(likedUser.id);
            ShowNextUser();
        }

        public void DislikeCurrentUser()
        {
            if (currentIndex >= potentialMatches.Count) return;

            var dislikedUser = potentialMatches[currentIndex];
            SaveMatchAction(dislikedUser.id, false);
            ShowNextUser();
        }
        private void CreateFloatingHearts(int count = 5)
        {
            Random rand = new Random();

            for (int i = 0; i < count; i++)
            {
                var delay = TimeSpan.FromMilliseconds(i * 150); // lekkie opóźnienie między kolejnymi

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = delay;
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    SpawnSingleHeart(rand);
                };
                timer.Start();
            }
        }

        private void SpawnSingleHeart(Random rand)
        {
            var heart = new TextBlock
            {
                Text = "❤",
                FontSize = rand.Next(18, 28),
                Foreground = new SolidColorBrush(Color.FromRgb(
                    (byte)rand.Next(220, 255),
                    (byte)rand.Next(50, 100),
                    (byte)rand.Next(100, 150)
                )),
                Opacity = 0.8,
                RenderTransform = new RotateTransform(rand.Next(-20, 20))
            };

            double startX = rand.Next(50, (int)(HeartCanvas.ActualWidth - 50));
            Canvas.SetLeft(heart, startX);
            Canvas.SetTop(heart, -30);

            HeartCanvas.Children.Add(heart);

            double endY = HeartCanvas.ActualHeight + 50;

            DoubleAnimation fall = new DoubleAnimation
            {
                From = -30,
                To = endY,
                Duration = TimeSpan.FromSeconds(rand.NextDouble() * 1 + 1.5),
                AccelerationRatio = 0.3
            };

            DoubleAnimation fade = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(1),
                BeginTime = TimeSpan.FromSeconds(1)
            };

            Storyboard sb = new Storyboard();
            sb.Children.Add(fall);
            sb.Children.Add(fade);

            Storyboard.SetTarget(fall, heart);
            Storyboard.SetTargetProperty(fall, new PropertyPath("(Canvas.Top)"));

            Storyboard.SetTarget(fade, heart);
            Storyboard.SetTargetProperty(fade, new PropertyPath("Opacity"));

            sb.Completed += (s, e) => HeartCanvas.Children.Remove(heart);
            sb.Begin();
        }
        private void CheckForMatch(int targetUserId)
        {
            using (var db = new UserDataContext())
            {
                var otherLike = db.paired
                    .FirstOrDefault(p => p.user_id == targetUserId &&
                                       p.user_id2 == loggedInUserId &&
                                       p.is_like == true);

                if (otherLike != null)
                {
                    var myLike = db.paired
                        .FirstOrDefault(p => p.user_id == loggedInUserId &&
                                           p.user_id2 == targetUserId);

                    if (myLike != null)
                    {
                        myLike.is_matched = true;
                        otherLike.is_matched = true;

                        db.SaveChanges();

                        Dispatcher.Invoke(() =>
                        {
                            SnackbarService.Show($"Nowy match! Z {GetUserName(targetUserId)}");
                        });
                    }
                }
            }
        }

        private string GetUserName(int userId)
        {
            using (var db = new UserDataContext())
            {
                return db.Users.FirstOrDefault(u => u.id == userId)?.name ?? "użytkownikiem";
            }
        }
        private async void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            currentIndex = 0;
            await LoadPotentialMatches();

            if (potentialMatches.Count == 0) 
            {
                
                Storyboard animation = (Storyboard)FindResource("PulseAnimation");
                animation.Begin(NoMoreUsersPanel);
            }
        }
        private void ShowNextUser()
        {
            currentIndex++;
            UpdateUI();

            if (currentIndex >= potentialMatches.Count)
            {
                NoMoreUsersPanel.Visibility = Visibility.Visible;

                var animation = (Storyboard)this.Resources["PulseAnimation"];
                if (animation != null)
                {
                    animation.Begin(NoMoreUsersPanel);
                }
            }
        }

        private void UpdateUI()
        {
            if (potentialMatches == null || currentIndex >= potentialMatches.Count)
            {
                UsersCardsContainer.Visibility = Visibility.Collapsed;
                NoMoreUsersPanel.Visibility = Visibility.Visible;
                return;
            }

            var currentUser = potentialMatches[currentIndex];
            UsersCardsContainer.ItemsSource = new List<User> { currentUser };
            UsersCardsContainer.Visibility = Visibility.Visible;
            NoMoreUsersPanel.Visibility = Visibility.Collapsed;
        }


        private void SaveMatchAction(int targetUserId, bool isLike)
        {
            using (var db = new UserDataContext())
            {
                if (!db.paired.Any(p => p.user_id == loggedInUserId && p.user_id2 == targetUserId))
                {
                    var match = new paired
                    {
                        user_id = loggedInUserId,
                        user_id2 = targetUserId,
                        is_like = isLike,
                        timestamp = DateTime.Now
                    };
                    db.paired.Add(match);
                    db.SaveChanges();
                }
            }
        }

        private void ViewProfile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int userId)
            {
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.MainFrame.Navigate(new UserPage(userId));
            }
        }
    }
}