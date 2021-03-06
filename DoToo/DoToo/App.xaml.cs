using DoToo.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace DoToo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            Startup.Init();
            MainPage =
                new NavigationPage(Startup.ServiceProvider.GetService<MainView>());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
