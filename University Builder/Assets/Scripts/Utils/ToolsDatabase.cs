using System.Collections.Generic;
using UnityEngine;

public static class ToolsDatabase
{
    private static readonly Dictionary<ToolType, List<ToolInfo>> data =
        new Dictionary<ToolType, List<ToolInfo>>
        {
            // ---------------- AXE ----------------
            {
                ToolType.Axe,
                new List<ToolInfo>
                {
                    new ToolInfo(
                        "Basic Axe",
                        "A worn-out axe.\nHarvests +1 wood per swing.",
                        new ResourceAmount[] { },     
                        harvestAmount: 1
                    ),
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
                }
            },

            // ---------------- PICKAXE ----------------
            {
                ToolType.Pickaxe,
                new List<ToolInfo>
                {
                    new ToolInfo(
                        "Basic Pickaxe",
                        "A dull pickaxe.\nHarvests +1 stone per swing.",
                        new ResourceAmount[] { },
                        harvestAmount: 1
                    ),
                    new ToolInfo(
                        "Copper Pickaxe",
                        "Improves stone mining.\nHarvests +4 stone per swing.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 5),
                            new ResourceAmount(ResourceType.Stone, 10)
                        },
                        harvestAmount: 4
                    )
                }
            },

            // ---------------- BOOTS ----------------
            {
                ToolType.Boots,
                new List<ToolInfo>
                {
                    new ToolInfo(
                        "Bare Feet",
                        "No boots. Normal movement speed.",
                        new ResourceAmount[] { },
                        movementSpeedBonus: 1.0f
                    ),
                    new ToolInfo(
                        "Silk Boots",
                        "Increases movement speed.\n+1.25x movement speed.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 8),
                            new ResourceAmount(ResourceType.Stone, 2)
                        },
                        movementSpeedBonus: 1.25f
                    )
                }
            }
        };

    public static ToolInfo Get(ToolType type, int level)
    {
        if (!data.TryGetValue(type, out var levels))
        {
            Debug.LogWarning($"No tools defined for type {type}");
            return null;
        }

        if (level < 0 || level >= levels.Count)
            return null;

        return levels[level];
    }

    public static int GetMaxLevel(ToolType type)
    {
        if (!data.TryGetValue(type, out var levels))
            return 0;

        return levels.Count - 1;
    }
}
