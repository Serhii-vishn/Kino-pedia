namespace Kino_pedia.Models;

public class ActorIndexViewModel
{
    public List<Actor> Actors { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
}
