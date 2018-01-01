using Autofac;
using FriendOrganizer.UI.Data;
using FriendOrganizer.UI.StartUp;
using FriendOrganizer.UI.ViewModel;
using System.Windows;

namespace FriendOrganizer.UI
{

    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var bootstrupper = new Bootstrapper();
            var container = bootstrupper.Bootstrup();

            var MainWindows = container.Resolve<MainWindow>();

            //var MainWindows = new MainWindow(
            //    new MainViewModel(
            //        new FriendDataService()));

            MainWindows.Show();
        }
    }
}
