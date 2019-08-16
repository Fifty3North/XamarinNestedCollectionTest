using F3N.YaMVVM.ViewModel;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NestedCollectionPerformanceTest
{
    public partial class App : F3N.YaMVVM.App.YamvvmApp
    {
        public App()
        {
            InitializeComponent();
        }

        protected override async void OnStart()
        {
            await ViewModelNavigation.SetMainPage<MainPage>(new ViewModels.MainPageViewModel());
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
