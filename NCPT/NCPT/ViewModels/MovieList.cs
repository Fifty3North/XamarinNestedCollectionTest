using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using F3N.YaMVVM.ViewModel;
using NestedCollectionPerformanceTest.Models;
using System.Diagnostics;

namespace NestedCollectionPerformanceTest.ViewModels
{
    public class MovieList : F3N.YaMVVM.ViewModel.PageViewModel
    {
        private Stopwatch _stopwatch;
        private string _apiKey;
        private SearchResult _results;

        public ObservableCollection<Models.Movie> Movies { get; set; }
        public bool Loading { get; set; }
        public long InitialisationTime { get { return _stopwatch.ElapsedMilliseconds; } }

        public void StopwatchStop()
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                OnPropertyChanged("InitialisationTime");
            }
        }

        public Command ViewMovieDetail => new Command(async (param) => {
            if (param is int movieId)
                await this.PushPage<MovieDetail>(new MovieDetailViewModel(_apiKey, movieId));
        });

        /// <summary>
        /// For XML preview only
        /// </summary>
        public MovieList()
        {

        }

        public MovieList(string apiKey, Models.SearchResult results, Stopwatch stopwatch)
        {
            _apiKey = apiKey;
            _results = results;
            _stopwatch = stopwatch;
        }

        public override async Task Initialise()
        {
            Loading = true;

            Movies = new ObservableCollection<Models.Movie>(_results.Movies);

            Loading = false;

            await base.Initialise();
        }
    }
}
