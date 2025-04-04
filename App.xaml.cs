using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Configuration;
using System.Data;
using System.Windows;

namespace studentoo;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
       
        using (var context = new UserDataContext())
        {
            DatabaseFacade facade = new DatabaseFacade(context);
            facade.EnsureCreated();
        }
    }
}

