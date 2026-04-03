using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Models;
using Core.State;

namespace Core.Diagnostics;

public static class MapVisualizer
{
    public static string ToAsciiString(this DungeonMap map, int zLevel = 0, Dictionary<RoomType, char>? typeSymbols = null)
    {
        if (map.AllRooms.Count == 0) return "Map is empty.";

        var sb = new StringBuilder();

        int minX = map.AllRooms.Min(r => r.GridPosition.X);
        int maxX = map.AllRooms.Max(r => r.GridPosition.X);
        int minY = map.AllRooms.Min(r => r.GridPosition.Y);
        int maxY = map.AllRooms.Max(r => r.GridPosition.Y);

        sb.AppendLine($"--- Dungeon Map (Z: {zLevel}) ---");

        for (int y = maxY; y >= minY; y--)
        {
            for (int x = minX; x <= maxX; x++)
            {
                if (map.TryGetRoomAt(new Position(x, y, zLevel), out var room))
                {
                    char symbol = GetSymbol(room.Template, typeSymbols);
                    sb.Append($"[{symbol}]");

                    if (room.Connections.ContainsKey(Direction.East))
                        sb.Append("-");
                    else
                        sb.Append(" ");
                }
                else
                {
                    sb.Append("    ");
                }
            }
            sb.AppendLine();

            // 2. North/South Connections
            if (y > minY)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    if (map.TryGetRoomAt(new Position(x, y, zLevel), out var room) &&
                        room.Connections.ContainsKey(Direction.South))
                    {
                        sb.Append(" |  ");
                    }
                    else
                    {
                        sb.Append("    ");
                    }
                }
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    private static char GetSymbol(RoomTemplate template, Dictionary<RoomType, char>? typeSymbols)
    {
        if (typeSymbols != null && typeSymbols.TryGetValue(template.Type, out char customSymbol))
        {
            return customSymbol;
        }

        var horizontalExits = template.AvailableExits & ~(Direction.Up | Direction.Down);

        return horizontalExits switch
        {
            Direction.North | Direction.South | Direction.East | Direction.West => '┼',
            Direction.North | Direction.South => '│',
            Direction.East | Direction.West => '─',
            Direction.North | Direction.South | Direction.East => '├',
            Direction.North | Direction.South | Direction.West => '┤',
            Direction.East | Direction.West | Direction.North => '┴',
            Direction.East | Direction.West | Direction.South => '┬',
            Direction.North | Direction.East => '└',
            Direction.North | Direction.West => '┘',
            Direction.South | Direction.East => '┌',
            Direction.South | Direction.West => '┐',
            Direction.North => '^',
            Direction.South => 'v',
            Direction.East => '>',
            Direction.West => '<',
            Direction.None => '■',
            _ => '#'
        };
    }
}
