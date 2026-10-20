using Core.GameModule;
using Xunit.Abstractions;

namespace Core.Tests.GameModule;



public class GameCatalogTests
{
   
    // yeni oyunu basariyla ekledi
    [Fact]
    public void Add_NewId_ReturnsTrue()
    {
        var gc = new GameCatalog();

        var game = new Game(1, "Cyberpunk", 200 * 1024);
        var result = gc.Add(game);

        Assert.True(result);
    }

    // ayni id'li oyunu eklemeye calisti
    [Fact]
    public void Add_ExistingId_ReturnsFalse()
    {
        var gc = new GameCatalog();

        var game = new Game(1, "Cyberpunk", 200 * 1024);
        var firstRes = gc.Add(game);



        var game1 = new Game(1, "Spiderman", 300 * 1024);

        var result = gc.Add(game1);

        Assert.True(firstRes);
        Assert.False(result);
    }

    // id'den var olan oyunu getirdi
    [Fact]
    public void TryGet_GameById_ReturnsGame()
    {
        var gc = new GameCatalog();

        var game = new Game(1, "Cyberpunk", 200 * 1024);
        gc.Add(game);
        
        var result = gc.TryGet(1, out var gameTest);

        Assert.Same(game, gameTest);
    }

    // id'den var olmayan oyunu getir
    [Fact]
    public void TryGet_UnknownId_ReturnsFalse()
    {
         var gc = new GameCatalog();

        var game = new Game(1, "Cyberpunk", 200 * 1024);
        gc.Add(game);
        
        var result = gc.TryGet(3, out var gameTest);

        Assert.False(result);
    }
}