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
    public partial class MovieDetail : F3N.YaMVVM.Views.YamvvmPage
    {
        public MovieDetail()
        {
            InitializeComponent();
        }
    }
}