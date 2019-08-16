using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace NestedCollectionPerformanceTest.Models
{
    // Generated using https://app.quicktype.io
    public class Movie
    {
        public bool Adult { get; set; }
        public string BackdropPath { get; set; }
        public object BelongsToCollection { get; set; }
        public long Budget { get; set; }
        public Genre[] Genres { get; set; }
        public object Homepage { get; set; }
        public int Id { get; set; }
        public string ImdbId { get; set; }
        public string OriginalLanguage { get; set; }
        public string OriginalTitle { get; set; }
        public string Overview { get; set; }
        public double Popularity { get; set; }
        public string PosterPath { get; set; }
        public ProductionCompany[] ProductionCompanies { get; set; }
        public ProductionCountry[] ProductionCountries { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }
        public long Revenue { get; set; }
        public long Runtime { get; set; }
        public SpokenLanguage[] SpokenLanguages { get; set; }
        public string Status { get; set; }
        public string Tagline { get; set; }
        public string Title { get; set; }
        public bool Video { get; set; }
        public double VoteAverage { get; set; }
        public long VoteCount { get; set; }
    }

    public class MovieImages
    {
        public List<string> Backdrops { get; set; }
        public List<string> Posters { get; set; }
    }

    public partial class Genre
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }

    public partial class ProductionCompany
    {
        public int Id { get; set; }
        public string LogoPath { get; set; }
        public string Name { get; set; }
        public string OriginCountry { get; set; }
    }

    public partial class ProductionCountry
    {
        public string Iso3166_1 { get; set; }
        public string Name { get; set; }
    }

    public partial class SpokenLanguage
    {
        public string Iso639_1 { get; set; }
        public string Name { get; set; }
    }

}
