using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Threading;

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
                brdrLike.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnLogin.Visibility = Visibility.Collapsed;
                btnLogout.Visibility = Visibility.Visible;
                brdrLike.Visibility = Visibility.Visible;
            }
        }

        private void LoadHomePage()
        {
          
            MainFrame.Navigate(new HomePage());
        }
        private DispatcherTimer snackbarTimer;

        public void ShowSnackbar(string message, int duration = 3000)
        {
            SnackbarMessage.Text = message;
            SnackbarContainer.Visibility = Visibility.Visible;

            var opacityAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(300));
            SnackbarContainer.BeginAnimation(OpacityProperty, opacityAnimation);

            snackbarTimer?.Stop();
            snackbarTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(duration) };
            snackbarTimer.Tick += (sender, args) =>
            {
                var hideAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(500));
                hideAnimation.Completed += (s, e) => SnackbarContainer.Visibility = Visibility.Collapsed;
                SnackbarContainer.BeginAnimation(OpacityProperty, hideAnimation);
                snackbarTimer.Stop();
            };
            snackbarTimer.Start();
        }
        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            if (App.LoggedInUser == null)
            {
                ShowSnackbar("Musisz się zalogować.");
                return;
            }
            if(MainFrame.Content is not UserPage)
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
               ShowSnackbar("Musisz się zalogować.");
                return;
            }
           // if (MainFrame.Content is not MessagesPage)
            // MainFrame.Navigate(new MessagesPage(App.LoggedInUser.id));
        }

        private void btnLike_Click(object sender, RoutedEventArgs e)
        {
            if (App.LoggedInUser == null)
            {
                ShowSnackbar("Musisz się zalogować.");
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
                ShowSnackbar("Musisz się zalogować.");
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