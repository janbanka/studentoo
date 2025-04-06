using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace studentoo
{
    public partial class MainWindow : Window
    {
        private int currentUserId;

        public MainWindow(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            LoadHomePage();
            App.MainF = this.MainFrame;
            MainFrame.Navigate(new loginPage());
        }

        private void LoadHomePage()
        {
          
            MainFrame.Navigate(new HomePage(currentUserId));
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            
            MainFrame.Navigate(new UserPage(currentUserId));
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            LoadHomePage();
        }

        private void btnMessages_Click(object sender, RoutedEventArgs e)
        {
           
           // MainFrame.Navigate(new MessagesPage(currentUserId));
        }

        private void btnLike_Click(object sender, RoutedEventArgs e)
        {
            
            if (MainFrame.Content is HomePage homePage)
            {   
                homePage.LikeCurrentUser();
            }
        }

        private void btnDislike_Click(object sender, RoutedEventArgs e)
        {
            
            if (MainFrame.Content is HomePage homePage)
            {
               // homePage.DislikeCurrentUser();
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.MainF.Navigate(new loginPage());
        }

        protected override void OnClosed(EventArgs e)
        {

           
            base.OnClosed(e);
        }

    }
}