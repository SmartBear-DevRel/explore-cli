using Explore.Cli.Models;
using Explore.Cli.Models.Explore;

public static class MappingHelper
{
    public static Connection MassageConnectionExportForImport(Connection? exportedConnection)
    {
        if (exportedConnection == null)
        {
            return new Connection();
        }

        //connection type is not set on exports, yet it needed when sending back to Explore
        exportedConnection.Type = "ConnectionRequest";

        return exportedConnection;
    }

   
    public static Endpoint MassageEndpointExportForImport(Endpoint? exportedEndpoint)
    {
        if( exportedEndpoint == null)
        {
            return new Endpoint();
        }

        //connection type is not set on exports, yet it needed when sending back to Explore
        if( exportedEndpoint.Connection == null)
        {
            exportedEndpoint.Connection = new Connection();
        }

        exportedEndpoint.Connection.Type = "ConnectionRequest";

        return exportedEndpoint;
    } 


    // Helper to map StagedAPI into an ExploreContracts.APIRequestV2
    public static ApiRequestV2 MapStagedApiToApiRequestV2(StagedAPI stagedApi)
    {
        return new ApiRequestV2
        {
            Name = stagedApi.APIName.Substring(0, 60),
            ServerURLs = new string[] { stagedApi.APIUrl }
        };
    }

}