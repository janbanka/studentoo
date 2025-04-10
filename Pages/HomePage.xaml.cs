﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using System.IO;
namespace studentoo
{
    public partial class HomePage : Page
    {
        private List<User> potentialMatches;
        private int currentIndex = 0;
        private int loggedInUserId;

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
                    var usersWithPhotos = await db.Users
                        .Where(u => u.id != loggedInUserId)
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

        

        private Button CreateActionButton(string content, string color, RoutedEventHandler handler, string width = "35")
        {
            var button = new Button
            {
                Content = content,
                Width = double.Parse(width),
                Height = 35,
                Margin = new Thickness(15, 0, 15, 0),
                Background = new System.Windows.Media.SolidColorBrush(
                    (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(color)),
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 20,
                Style = (Style)FindResource("MinimalIconButton"),
                CommandParameter = content
            };

            button.Click += handler;

            return button;
        }


        public void LikeCurrentUser()
        {
            if (currentIndex >= potentialMatches.Count) return;

            var likedUser = potentialMatches[currentIndex];

            if (!HasExistingLike(loggedInUserId, likedUser.id))
            {
                SaveMatchAction(likedUser.id, true);
                CheckForMatch(likedUser.id);
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    SnackbarService.Show("Już polubiłeś tę osobę!");
                });
            }

            ShowNextUser();
        }

        public void DislikeCurrentUser()
        {
            if (currentIndex >= potentialMatches.Count) return;

            var dislikedUser = potentialMatches[currentIndex];
            SaveMatchAction(dislikedUser.id, false);
            ShowNextUser();
        }
        private bool HasExistingLike(int userId1, int userId2)
        {
            using (var db = new UserDataContext())
            {
                return db.paired.Any(m =>
                    m.user_id == userId1 &&
                    m.user_id2 == userId2 &&
                    m.is_like == true);
            }
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

        private void ShowNextUser()
        {
            currentIndex++;
            UpdateUI();

            if (currentIndex >= potentialMatches.Count)
            {
                Dispatcher.Invoke(() =>
                {
                    SnackbarService.Show("To już wszyscy użytkownicy w Twojej okolicy!");
                });
            }
        }

        private void UpdateUI()
        {
            if (UsersCardsContainer != null)
            {
                UsersCardsContainer.ItemsSource = potentialMatches
                    .Skip(currentIndex)
                    .Take(1) // tylko jeden użytkownik na raz
                    .ToList();

                UsersCardsContainer.Visibility =
                    potentialMatches.Count > 0 && currentIndex < potentialMatches.Count
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }


        private void SaveMatchAction(int targetUserId, bool isLike)
        {
            using (var db = new UserDataContext())
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