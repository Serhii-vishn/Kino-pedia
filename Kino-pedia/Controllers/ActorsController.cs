using Kino_pedia.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kino_pedia.Controllers
{
    public class ActorsController : Controller
    {
        private readonly IActorRepository _actorRepository;

        public ActorsController(IActorRepository actorRepository)
        {
            _actorRepository = actorRepository;
        }
        public async Task<IActionResult> Index(string searchTerm = "", int pageIndex = 1)
        {
            int pageSize = 20;
            var actors = await _actorRepository.GetActors(searchTerm, pageIndex, pageSize);

            ViewData["CurrentFilter"] = searchTerm;

            return View(actors);
        }
    }
}
