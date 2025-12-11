using System.Collections.Generic;
using UnityEngine;

public static class ToolsDatabase
{
    private static readonly Dictionary<ToolType, ToolInfo> data =
        new Dictionary<ToolType, ToolInfo>
        {
            {
                ToolType.Axe,
                new ToolInfo(
                    "Copper Axe",
                    "Improves wood chopping speed.\nHarvests +3 wood per swing.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 10),
                        new ResourceAmount(ResourceType.Stone, 5)
                    },
                    harvestAmount: 3
                )
            },

            {
                ToolType.Pickaxe,
                new ToolInfo(
                    "Copper Pickaxe",
                    "Improves stone mining efficiency.\nHarvests +4 stone per swing.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 5),
                        new ResourceAmount(ResourceType.Stone, 10)
                    },
                    harvestAmount: 4
                )
            },

            {
                ToolType.Boots,
                new ToolInfo(
                    "Silk Boots",
                    "Increases worker movement speed.\n+1.25x movement speed.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 8),
                        new ResourceAmount(ResourceType.Stone, 2)
                    },
                    movementSpeedBonus: 1.25f
                )
            }
        };

    public static ToolInfo Get(ToolType type)
    {
        if (data.TryGetValue(type, out var info))
            return info;

        Debug.LogWarning($"No ToolInfo defined for type {type}");
        return null;
    }
}
