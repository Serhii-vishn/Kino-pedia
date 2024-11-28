using Kino_pedia.Models;
using Kino_pedia.Repositories.Interfaces;
using VDS.RDF.Query;

namespace Kino_pedia.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly SparqlDAL _sparqlDAL;

    private readonly IActorRepository _actorRepository;

    public MovieRepository(SparqlDAL sparqlDAL, IActorRepository actorRepository)
    {
        _sparqlDAL = sparqlDAL;
        _actorRepository = actorRepository;
    }

    public async Task<IEnumerable<Movie>> GetMoviesByName(string name, int limit)
    {
        string filter = !string.IsNullOrEmpty(name)
            ? $"FILTER (strStarts(lcase(?title), lcase(\"{name}\")))"
            : "";

        string sparqlQuery = $@"
                PREFIX dbo: <http://dbpedia.org/ontology/>
                PREFIX dbr: <http://dbpedia.org/resource/>
                SELECT DISTINCT ?movie ?plainTitle ?releaseDateStr ?imageStr ?actor WHERE {{
                    ?movie a dbo:Film ;
                           rdfs:label ?title ;
                           dbo:releaseDate ?releaseDate ;
                           dbo:thumbnail ?image ;
                           dbo:starring ?actor .
                    FILTER (lang(?title) = 'en')
                    {filter}
                    BIND(STR(?title) AS ?plainTitle)
                    BIND(STR(?releaseDate) AS ?releaseDateStr)
                    BIND(STR(?image) AS ?imageStr)
                }}
                LIMIT {limit}";

        var movies = await _sparqlDAL.ExecuteQueryAsync(sparqlQuery, result => new Movie
        {
            Id = Guid.NewGuid(),
            Uri = result["movie"]?.ToString(),
            Title = CleanString(result["plainTitle"].ToString()) ?? "Unknown Movie",
            ReleaseDate = CleanString(result["releaseDateStr"].ToString()) ?? "-",
            Image = CleanString(result["imageStr"].ToString())
        });

        return movies.DistinctBy(m => m.Uri);
    }

    public async Task<List<Movie>> GetTopMovies(int limit_movies)
    {
        string sparqlQuery = $@"
                PREFIX dbo: <http://dbpedia.org/ontology/>
                PREFIX dbr: <http://dbpedia.org/resource/>
                SELECT DISTINCT ?movie ?plainTitle ?releaseDateStr ?imageStr ?actor WHERE {{
                    ?movie a dbo:Film ;
                           rdfs:label ?title ;
                           dbo:releaseDate ?releaseDate ;
                           dbo:thumbnail ?image ;
                           dbo:starring ?actor .
                    FILTER (lang(?title) = 'en')
                    FILTER (BOUND(?image))
                    FILTER (BOUND(?actor))  # Ensure that the actor is bound
                    BIND(STR(?title) AS ?plainTitle)
                    BIND(STR(?releaseDate) AS ?releaseDateStr)
                    BIND(STR(?image) AS ?imageStr)
                }}
                ORDER BY DESC(?releaseDate)
                LIMIT {limit_movies}";

        var movies = await _sparqlDAL.ExecuteQueryAsync(sparqlQuery, result => new Movie
        {
            Id = Guid.NewGuid(),
            Uri = result["movie"]?.ToString(),
            Title = CleanString(result["plainTitle"].ToString()) ?? "Unknown Movie",
            ReleaseDate = CleanString(result["releaseDateStr"].ToString()) ?? "-",
            Image = CleanString(result["imageStr"].ToString()) 
        });

        return movies;
    }

    private string CleanString(string input) => input?.Split("^^")[0].Trim() ?? string.Empty;
}
