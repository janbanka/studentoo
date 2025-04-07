using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace studentoo
{
    public partial class MainWindow : Window
    {
        private int currentUserId;

        public MainWindow()
        {
            InitializeComponent();
            App.MainF = this.MainFrame;
            LoadHomePage();
            UpdateLoginStateUI();
        }

        public void UpdateLoginStateUI()
        {
            if (App.LoggedInUser == null)
            {
                btnLogin.Visibility = Visibility.Visible;
                btnLogout.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnLogin.Visibility = Visibility.Collapsed;
                btnLogout.Visibility = Visibility.Visible;
            }
        }

        private void LoadHomePage()
        {
          
            MainFrame.Navigate(new HomePage());
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            if (App.LoggedInUser == null)
            {
                MessageBox.Show("Musisz się zalogować, aby zobaczyć profil.");
                return;
            }
            MainFrame.Navigate(new UserPage(App.LoggedInUser.id));
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            LoadHomePage();
        }

        private void btnMessages_Click(object sender, RoutedEventArgs e)
        {
            if (App.LoggedInUser == null)
            {
                MessageBox.Show("Musisz się zalogować, aby zobaczyć profil.");
                return;
            }
            // MainFrame.Navigate(new MessagesPage(App.LoggedInUser.id));
        }

        private void btnLike_Click(object sender, RoutedEventArgs e)
        {
            if (App.LoggedInUser == null)
            {
                MessageBox.Show("Musisz się zalogować, aby zobaczyć profil.");
                return;
            }
            if (MainFrame.Content is HomePage homePage)
            {   
                homePage.LikeCurrentUser();
            }
        }

        private void btnDislike_Click(object sender, RoutedEventArgs e)
        {
            if (App.LoggedInUser == null)
            {
                MessageBox.Show("Musisz się zalogować, aby zobaczyć profil.");
                return;
            }
            if (MainFrame.Content is HomePage homePage)
            {
               // homePage.DislikeCurrentUser();
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.LoggedInUser = null;
            UpdateLoginStateUI();
            MainFrame.Navigate(new HomePage());
        }

        protected override void OnClosed(EventArgs e)
        {

           
            base.OnClosed(e);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new loginPage());
        }
    }
}