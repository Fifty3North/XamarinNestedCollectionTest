using NestedCollectionPerformanceTest.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace NestedCollectionPerformanceTest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MoviesByCompanyListView : F3N.YaMVVM.Views.YamvvmPage
    {
        public MoviesByCompanyListView()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (this.BindingContext is MovieListByCompany vm)
            {
                vm.StopwatchStop();
            }
        }
    }
}