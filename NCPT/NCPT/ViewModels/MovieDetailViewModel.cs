using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NestedCollectionPerformanceTest.ViewModels
{
    public class MovieDetailViewModel : F3N.YaMVVM.ViewModel.PageViewModel
    {
        private int _movieId;
        private Services.MovieService _movieService;
        public Models.Movie Movie { get; set; }
        public Models.MovieImages Images { get; set; }
        public bool Loading { get; set; }

        public Command OpenUrl => new Command((param) => { if (param is string url) Device.OpenUri(new Uri(url)); });

        //XAML intellisense
        public MovieDetailViewModel()
        {

        }

        public MovieDetailViewModel(string apiKey, int movieId)
        {
            _movieId = movieId;
            _movieService = new Services.MovieService(apiKey);
        }

        public override async Task Initialise()
        {
            Loading = true;

            Movie = await _movieService.MovieDetail(_movieId);

            Loading = false;

            Images = await _movieService.MovieImages(_movieId);

            await base.Initialise();
        }
    }
}
