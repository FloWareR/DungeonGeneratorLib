using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;

namespace Core.Extensions;

public static class RandomExtensions
{
    public static RoomTemplate GetRandomWeighted(this IEnumerable<RoomTemplate> templates, Random rng)
    {
        var templateList = templates.ToList();
        var totalWeight = templateList.Sum(t => Math.Max(0, t.SpawnWeight));

        if (totalWeight == 0)
        {
            return templateList[rng.Next(templateList.Count)];
        }

        var randomValue = rng.Next(totalWeight);
        var currentWeight = 0;

        foreach (var template in templateList)
        {
            currentWeight += Math.Max(0, template.SpawnWeight);
            if (randomValue < currentWeight)
            {
                return template;
            }
        }

        return templateList.Last();
    }
}
