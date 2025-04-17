using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace studentoo.Pages
{
    public partial class ChatPage : Page
    {
        private readonly UserDataContext _db = new UserDataContext();
        private readonly int _currentUserId;
        private readonly int _partnerId;
        private DispatcherTimer _updateTimer;
        private int _chatId;
        public ChatPage(int partnerId, int currentUserId)
        {
            InitializeComponent();
            _partnerId = partnerId;
            _currentUserId = currentUserId;

            InitializeChat();

            Loaded += ChatPage_Loaded;

            _updateTimer = new DispatcherTimer();
            _updateTimer.Interval = TimeSpan.FromSeconds(5);
            _updateTimer.Tick += (s, e) => LoadMessages();
            _updateTimer.Start();
        }

        private void ChatPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPartnerInfo();
            LoadMessages();
        }
        private void InitializeChat()
        {
            try
            {
                var existingChat = _db.paired
                    .FirstOrDefault(c =>
                        (c.user_id == _currentUserId && c.user_id2 == _partnerId) ||
                        (c.user_id == _partnerId && c.user_id2 == _currentUserId));

                if (existingChat == null)
                {
                    // Jeśli nie ma chatu, utwórz nowy
                    var newChat = new chats
                    {
                        paired_id=existingChat.id,
                        created_at = DateTime.Now
                    };
                    _db.chats.Add(newChat);
                    _db.SaveChanges();
                    _chatId = newChat.id;
                }
                else
                {
                    _chatId = existingChat.id;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Błąd inicjalizacji chatu: " + ex);
                MessageBox.Show("Wystąpił błąd podczas inicjalizacji chatu");
            }
        }
        private void LoadPartnerInfo()
        {
            try
            {
                var partner = _db.Users.Include(u => u.zdj).FirstOrDefault(u => u.id == _partnerId);
                if (partner != null)
                {
                    PartnerNameText.Text = $"{partner.name} {partner.surname}";
                    if (partner.zdj.Count != null)
                    {
                        var image = new BitmapImage();
                        using (var ms = new System.IO.MemoryStream(partner.zdj.Count))
                        {
                            image.BeginInit();
                            image.StreamSource = ms;
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.EndInit();
                        }
                        PartnerImage.Source = image;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Błąd ładowania partnera: " + ex);
            }
        }

        private void LoadMessages()
        {
            try
            {
                var messages = _db.messages
                    .Where(m => m.chat_id == _chatId) 
                    .OrderBy(m => m.sent_at)
                    .ToList();

                MessagesList.ItemsSource = messages;
                MessagesScrollViewer.ScrollToEnd();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Błąd ładowania wiadomości: " + ex);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void MessageTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                SendMessage();
            }
        }

        private void SendMessage()
        {
            var text = MessageTextBox.Text?.Trim();
            if (string.IsNullOrWhiteSpace(text)) return;

            try
            {
                // Sprawdź czy chat istnieje
                if (_chatId == 0)
                {
                    MessageBox.Show("Nie można wysłać wiadomości - czat nie został poprawnie zainicjowany");
                    return;
                }

                var message = new messages
                {
                    chat_id = _chatId, // Kluczowe - przypisanie do istniejącego czatu
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
                Debug.WriteLine($"Błąd wysyłania wiadomości: {ex}");
                MessageBox.Show("Nie udało się wysłać wiadomości. Spróbuj ponownie.");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
