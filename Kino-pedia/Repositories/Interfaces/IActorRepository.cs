using Kino_pedia.Models;

namespace Kino_pedia.Repositories.Interfaces;

public interface IActorRepository
{
    Task<List<Actor>> GetActorsByMovie(string movieName);
    Task<PaginatedList<Actor>> GetActors(string searchTerm, int pageIndex, int pageSize);
}
