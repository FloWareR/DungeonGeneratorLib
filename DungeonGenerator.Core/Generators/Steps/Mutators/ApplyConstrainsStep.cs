using System;
using System.Collections.Generic;
using System.Linq;
using Core.Interfaces;
using Core.Models;
using Core.State;
using Core.Extensions;

namespace Core.Generators.Steps.Mutators;

public class ApplyConstraintsStep : IDungeonGenerationStep
{
    public void Execute(DungeonGenerationContext context)
    {
        var normalRooms = context.Map.AllRooms.Where(r => r.Template.Type == "Normal").ToList();
        var startRoom = context.Map.GetRoomByType(context.Config.StartRoomType);
        var roomDistances = startRoom != null ? CalculateTopologicalDistances(startRoom, context.Map) : new Dictionary<Guid, int>();

        foreach (var limit in context.Config.TypeLimits)
        {
            var type = limit.Key;
            var bounds = limit.Value;

            if (type == context.Config.StartRoomType)
            {
                continue;
            }

            var currentCount = context.Map.AllRooms.Count(r => r.Template.Type == type);
            var targetCount = context.Rng.Next(bounds.Min, bounds.Max + 1);
            var neededCount = targetCount - currentCount;

            var availableTemplates = context.Templates.Where(t => t.Type == type).ToList();

            if (availableTemplates.Count == 0 || neededCount <= 0)
            {
                continue;
            }

            for (var i = 0; i < neededCount; i++)
            {
                if (normalRooms.Count == 0)
                {
                    break;
                }

                var candidateRooms = normalRooms.ToList();

                if (type == context.Config.ObjectiveRoomType && startRoom != null)
                {
                    candidateRooms = FilterRoomsByDistance(candidateRooms, roomDistances, context.Config.MinObjectiveDistance, context.Config.MaxObjectiveDistance);

                    if (candidateRooms.Count == 0)
                    {
                        candidateRooms = GetFurthestAvailableRooms(normalRooms, roomDistances);
                    }
                }

                if (candidateRooms.Count == 0)
                {
                    break;
                }

                var roomToMutate = candidateRooms[context.Rng.Next(candidateRooms.Count)];
                var newTemplate = availableTemplates.GetRandomWeighted(context.Rng);

                roomToMutate.SetTemplate(newTemplate);
                normalRooms.Remove(roomToMutate);
            }
        }
    }

    private Dictionary<Guid, int> CalculateTopologicalDistances(Room startRoom, DungeonMap map)
    {
        var distances = new Dictionary<Guid, int>();
        var roomsById = map.AllRooms.ToDictionary(r => r.InstanceId);
        var queue = new Queue<(Room Room, int Distance)>();

        queue.Enqueue((startRoom, 0));
        distances[startRoom.InstanceId] = 0;

        while (queue.Count > 0)
        {
            var (current, dist) = queue.Dequeue();

            foreach (var connectedId in current.Connections.Values)
            {
                if (!distances.ContainsKey(connectedId) && roomsById.TryGetValue(connectedId, out var neighbor))
                {
                    distances[connectedId] = dist + 1;
                    queue.Enqueue((neighbor, dist + 1));
                }
            }
        }

        return distances;
    }

    private List<Room> FilterRoomsByDistance(List<Room> rooms, Dictionary<Guid, int> distances, int min, int max)
    {
        return rooms.Where(r => distances.TryGetValue(r.InstanceId, out int dist) && dist >= min && dist <= max).ToList();
    }

    private List<Room> GetFurthestAvailableRooms(List<Room> rooms, Dictionary<Guid, int> distances)
    {
        var maxDistance = rooms.Max(r => distances.GetValueOrDefault(r.InstanceId, 0));
        return rooms.Where(r => distances.GetValueOrDefault(r.InstanceId, 0) == maxDistance).Take(1).ToList();
    }
}
