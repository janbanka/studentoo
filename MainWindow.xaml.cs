using studentoo.Pages;
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
        private DateTime LastAnimation;
        public MainWindow()
        {
            InitializeComponent();


            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            //this.Width = screenWidth * 0.8;
            this.Height = screenHeight * 0.9;

           // this.Left = (screenWidth - this.Width) / 2;
            this.Top = (screenHeight - this.Height) / 2;
            App.MainF = this.MainFrame;
           
            UpdateLoginStateUI();
        }

        public void UpdateLoginStateUI()
        {
            if (App.LoggedInUser == null)
            {
                btnLogin.Visibility = Visibility.Visible;
                btnLogout.Visibility = Visibility.Collapsed;
                brdrLike.Visibility = Visibility.Collapsed;
                MainFrame.Navigate(new LandingPage());
            }
            else
            {
                btnLogin.Visibility = Visibility.Collapsed;
                btnLogout.Visibility = Visibility.Visible;
                brdrLike.Visibility = Visibility.Visible;
                LoadHomePage();
            }
        }

        private void LoadHomePage()
        {
            if (App.LoggedInUser == null)
                MainFrame.Navigate(new LandingPage());
            else
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
            if (App.LoggedInUser != null)
            {
                MainFrame.Navigate(new ChatHubPage(App.LoggedInUser.id));
            }
        }
        public void UpdateMessagesBadge(int count)
        {
            Dispatcher.Invoke(() =>
            {
                if (count > 0)
                {
                    MessagesBadge.Visibility = Visibility.Visible;
                    MessagesBadgeText.Text = count.ToString();
                }
                else
                {
                    MessagesBadge.Visibility = Visibility.Collapsed;
                }
            });
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
                if ((DateTime.Now - LastAnimation).TotalMilliseconds > 300)
                    AnimateButton(btnLike);
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
                if ((DateTime.Now - LastAnimation).TotalMilliseconds > 300)
                    AnimateButton(btnDislike);
                homePage.DislikeCurrentUser();
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.LoggedInUser = null;
            UpdateLoginStateUI();
            MainFrame.Navigate(new LandingPage());
        }
        private void AnimateButton(Button button)
        {
            Storyboard storyboard = (Storyboard)FindResource("ButtonClickAnimation");
            Storyboard clone = storyboard.Clone();
            Storyboard.SetTarget(clone, button);
            clone.Begin();
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