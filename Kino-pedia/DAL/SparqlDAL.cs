using VDS.RDF.Query;

public class SparqlDAL
{
    private readonly string sparqlEndpoint;

    public SparqlDAL(string sparqlEndpoint)
    {
        this.sparqlEndpoint = sparqlEndpoint;
    }

    public async Task<List<T>> ExecuteQueryAsync<T>(string sparqlQuery, Func<ISparqlResult, T> mapResult)
    {
        try
        {
            var endpoint = new SparqlRemoteEndpoint(new Uri(sparqlEndpoint));
            var resultSet = await Task.Run(() => endpoint.QueryWithResultSet(sparqlQuery));

            var results = new List<T>();
            foreach (var result in resultSet)
            {
                results.Add(mapResult(result));
            }

            return results;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error executing SPARQL query: {ex.Message}", ex);
        }
    }
}
