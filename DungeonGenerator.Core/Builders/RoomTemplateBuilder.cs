using System.Collections.Generic;
using Core.Models;

namespace Core.Builders;

public class RoomTemplateBuilder
{
    private string _id = string.Empty;
    private string _name = string.Empty;
    private RoomType _type = new("Default");
    private readonly List<RoomTag> _tags = new();
    private Direction _exits = Direction.None;
    private int _weight = 1;

    public static RoomTemplateBuilder Create(string id) => new() { _id = id };

    public RoomTemplateBuilder WithName(string name) { _name = name; return this; }
    public RoomTemplateBuilder WithType(RoomType type) { _type = type; return this; }
    public RoomTemplateBuilder AddTag(RoomTag tag) { _tags.Add(tag); return this; }
    public RoomTemplateBuilder AddTags(params RoomTag[] tags) { _tags.AddRange(tags); return this; }
    public RoomTemplateBuilder WithExits(Direction exits) { _exits = exits; return this; }
    public RoomTemplateBuilder WithWeight(int weight) { _weight = weight; return this; }

    public RoomTemplate Build()
    {
        return new RoomTemplate
        {
            Id = _id,
            Name = string.IsNullOrEmpty(_name) ? _id : _name,
            Type = _type,
            Tags = _tags,
            AvailableExits = _exits,
            SpawnWeight = _weight
        };
    }
}
