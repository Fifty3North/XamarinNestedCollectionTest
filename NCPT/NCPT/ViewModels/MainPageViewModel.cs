using F3N.YaMVVM.ViewModel;
using FFImageLoading;
using NestedCollectionPerformanceTest.Models;
using NestedCollectionPerformanceTest.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace NestedCollectionPerformanceTest.ViewModels
{
    public class MainPageViewModel : F3N.YaMVVM.ViewModel.PageViewModel
    {
        public string APIKey { get; set; } = "be4193a8e7f27a465486533fc95d2cea";

        public Command NavigateToListViewPage { get; }
        public Command NavigateToCollectionViewPage { get; }
        public Command NavigateToBindableLayoutPage { get; }
        public Command NavigateToBindableLayoutFlexPage { get; }
        public Command NavigateToMoviesByCompanyListView { get; }
        public Command NavigateToMoviesByCompanyBindableLayout { get; }
        public Command LoadMovies { get; }
        public Command LoadMoviesByCompany { get; }

        private Dictionary<ProductionCompany, SearchResult> _moviesByCompany = new Dictionary<ProductionCompany, SearchResult>();

        public bool Loading { get; private set; }
        public bool Ready { get; private set; }

        public bool MoviesByCompanyReady { get; private set; }
        public bool Preloading { get; private set; }
        public bool LoadingCompanies { get; private set; }

        private SearchResult _results;
        private MovieService _movieService;

        public MainPageViewModel()
        {
            FFImageLoading.Config.Configuration.Default.VerboseLogging = true;
            NavigateToListViewPage = new Command(async () => await Navigate<ListViewPage>());
            NavigateToCollectionViewPage = new Command(async () => await Navigate<CollectionViewPage>());
            NavigateToBindableLayoutPage = new Command(async () => await Navigate<BindableLayoutPage>());
            NavigateToBindableLayoutFlexPage = new Command(async () => await Navigate<BindableLayoutFlexPage>());
            
            NavigateToMoviesByCompanyListView = new Command(async () => await NavigateMoviesByCompany<MoviesByCompanyListView>());
            NavigateToMoviesByCompanyBindableLayout = new Command(async () => await NavigateMoviesByCompany<MoviesByCompanyBindableLayout>());

            LoadMovies = new Command(async () => {
                if (string.IsNullOrEmpty(APIKey))
                    await this.DisplayAlert("API Key","Oops, looks like you forgot to enter your API Key", "OK");
                else if (APIKey.Length < 24)
                    await this.DisplayAlert("API Key", "Oops, please enter a valid API Key", "OK");
                else
                    await LoadMovieDataAndPreloadImages();
            });

            LoadMoviesByCompany = new Command(async () => {
                if (string.IsNullOrEmpty(APIKey))
                    await this.DisplayAlert("API Key", "Oops, looks like you forgot to enter your API Key", "OK");
                else if (APIKey.Length < 24)
                    await this.DisplayAlert("API Key", "Oops, please enter a valid API Key", "OK");
                else
                    await LoadCompanies();
            });


        }
        private async Task NavigateMoviesByCompany<T>() where T : F3N.YaMVVM.Views.YamvvmPage, new()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await this.PushPage<T>(new ViewModels.MovieListByCompany(APIKey, _moviesByCompany, stopwatch));
        }

        private async Task Navigate<T>() where T : F3N.YaMVVM.Views.YamvvmPage, new()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await this.PushPage<T>(new ViewModels.MovieList(APIKey, _results, stopwatch));
        }

        public override async Task Initialise()
        {
            await base.Initialise();
        }

        async Task<KeyValuePair<ProductionCompany,SearchResult>> LookupMoviesForCompanyId(ProductionCompany company) => new KeyValuePair<ProductionCompany, SearchResult>(company, await _movieService.GetMoviesByCompany(company.Id));

        private async Task LoadCompanies()
        {
            LoadingCompanies = true;
            _movieService = new MovieService(APIKey);
            var companies = await _movieService.ProductionCompanies();
            LoadingCompanies = false;
            Loading = true;

            List<Task<KeyValuePair<ProductionCompany, SearchResult>>> tasks = new List<Task<KeyValuePair<ProductionCompany, SearchResult>>>();

            foreach(var company in companies)
            {
                tasks.Add(LookupMoviesForCompanyId(company));
                // API rate limit
                await Task.Delay(40);
            }

            var results = await Task.WhenAll(tasks);

            foreach (var result in results) {
                _moviesByCompany[result.Key] = result.Value;
            }

            Loading = false;
            Preloading = true;
            
            int posterWidth = 171;
            int posterHeight = 256;

            if (Xamarin.Forms.Device.Idiom == TargetIdiom.Phone)
            {
                posterWidth = 85;
                posterHeight = 128;
            }
            
            var preloadTasks = new List<Task>();
            
            try
            {
                foreach (var kvp in _moviesByCompany)
                {
                    foreach (var result in kvp.Value.Movies)
                    {
                        if (!string.IsNullOrEmpty(result.PosterPath))
                        {
                            System.Diagnostics.Debug.WriteLine("Loading: " + result.PosterPath);
                            preloadTasks.Add(ImageService.Instance.LoadUrl(result.PosterPath)
                                .DownSample(posterWidth)
                                .PreloadAsync());
                        }
                    }
                }

                await Task.WhenAll(preloadTasks);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            Preloading = false;
            MoviesByCompanyReady = true;
        }

        private async Task LoadMovieDataAndPreloadImages()
        {
            _movieService = new MovieService(APIKey);
            Loading = true;
            _results = await _movieService.GetMoviesFromYear(2018);
            Loading = false;
            Preloading = true;

            var tasks = new List<Task>();

            int posterWidth = 171;
            int posterHeight = 256;

            if (Xamarin.Forms.Device.Idiom == TargetIdiom.Phone)
            {
                posterWidth = 85;
                posterHeight = 128;
            }

            try
            {
                foreach (var result in _results.Movies)
                {
                    tasks.Add(ImageService.Instance.LoadUrl(result.PosterPath)
                        .DownSample(posterWidth)
                        .PreloadAsync());
                }

                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            Preloading = false;
            Ready = true;
        }
    }
}
