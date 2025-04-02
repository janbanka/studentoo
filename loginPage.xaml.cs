using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace studentoo
{
    /// <summary>
    /// Logika interakcji dla klasy loginPage.xaml
    /// </summary>
    public partial class loginPage : Window
    {
        public loginPage()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = txtLogin.Text;
            var password = txtHaslo.Password;

            using (UserDataContext context = new UserDataContext())
            {
                bool userFound = context.Users.Any(user => (user.login == login) && (user.password_hash == password));
                if (userFound)
                {
                    grantAccess();
                    Close();
                }
                else
                {
                    MessageBox.Show("User not found");
                }
            }
        }

        public void grantAccess()
        {
            MainWindow main = new MainWindow();
            main.Show();
        }
    }
}
