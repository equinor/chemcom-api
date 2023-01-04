using Microsoft.Graph;

namespace ChemDec.Api.GraphApi
{
    public interface IGraphServiceProvider
    {
        GraphServiceClient GetGraphServiceClient(string[] scopes);
    }
}