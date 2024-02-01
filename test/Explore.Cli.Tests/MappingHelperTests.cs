using Explore.Cli.Models;

namespace Explore.Cli.Tests;

public class MappingHelperTests
{
    [Fact]
    public static void MassageConnectionExportForImport_Should_Pass()
    {
        var sut = new Connection();

        var act = MappingHelper.MassageConnectionExportForImport(sut);

        Assert.Equal("ConnectionRequest", sut.Type);
    }
}