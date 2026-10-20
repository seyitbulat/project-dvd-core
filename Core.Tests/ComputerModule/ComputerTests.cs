
namespace Core.Tests.ComputerModule;

using Core.Computer;
using Core.Computer.Models;
using Xunit.Abstractions;

public class ComputerTests
{
    private readonly ITestOutputHelper output;

     public ComputerTests(ITestOutputHelper output)
    {
        this.output = output;
    }
    [Fact]
    public void Update_WithActiveDownloads_DoesNotAllocate()
    {
        double delta = 0.016;
        Computer cp = new(256 * 1024 * 1024, 100);

        var game1 = new Game("Cyberpunk", 39 * 1024 * 1024);
        var game2 = new Game("FF7", 68 * 1024 * 1024);

        cp.DownloadManager.AddGameToDownload(game1);
        cp.DownloadManager.AddGameToDownload(game2);

        cp.Update(delta);
        cp.Update(delta);
        cp.Update(delta);
        cp.Update(delta);
        cp.Update(delta);
        cp.Update(delta);

        var bf = GC.GetAllocatedBytesForCurrentThread();

        for (int i = 0; i < 1000; i++)
        {
            cp.Update(delta);
        }

        var af = GC.GetAllocatedBytesForCurrentThread();

        var result = af - bf;
        output.WriteLine($"Result: {result}");

        Assert.Equal(0, result);
    }
}