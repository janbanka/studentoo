using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace studentoo;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static Frame MainF { get; set; }
    public static Frame MainFrame => ((MainWindow)Current.MainWindow).MainFrame;
    public static User? LoggedInUser { get; set; } = null;
}



