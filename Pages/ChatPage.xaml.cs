using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace studentoo.Pages
{
    public partial class ChatPage : Page
    {
        private readonly UserDataContext _db = new UserDataContext();
        private readonly int _currentUserId;
        private readonly int _partnerId;
        private int _chatId;
        private int? _pairedId;
        private DispatcherTimer _updateTimer;

        public ChatPage(int partnerId, int currentUserId)
        {
            InitializeComponent();
            _partnerId = partnerId;
            _currentUserId = currentUserId;
            _db = new UserDataContext();

            if (!InitializeChat())
            {
                NavigationService?.GoBack();
                return;
            }

            Loaded += ChatPage_Loaded;
            _updateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            _updateTimer.Tick += (s, e) => LoadMessages();
            _updateTimer.Start();
        }

        private bool InitializeChat()
        {
            try
            {
                // Znajdź sparowanie
                _pairedId = _db.paired
                    .Where(p => (p.user_id == _currentUserId && p.user_id2 == _partnerId && p.is_matched) ||
                               (p.user_id == _partnerId && p.user_id2 == _currentUserId && p.is_matched))
                    .Select(p => (int?)p.id)
                    .FirstOrDefault();

                if (_pairedId == null)
                {
                    MessageBox.Show("Najpierw musisz dopasować się z tym użytkownikiem!");
                    return false;
                }

                // Sprawdź czy chat istnieje
                var chatExists = _db.chats.Any(c => c.paired_id == _pairedId);
                if (!chatExists)
                {
                    _db.chats.Add(new chats
                    {
                        paired_id = _pairedId.Value,
                        created_at = DateTime.Now
                    });
                    _db.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd inicjalizacji: {ex}");
                MessageBox.Show("Błąd podczas inicjalizacji chatu");
                return false;
            }
        }

        private void ChatPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPartnerInfo();
            LoadMessages();
        }

        private void LoadPartnerInfo()
        {
            try
            {
                var partner = _db.Users
                    .Include(u => u.zdj)
                    .FirstOrDefault(u => u.id == _partnerId);

                if (partner == null) return;

                PartnerNameText.Text = $"{partner.name} {partner.surname}";

                var photo = partner.zdj.FirstOrDefault()?.photo_data;
                if (photo != null && photo.Length > 0)
                {
                    var bitmap = ConvertByteArrayToImage(photo);
                    PartnerEllipse.Fill = new ImageBrush(bitmap)
                    {
                        Stretch = Stretch.UniformToFill
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd ładowania partnera: {ex}");
            }
        }

        private BitmapImage ConvertByteArrayToImage(byte[] imageData)
        {
            if (imageData == null || imageData.Length == 0) return null;

            var image = new BitmapImage();
            using (var ms = new MemoryStream(imageData))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
            }
            return image;
        }

        private void LoadMessages()
        {
            if (_pairedId == null) return;

            try
            {
                // 1) Pobierz chatId dla naszego paired_id
                var chatId = _db.chats
                    .Where(c => c.paired_id == _pairedId.Value)
                    .Select(c => c.id)
                    .FirstOrDefault();

                if (chatId == 0)
                    return; // brak chatu, wychodzimy

                // 2) Pobierz wiadomości filtrowane po chat_id
                var messages = _db.messages
                    .Where(m => m.chat_id == chatId)
                    .OrderBy(m => m.sent_at)
                    .Select(m => new
                    {
                        Content = m.content,
                        SentAt = m.sent_at,
                        IsFromCurrentUser = (m.sender_id == _currentUserId)
                    })
                    .ToList();

                // 3) Wyświetl je „ręcznie” w MessagesPanel (StackPanel)
                MessagesPanel.Children.Clear();
                foreach (var msg in messages)
                {
                    var panel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        HorizontalAlignment = msg.IsFromCurrentUser
                                              ? HorizontalAlignment.Right
                                              : HorizontalAlignment.Left,
                        Margin = new Thickness(0, 5, 0, 5)
                    };

                    var border = new Border
                    {
                        Background = msg.IsFromCurrentUser
                                     ? (Brush)new SolidColorBrush(Color.FromRgb(254, 60, 114))
                                     : Brushes.White,
                        CornerRadius = msg.IsFromCurrentUser
                                       ? new CornerRadius(10, 10, 0, 10)
                                       : new CornerRadius(10, 10, 10, 0),
                        Padding = new Thickness(10),
                        MaxWidth = 300
                    };
                    border.Child = new TextBlock
                    {
                        Text = msg.Content,
                        TextWrapping = TextWrapping.Wrap,
                        Foreground = msg.IsFromCurrentUser
                                     ? Brushes.White
                                     : Brushes.Black
                    };

                    var timeText = new TextBlock
                    {
                        Text = msg.SentAt.ToString("HH:mm"),
                        FontSize = 10,
                        Foreground = Brushes.Gray,
                        Margin = new Thickness(5, 2, 5, 0),
                        HorizontalAlignment = msg.IsFromCurrentUser
                                              ? HorizontalAlignment.Right
                                              : HorizontalAlignment.Left
                    };

                    panel.Children.Add(border);
                    panel.Children.Add(timeText);
                    MessagesPanel.Children.Add(panel);
                }

                MessagesScrollViewer.ScrollToEnd();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd ładowania wiadomości: {ex}");
            }
        }



        private void SendMessage()
        {
            if (_pairedId == null) return;

            var text = MessageTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;

            try
            {
                // Znajdź chat_id dla tego sparowania
                var chatId = _db.chats
                    .Where(c => c.paired_id == _pairedId)
                    .Select(c => c.id)
                    .FirstOrDefault();

                if (chatId == 0)
                {
                    MessageBox.Show("Nie znaleziono odpowiedniego chatu");
                    return;
                }

                var message = new messages
                {
                    chat_id = chatId,
                    sender_id = _currentUserId,
                    content = text,
                    sent_at = DateTime.Now
                };

                _db.messages.Add(message);
                _db.SaveChanges();

                MessageTextBox.Clear();
                LoadMessages();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd wysyłania: {ex}");
                MessageBox.Show("Nie udało się wysłać wiadomości");
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e) => SendMessage();

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                SendMessage();
                e.Handled = true;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _updateTimer?.Stop();
            NavigationService?.GoBack();
        }
    }
}