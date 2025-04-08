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
            //App.MainF = this.MainFrame;
            LoadInitialPage();
            LoadHomePage();
            UpdateLoginStateUI();
        }

        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (sidebar.SelectedItem is NavButton selected)
            {
                // Sprawdzamy, czy użytkownik jest zalogowany przy próbie przejścia do UserPage
                if (selected.Navlink.OriginalString.Contains("UserPage.xaml") && App.LoggedInUser == null)
                {
                    MessageBox.Show("Musisz się zalogować, aby uzyskać dostęp do tej strony.");
                    sidebar.SelectedIndex = -1; // Resetujemy wybór
                    return;
                }

                navframe.Navigate(selected.Navlink);
            }

        }

        public void UpdateLoginStateUI()
        {
            if (App.LoggedInUser == null)
            {
               // btnLogin.Visibility = Visibility.Visible;
               // btnLogout.Visibility = Visibility.Collapsed;
               // brdrLike.Visibility = Visibility.Collapsed;
            }
            else
            {
               // btnLogin.Visibility = Visibility.Collapsed;
              //  btnLogout.Visibility = Visibility.Visible;
                //brdrLike.Visibility = Visibility.Visible;
            }
        }

        private void LoadInitialPage()
        {
            // Ładujemy domyślną stronę (np. loginPage jeśli nie zalogowany, lub HomePage jeśli zalogowany)
            if (App.LoggedInUser == null)
            {
                navframe.Navigate(new Uri("/Pages/loginPage.xaml", UriKind.Relative));
            }
            else
            {
                navframe.Navigate(new Uri("/Pages/HomePage.xaml", UriKind.Relative));
            }
        }

        private void LoadHomePage()
        {
          
           // MainFrame.Navigate(new HomePage());
        }

        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            if (App.LoggedInUser == null)
            {
                MessageBox.Show("Musisz się zalogować.");
                return;
            }
            //MainFrame.Navigate(new UserPage(App.LoggedInUser.id));
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            LoadHomePage();
        }

        private void btnMessages_Click(object sender, RoutedEventArgs e)
        {
            if (App.LoggedInUser == null)
            {
                MessageBox.Show("Musisz się zalogować.");
                return;
            }
            // MainFrame.Navigate(new MessagesPage(App.LoggedInUser.id));
        }

        private void btnLike_Click(object sender, RoutedEventArgs e)
        {
            if (App.LoggedInUser == null)
            {
                MessageBox.Show("Musisz się zalogować.");
                return;
            }
            //if (MainFrame.Content is HomePage homePage)
            //{   
            //    homePage.LikeCurrentUser();
            //}
        }

        private void btnDislike_Click(object sender, RoutedEventArgs e)
        {
            if (App.LoggedInUser == null)
            {
                MessageBox.Show("Musisz się zalogować.");
                return;
            }
            //if (MainFrame.Content is HomePage homePage)
           // {
               // homePage.DislikeCurrentUser();
           // }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.LoggedInUser = null;
            UpdateLoginStateUI();
            //MainFrame.Navigate(new HomePage());
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            //MainFrame.Navigate(new loginPage());
        }

        private void MainFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }
    }
}