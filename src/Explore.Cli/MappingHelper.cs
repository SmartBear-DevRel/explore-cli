using Explore.Cli.Models;

public static class MappingHelper
{
    public static Connection MassageConnectionExportForImport(Connection? exportedConnection)
    {
        if(exportedConnection == null)
        {
            return new Connection();
        }
        
        //connection type is not set on exports, yet it needed when sending back to Explore
        exportedConnection.Type = "ConnectionRequest";
        
        return exportedConnection;
    }

}