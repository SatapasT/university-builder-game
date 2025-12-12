using System.Collections.Generic;
using UnityEngine;

public enum RefineType
{
    None = 0,
    Iron = 1,
    Planks = 2,
}

public class RefineMaterialInfo
{
    public string Name { get; }
    public string Description { get; }

    public ResourceAmount[] InputCosts { get; }

    public ResourceAmount Output { get; }

    public BuildType[] RequiredBuildings { get; }

    public RefineMaterialInfo(
        string name,
        string description,
        ResourceAmount[] inputCosts,
        ResourceAmount output,
        BuildType[] requiredBuildings = null)
    {
        Name = name;
        Description = description;
        InputCosts = inputCosts ?? new ResourceAmount[0];
        Output = output;
        RequiredBuildings = requiredBuildings ?? new BuildType[0];
    }
}

public static class RefineMaterialDatabase
{
    private static readonly Dictionary<RefineType, RefineMaterialInfo> data =
        new Dictionary<RefineType, RefineMaterialInfo>
        {
            {
                RefineType.Iron,
                new RefineMaterialInfo(
                    "Iron",
                    "A strong metal used for higher tier tools and construction.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Stone, 5),
                    },
                    new ResourceAmount(ResourceType.Iron, 1),
                    requiredBuildings: new[]
                    {
                        BuildType.Motte
                    }
                )
            },

            {
                RefineType.Planks,
                new RefineMaterialInfo(
                    "Planks",
                    "Processed timber used for advanced building and equipment.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 5),
                    },
                    new ResourceAmount(ResourceType.Planks, 1),
                    requiredBuildings: new[]
                    {
                        BuildType.Courtyard
                    }
                )
            }
        };

    public static RefineMaterialInfo Get(RefineType type)
    {
        if (data.TryGetValue(type, out var info))
            return info;

        Debug.LogWarning($"No RefineMaterialInfo defined for type {type}");
        return null;
    }
}
