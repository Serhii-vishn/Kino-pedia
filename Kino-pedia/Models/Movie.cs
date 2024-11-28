namespace Kino_pedia.Models;

public class Movie
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Uri { get; set; }
    public string? Title { get; set; }
    public string? ReleaseDate { get; set; }
    public string? Image { get; set; }

    public List<Actor> Actors { get; set; } = new List<Actor>();
}
