using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
    /// Logika interakcji dla klasy ChatPage.xaml
    /// </summary>
    public partial class ChatPage : Page
    {
        private int _currentUserId;
        private int _partnerId;
        private UserDataContext _db = new UserDataContext();

        public ChatPage(int partnerId)
        {
            InitializeComponent();
            _currentUserId = App.LoggedInUser.id;
            _partnerId = partnerId;

            LoadPartnerInfo();
            LoadMessages();
        }

        private void LoadPartnerInfo()
        {
            var partner = _db.Users.FirstOrDefault(u => u.id == _partnerId);
            if (partner != null)
            {
                PartnerNameText.Text = partner.name;
                PartnerImage.Source = partner.PhotoImage;
            }
        }

        private void LoadMessages()
        {
            //var messages = _db.messages
            //    .Where(m => (m.sender_id == _currentUserId && m.receiver_id == _partnerId) ||
            //               (m.sender_id == _partnerId && m.receiver_id == _currentUserId))
            //    .OrderBy(m => m.sent_at)
            //    .Select(m => new MessageViewModel
            //    {
            //        Content = m.content,
            //        SentAt = m.sent_at,
            //        IsCurrentUser = (m.sender_id == _currentUserId)
            //    })
            //    .ToList();

            //MessagesList.ItemsSource = messages;
            //ScrollToBottom();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void MessageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        private void SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(MessageTextBox.Text))
            {
                var message = new messages
                {
                    sender_id = _currentUserId,
                    receiver_id = _partnerId,
                    content = MessageTextBox.Text,
                    sent_at = DateTime.Now
                };

                _db.messages.Add(message);
                _db.SaveChanges();

                MessageTextBox.Clear();
                LoadMessages();
            }
        }

        private void ScrollToBottom()
        {
            MessagesScrollViewer.ScrollToEnd();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }

    public class MessageViewModel
    {
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
