using Kino_pedia.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kino_pedia.Controllers;

public class HomeController : Controller
{
    private readonly IMovieRepository _movieRepository;

    public HomeController(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<IActionResult> Index(int limit = 100)
    {
        var movies = await _movieRepository.GetTopMovies(limit);

        return View(movies);
    }

    public async Task<IActionResult> MoviesByGenre(string genre = "", int limit = 100)
    {
        var movies = await _movieRepository.GetMoviesByName(genre, limit);
        return View(movies);
    }
}
