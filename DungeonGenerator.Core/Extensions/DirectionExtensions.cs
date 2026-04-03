using System.Collections.Generic;
using Core.Models;
namespace Core.Extensions;

public static class DirectionExtensions
{
    private static readonly Direction[] _allDirections =
    {
      Direction.North, Direction.South, Direction.East,
      Direction.West, Direction.Up, Direction.Down
    };

    /// <summary>Returns the opposite direction. Supports bitwise masks</summary>
    public static Direction GetOpposite(this Direction direction)
    {
        var result = Direction.None;

        if ((direction & Direction.North) != 0) result |= Direction.South;
        if ((direction & Direction.South) != 0) result |= Direction.North;
        if ((direction & Direction.East) != 0) result |= Direction.West;
        if ((direction & Direction.West) != 0) result |= Direction.East;
        if ((direction & Direction.Up) != 0) result |= Direction.Down;
        if ((direction & Direction.Down) != 0) result |= Direction.Up;

        return result;
    }

    ///<summary>Returns indivual flags from a bitmask (Does not allocate)</summary>
    public static IEnumerable<Direction> GetActiveFlags(this Direction directions)
    {
        for (var i = 0; i < _allDirections.Length; i++)
        {
            var dir = _allDirections[i];
            if ((directions & dir) != 0)
            {
                yield return dir;
            }
        }
    }
}
