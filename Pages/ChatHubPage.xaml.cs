using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace studentoo.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy ChatHubPage.xaml
    /// </summary>
    public partial class ChatHubPage : Page
    {
        private UserDataContext _db = new UserDataContext();
        private int currentUserId { get; set; }
        private System.Windows.Threading.DispatcherTimer _updateTimer;

        public ChatHubPage(int userId)
        {
            InitializeComponent(); 
            currentUserId = userId;
            Loaded += (s, e) => LoadConversations();
            _updateTimer = new System.Windows.Threading.DispatcherTimer();
            _updateTimer.Tick += UpdateMatches;
            _updateTimer.Interval = TimeSpan.FromSeconds(30);
            _updateTimer.Start();
        }
        
        private void UpdateMatches(object sender, EventArgs e)
        {
            LoadConversations();
        }
       private void LoadConversations()
{
    try
    {
        var matches = _db.paired
            .Include(p => p.User1).ThenInclude(u => u.zdj)
            .Include(p => p.User2).ThenInclude(u => u.zdj)
            .Where(p => (p.user_id == currentUserId || p.user_id2 == currentUserId) &&
                       p.is_matched) // Tylko wzajemne dopasowania
            .AsNoTracking()
            .ToList();

                var uniquePartners = matches
                   .Select(p => p.user_id == currentUserId ? p.User2 : p.User1)
                   .GroupBy(u => u.id)
                   .Select(g => g.First())
                   .ToList();

                var viewModels = uniquePartners.Select(partner => new ConversationViewModel
                {
                    Partner = partner,
                    LastMessageTime = matches
                        .FirstOrDefault(p =>
                            (p.user_id == currentUserId && p.user_id2 == partner.id) ||
                            (p.user_id == partner.id && p.user_id2 == currentUserId))
                        ?.timestamp.ToString("dd.MM.yyyy") ?? "Brak daty"
                })
                .OrderByDescending(c => c.LastMessageTime)
        .ToList();

        Dispatcher.Invoke(() =>
        {
            ConversationsListView.ItemsSource = viewModels;
            NoMatchesText.Visibility = viewModels.Any() ? Visibility.Collapsed : Visibility.Visible;
        });
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Błąd ładowania konwersacji: {ex}");
        Dispatcher.Invoke(() => 
            MessageBox.Show($"Błąd: {ex.Message}"));
    }
}

        private string GetLastMessagePreview(paired pair)
        {
            var partnerId = pair.user_id == currentUserId ? pair.user_id2 : pair.user_id;
            var lastMessage = _db.messages
                .Where(m => (m.sender_id == currentUserId && m.receiver_id == partnerId) ||
                           (m.sender_id == partnerId && m.receiver_id == currentUserId))
                .OrderByDescending(m => m.sent_at)
                .AsNoTracking()
                .FirstOrDefault();

            return lastMessage?.content ?? "Brak wiadomości";
        }

        private string GetLastMessageTime(paired pair)
        {
            var partnerId = pair.user_id == currentUserId ? pair.user_id2 : pair.user_id;
            var lastMessage = _db.messages
                .Where(m => (m.sender_id == currentUserId && m.receiver_id == partnerId) ||
                           (m.sender_id == partnerId && m.receiver_id == currentUserId))
                .OrderByDescending(m => m.sent_at)
                .AsNoTracking()
                .FirstOrDefault();

            return lastMessage?.sent_at.ToString("HH:mm") ?? "--:--";
        }

        private int CountMatches()
        {
            using (var db = new UserDataContext())
            {
                return db.paired
                    .Where(p => (p.user_id == currentUserId || p.user_id2 == currentUserId) &&
                               p.is_like == true )
                              
                    .Count();
            }
        }

        private void ConversationSelected(object sender, SelectionChangedEventArgs e)
        {
            if (ConversationsListView.SelectedItem is ConversationViewModel selected)
            {
                NavigationService.Navigate(new ChatPage(selected.Partner.id));
            }
        }

        public class ConversationViewModel
        {
            public User Partner { get; set; }
            public string LastMessagePreview { get; set; } // "Wzajemne dopasowanie"
            public string LastMessageTime { get; set; } // Czas dopasowania
        }
    }
}
