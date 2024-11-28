using Kino_pedia.Models;

namespace Kino_pedia.Repositories.Interfaces;

public interface IMovieRepository
{
    Task<List<Movie>> GetTopMovies(int limit_movies);
    Task<IEnumerable<Movie>> GetMoviesByName(string genre, int limit);
}
