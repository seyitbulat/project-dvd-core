

using Core.GameModule;

namespace Core.DiscModule;

public readonly record struct DiscKey
{

    public DiscType Type { get; }
    public Game? Game { get; }
    public int Part { get; }
    public bool IsBlank => Game is null;

    private DiscKey(DiscType type, Game? game, int part)
    {
        Type = type;
        Game = game;
        Part = part;
    }

    public static DiscKey Blank(DiscType type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return new DiscKey(type, null, 0);
    }


    public static DiscKey Burned(DiscType type, Game game, int part)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(game);

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(part);

        var gameDisc = type.GetCountAndSize(game);

        ArgumentOutOfRangeException.ThrowIfGreaterThan(part, gameDisc.Piece);

        return new DiscKey(type, game, part);
    }
}