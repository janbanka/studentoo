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
    /// Logika interakcji dla klasy LandingPage.xaml
    /// </summary>
    public partial class LandingPage : Page
    {
        public LandingPage()
        {
            InitializeComponent();
        }



        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            App.MainFrame.Navigate(new loginPage());
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            App.MainFrame.Navigate(new Rejestracja1()); 
        }
    }
}