using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

                if (partner != null)
                {
                    PartnerNameText.Text = $"{partner.name} {partner.surname}";

                    var firstPhoto = partner.zdj.FirstOrDefault();
                    if (firstPhoto != null)
                    {
                        PartnerImage.Source = ConvertByteArrayToImage(firstPhoto.photo_data);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Błąd ładowania informacji o partnerze: {ex}");
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
                var messages = _db.messages
                    .Where(m => m.chat.paired_id == _pairedId)
                    .OrderBy(m => m.sent_at)
                    .Select(m => new
                    {
                        m.content,
                        m.sent_at,
                        IsCurrentUser = (m.sender_id == _currentUserId)
                    })
                    .ToList();

                MessagesList.ItemsSource = messages;
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