using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.Trending;

namespace NestedCollectionPerformanceTest.Services
{
    public class MovieService
    {
        TMDbClient _client;

        public MovieService(string apiKey)
        {
            _client = new TMDbClient(apiKey);
        }

        public async Task<Models.SearchResult> Movies()
        {
            var movieResults = await _client.GetTrendingMoviesAsync(TimeWindow.Week);
            return new Models.SearchResult(movieResults.Results.Select(m => Mapper(m)).ToList(), movieResults.Page, movieResults.TotalPages, movieResults.TotalResults);
        }

        public async Task<Models.SearchResult> GetMoviesFromYear(int year)
        {
            var pages = new List<Task<TMDbLib.Objects.General.SearchContainer<TMDbLib.Objects.Search.SearchMovie>>>();

            for (int page = 0; page < 5; page++)
            {
                pages.Add(_client.DiscoverMoviesAsync()
                    .WhereAnyReleaseDateIsInYear(year)
                    .IncludeAdultMovies(false)
                    .IncludeVideoMovies(false)
                    .Query(page));
            }

            var result = await Task.WhenAll(pages);

            return new Models.SearchResult(result.SelectMany(s => s.Results).Select(m => Mapper(m)).ToList(), 0, result.Last().TotalPages, result.Last().TotalResults);
        }
        public async Task<Models.SearchResult> GetMoviesByCompany(int companyId)
        {
            var pages = new List<Task<TMDbLib.Objects.General.SearchContainer<TMDbLib.Objects.Search.SearchMovie>>>();
            var result = await _client.GetCompanyMoviesAsync(companyId);
            if (result == null || result.Results == null) return new Models.SearchResult(new List<Models.Movie>(),0,0,0);
            return new Models.SearchResult(result.Results.Select(m => Mapper(m)).ToList(), 0, result.TotalPages, result.TotalResults);
        }

        public async Task<List<Models.ProductionCompany>> ProductionCompanies()
        {
            List<Models.ProductionCompany> results = new List<Models.ProductionCompany>();
            var pages = new List<Task<TMDbLib.Objects.General.SearchContainer<TMDbLib.Objects.Search.SearchCompany>>>();

            for (int i = 0; i < 20; i++)
            {
                var companyId = i + 1;
                var company = await _client.GetCompanyAsync(companyId);
                
                await Task.Delay(80);

                if (company != null)
                {
                    results.Add(Mapper(company));
                } 
                else
                {
                    System.Diagnostics.Debug.WriteLine($"No company found for id: {companyId}");
                }
            }
            
            return results;
        }

        public async Task<Models.Movie> MovieDetail(int id)
        {
            var movie = await _client.GetMovieAsync(id);
            return Mapper(movie);
        }

        public async Task<Models.MovieImages> MovieImages(int id)
        {
            var movieImages = await _client.GetMovieImagesAsync(id);
            Models.MovieImages images = new Models.MovieImages();
            images.Backdrops = movieImages.Backdrops.Select(i => "http://image.tmdb.org/t/p/w1280" + i.FilePath).ToList();
            images.Posters = movieImages.Posters.Select(i => "http://image.tmdb.org/t/p/w780" + i.FilePath).ToList();
            return images;
        }

        public Models.Movie Mapper(TMDbLib.Objects.Movies.Movie movie)
        {
            return new Models.Movie {
                Adult = movie.Adult,
                BackdropPath = string.IsNullOrEmpty(movie.BackdropPath) ? string.Empty : "http://image.tmdb.org/t/p/w1280" + movie.BackdropPath,
                BelongsToCollection = movie.BelongsToCollection,
                Budget = movie.Budget,
                Genres = movie.Genres.Select(g => Mapper(g)).ToArray(),
                Homepage = movie.Homepage,
                Id = movie.Id,
                ImdbId = movie.ImdbId,
                OriginalLanguage = movie.OriginalLanguage,
                OriginalTitle = movie.OriginalTitle,
                Overview = movie.Overview,
                Popularity = movie.Popularity,
                PosterPath = string.IsNullOrEmpty(movie.PosterPath) ? string.Empty : "http://image.tmdb.org/t/p/w780" + movie.PosterPath,
                ProductionCompanies = movie.ProductionCompanies.Select(c => Mapper(c)).ToArray(),
                ProductionCountries = movie.ProductionCountries.Select(c => Mapper(c)).ToArray(),
                ReleaseDate = movie.ReleaseDate.Value,
                Revenue = movie.Revenue,
                Runtime = movie.Runtime.HasValue ? movie.Runtime.Value : 0,
                SpokenLanguages = movie.SpokenLanguages.Select(l => Mapper(l)).ToArray(),
                Status = movie.Status,
                Tagline = movie.Tagline,
                Title = movie.Title,
                Video = movie.Video,
                VoteAverage = movie.VoteAverage,
                VoteCount = movie.VoteCount
            };
        }

        public Models.Genre Mapper(TMDbLib.Objects.General.Genre genre)
        {
            return new Models.Genre
            {
                Id = genre.Id,
                Name = genre.Name
            };
        }

        public Models.ProductionCountry Mapper(TMDbLib.Objects.Movies.ProductionCountry country)
        {
            return new Models.ProductionCountry
            {
                Iso3166_1 = country.Iso_3166_1,
                Name = country.Name
            };
        }


        public Models.ProductionCompany Mapper(TMDbLib.Objects.Movies.ProductionCompany company)
        {
            return new Models.ProductionCompany
            {
                Id = company.Id,
                Name = company.Name
            };
        }

        public Models.ProductionCompany Mapper(TMDbLib.Objects.Search.SearchCompany company)
        {
            return new Models.ProductionCompany
            {
                Id = company.Id,
                LogoPath = "https://image.tmdb.org/t/p/w500" + company.LogoPath,
                Name = company.Name
            };
        }

        public Models.ProductionCompany Mapper(TMDbLib.Objects.Companies.Company company)
        {
            return new Models.ProductionCompany
            {
                Id = company.Id,
                LogoPath = "https://image.tmdb.org/t/p/w500" + company.LogoPath,
                Name = company.Name
            };
        }

        public Models.SpokenLanguage Mapper(TMDbLib.Objects.Movies.SpokenLanguage language)
        {
            return new Models.SpokenLanguage
            {
                Iso639_1 = language.Iso_639_1,
                Name = language.Name
            };
        }

        public Models.Movie Mapper(TMDbLib.Objects.Search.SearchMovie movie)
        {
            return new Models.Movie
            {
                Adult = movie.Adult,
                BackdropPath = movie.BackdropPath,
                Genres = movie.GenreIds.Select(g => new Models.Genre { Id = g }).ToArray(),
                Id = movie.Id,
                OriginalLanguage = movie.OriginalLanguage,
                OriginalTitle = movie.OriginalTitle,
                Overview = movie.Overview,
                Popularity = movie.Popularity,
                PosterPath = string.IsNullOrEmpty(movie.PosterPath) ? string.Empty : "http://image.tmdb.org/t/p/w342" + movie.PosterPath,
                ReleaseDate = movie.ReleaseDate.HasValue ? movie.ReleaseDate.Value : DateTime.MinValue,
                Title = movie.Title,
                Video = movie.Video,
                VoteAverage = movie.VoteAverage,
                VoteCount = movie.VoteCount
            };
        }
    }
}
