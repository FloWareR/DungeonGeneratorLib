namespace Core.Models;

public readonly record struct Position(int X, int Y, int Z)
{
    public static Position Zero => new(0, 0, 0);

    public Position Move(Direction direction) => direction switch
    {
        Direction.North => this with { Y = Y + 1 },
        Direction.South => this with { Y = Y - 1 },
        Direction.East => this with { X = X + 1 },
        Direction.West => this with { X = X - 1 },
        Direction.Up => this with { Z = Z + 1 },
        Direction.Down => this with { Z = Z - 1 },
        _ => this
    };
}
