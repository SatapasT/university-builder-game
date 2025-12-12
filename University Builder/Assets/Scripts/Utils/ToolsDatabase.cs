using System.Collections.Generic;
using UnityEngine;
public class ToolInfo
{
    public string Name { get; }
    public string Description { get; }
    public ResourceAmount[] Costs { get; }

    public int HarvestAmount { get; }
    public float MovementSpeedBonus { get; }

    public BuildType[] RequiredBuildings { get; }

    public ToolInfo(
        string name,
        string description,
        ResourceAmount[] costs,
        int harvestAmount,
        float movementSpeedBonus,
        BuildType[] requiredBuildings)
    {
        Name = name;
        Description = description;
        Costs = costs;
        HarvestAmount = harvestAmount;
        MovementSpeedBonus = movementSpeedBonus;
        RequiredBuildings = requiredBuildings ?? new BuildType[0];
    }
}


public static class ToolsDatabase
{
    private static readonly Dictionary<ToolType, List<ToolInfo>> data =
        new Dictionary<ToolType, List<ToolInfo>>
        {
            // ================= AXE =================
            {
                ToolType.Axe,
                new List<ToolInfo>
                {
                    // L1 – BASIC
                    new ToolInfo(
                        "Rusted Hatchet",
                        "Harvests +1 wood.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 1),
                            new ResourceAmount(ResourceType.Stone, 1)
                        },
                        1,
                        1f,
                        new BuildType[] { }
                    ),

                    // L2 – GOLD
                    new ToolInfo(
                        "Bronze Axe",
                        "Harvests +3 wood.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 2),
                            new ResourceAmount(ResourceType.Stone, 2),
                            new ResourceAmount(ResourceType.Gold, 1)
                        },
                        3,
                        1f,
                        new BuildType[] { }
                    ),

                    // L3 – IRON (MOTTE)
                    new ToolInfo(
                        "Iron Axe",
                        "Harvests +6 wood.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 3),
                            new ResourceAmount(ResourceType.Stone, 3),
                            new ResourceAmount(ResourceType.Iron, 1)
                        },
                        6,
                        1f,
                        new[] { BuildType.Motte }
                    ),

                    // L4 – PLANKS (GRAND HALL)
                    new ToolInfo(
                        "Steel Axe",
                        "Harvests +10 wood.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 4),
                            new ResourceAmount(ResourceType.Stone, 4),
                            new ResourceAmount(ResourceType.Planks, 1)
                        },
                        10,
                        1f,
                        new[] { BuildType.GrandHall }
                    ),

                    // L5 – MASTERWORK (LIBRARY)
                    new ToolInfo(
                        "Masterwork Axe",
                        "Harvests +15 wood.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 5),
                            new ResourceAmount(ResourceType.Stone, 5),
                            new ResourceAmount(ResourceType.Planks, 2),
                            new ResourceAmount(ResourceType.Gold, 2)
                        },
                        15,
                        1f,
                        new[] { BuildType.Libary }
                    )
                }
            },

            // ================= PICKAXE =================
            {
                ToolType.Pickaxe,
                new List<ToolInfo>
                {
                    new ToolInfo(
                        "Cracked Pick",
                        "Harvests +1 stone.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 1),
                            new ResourceAmount(ResourceType.Stone, 1)
                        },
                        1,
                        1f,
                        new BuildType[] { }
                    ),

                    new ToolInfo(
                        "Bronze Pickaxe",
                        "Harvests +3 stone.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 2),
                            new ResourceAmount(ResourceType.Stone, 2),
                            new ResourceAmount(ResourceType.Gold, 1)
                        },
                        3,
                        1f,
                        new BuildType[] { }
                    ),

                    new ToolInfo(
                        "Iron Pickaxe",
                        "Harvests +6 stone.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 3),
                            new ResourceAmount(ResourceType.Stone, 3),
                            new ResourceAmount(ResourceType.Iron, 1)
                        },
                        6,
                        1f,
                        new[] { BuildType.Motte }
                    ),

                    new ToolInfo(
                        "Steel Pickaxe",
                        "Harvests +10 stone.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 4),
                            new ResourceAmount(ResourceType.Stone, 4),
                            new ResourceAmount(ResourceType.Planks, 1)
                        },
                        10,
                        1f,
                        new[] { BuildType.GrandHall }
                    ),

                    new ToolInfo(
                        "Masterwork Pickaxe",
                        "Harvests +15 stone.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 5),
                            new ResourceAmount(ResourceType.Stone, 5),
                            new ResourceAmount(ResourceType.Planks, 2),
                            new ResourceAmount(ResourceType.Gold, 2)
                        },
                        15,
                        1f,
                        new[] { BuildType.Libary }
                    )
                }
            },

            // ================= BOOTS =================
            {
                ToolType.Boots,
                new List<ToolInfo>
                {
                    new ToolInfo(
                        "Bare Feet",
                        "1x movement speed.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 1),
                            new ResourceAmount(ResourceType.Stone, 1)
                        },
                        0,
                        1f,
                        new BuildType[] { }
                    ),

                    new ToolInfo(
                        "Leather Boots",
                        "2x movement speed.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 2),
                            new ResourceAmount(ResourceType.Stone, 2),
                            new ResourceAmount(ResourceType.Gold, 1)
                        },
                        0,
                        2f,
                        new BuildType[] { }
                    ),

                    new ToolInfo(
                        "Iron-Toed Boots",
                        "3x movement speed.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 3),
                            new ResourceAmount(ResourceType.Stone, 3),
                            new ResourceAmount(ResourceType.Iron, 1)
                        },
                        0,
                        3f,
                        new[] { BuildType.Motte }
                    ),

                    new ToolInfo(
                        "Steel-Plated Boots",
                        "4x movement speed.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 4),
                            new ResourceAmount(ResourceType.Stone, 4),
                            new ResourceAmount(ResourceType.Planks, 1)
                        },
                        0,
                        4f,
                        new[] { BuildType.GrandHall }
                    ),

                    new ToolInfo(
                        "Master Courier Boots",
                        "5x movement speed.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 5),
                            new ResourceAmount(ResourceType.Stone, 5),
                            new ResourceAmount(ResourceType.Planks, 2),
                            new ResourceAmount(ResourceType.Gold, 2)
                        },
                        0,
                        5f,
                        new[] { BuildType.Libary }
                    )
                }
            }
        };

    public static ToolInfo Get(ToolType type, int level)
    {
        if (!data.TryGetValue(type, out var levels)) return null;
        if (level < 0 || level >= levels.Count) return null;
        return levels[level];
    }

    public static int GetMaxLevel(ToolType type)
    {
        if (!data.TryGetValue(type, out var levels)) return 0;
        return levels.Count - 1;
    }
}
