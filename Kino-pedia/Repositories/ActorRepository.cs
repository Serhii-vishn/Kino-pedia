using Kino_pedia.Models;
using Kino_pedia.Repositories.Interfaces;

namespace Kino_pedia.Repositories;

public class ActorRepository : IActorRepository
{
    private readonly SparqlDAL _sparqlDAL;

    public ActorRepository(SparqlDAL sparqlDAL)
    {
        _sparqlDAL = sparqlDAL;
    }

    public async Task<PaginatedList<Actor>> GetActors(string searchTerm = "", int pageIndex = 1, int pageSize = 10)
    {
        int offset = (pageIndex - 1) * pageSize;

        string filter = !string.IsNullOrEmpty(searchTerm)
            ? $"FILTER (regex(?name, \"{searchTerm}\", \"i\"))"
            : "";

        string countQuery = $@"
                PREFIX dbo: <http://dbpedia.org/ontology/>
                SELECT (COUNT(DISTINCT ?actor) as ?totalCount) WHERE {{
                    ?actor a dbo:Actor ;
                           rdfs:label ?name .
                    FILTER (lang(?name) = 'en')
                    {filter}
                }}";

        var countResult = await _sparqlDAL.ExecuteQueryAsync(countQuery, result => new
        {
            TotalCount = int.TryParse(CleanString(result["totalCount"].ToString()), out int count) ? count : 0
        });

        int totalCount = countResult.FirstOrDefault()?.TotalCount ?? 0;

        string sparqlQuery = $@"
                PREFIX dbo: <http://dbpedia.org/ontology/>
                PREFIX dbr: <http://dbpedia.org/resource/>
                SELECT DISTINCT ?actor (STR(?name) AS ?cleanName) ?birthDate ?image WHERE {{
                    ?actor a dbo:Actor ;
                            rdfs:label ?name ;
                            dbo:birthDate ?birthDate ;
                            dbo:thumbnail ?image .
                    FILTER (lang(?name) = 'en')
                }}
                LIMIT {pageSize} OFFSET {offset}";

        var actors = await _sparqlDAL.ExecuteQueryAsync(sparqlQuery, result => new Actor
        {
            Id = Guid.NewGuid(),
            Uri = CleanString(result["actor"].ToString()),
            FullName = CleanString(result["cleanName"].ToString()) ?? "Unknown Actor",
            BithDate = CleanString(result["birthDate"].ToString()) ?? "-",
            Image = CleanString(result["image"].ToString())
        });

        return await PaginatedList<Actor>.CreateAsync(actors, totalCount, pageIndex, pageSize);
    }

    public async Task<List<Actor>> GetActorsByMovie(string movieName)
    {
        string sparqlQuery = $@"
            PREFIX dbo: <http://dbpedia.org/ontology/>
            PREFIX dbr: <http://dbpedia.org/resource/>
            SELECT DISTINCT ?actor ?actorName ?actorImage ?actorBirthDate WHERE {{
                ?movie dbo:starring ?actor .
                ?actor rdfs:label ?actorName .
                ?actor dbo:thumbnail ?actorImage .
                OPTIONAL {{ ?actor dbo:birthDate ?actorBirthDate }}
                FILTER (lang(?actorName) = 'en')
                FILTER (regex(?movie, '^{movieName}', 'i'))
            }}";

        return await _sparqlDAL.ExecuteQueryAsync(sparqlQuery, actorResult => new Actor
        {
            Id = Guid.NewGuid(),
            FullName = actorResult["actorName"]?.ToString(),
            BithDate = actorResult["actorBirthDate"]?.ToString(),
            Image = actorResult["actorImage"]?.ToString()
        });
    }

    private string CleanString(string input) => input?.Split("^^")[0].Trim() ?? string.Empty;
}
