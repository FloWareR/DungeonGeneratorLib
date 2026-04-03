using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Builders;
using Core.Models;

namespace DungeonGenerator.Sandbox.Data;

public static class TemplateMapper
{
    public static IReadOnlyList<RoomTemplate> LoadFromJson(string filePath)
    {
        var json = File.ReadAllText(filePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        var dtos = JsonSerializer.Deserialize<List<RoomTemplateDto>>(json, options) ?? new();
        var templates = new List<RoomTemplate>();

        foreach (var dto in dtos)
        {
            Enum.TryParse<Direction>(dto.AvailableExits, true, out var exits);

            var builder = RoomTemplateBuilder.Create(dto.Id)
                .WithType(new RoomType(dto.Type))
                .WithExits(exits)
                .WithWeight(dto.SpawnWeight);

            foreach (var tag in dto.Tags)
            {
                builder.AddTag(new RoomTag(tag));
            }

            templates.Add(builder.Build());
        }

        return templates;
    }
}
