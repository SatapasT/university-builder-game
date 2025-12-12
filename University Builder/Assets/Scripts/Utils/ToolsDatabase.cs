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
                        "Rusted Hatchet",
                        "A battered old hatchet.\nHarvests +1 wood per swing.",
                        new ResourceAmount[] { },
                        harvestAmount: 1
                    ),
                    new ToolInfo(
                        "Bronze Axe",
                        "A sturdier edge and better balance.\nHarvests +3 wood per swing.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 10),
                            new ResourceAmount(ResourceType.Stone, 5)
                        },
                        harvestAmount: 3
                    ),
                    new ToolInfo(
                        "Iron Axe",
                        "A reliable iron blade for serious chopping.\nHarvests +6 wood per swing.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 25),
                            new ResourceAmount(ResourceType.Stone, 15)
                        },
                        harvestAmount: 6
                    ),
                    new ToolInfo(
                        "Steel Woodsman Axe",
                        "A hardened steel head that bites deep.\nHarvests +10 wood per swing.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 45),
                            new ResourceAmount(ResourceType.Stone, 30)
                        },
                        harvestAmount: 10
                    ),
                    new ToolInfo(
                        "Masterwork Axe",
                        "A master-forged axe fit for a guild champion.\nHarvests +15 wood per swing.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 80),
                            new ResourceAmount(ResourceType.Stone, 60)
                        },
                        harvestAmount: 15
                    ),
                }
            },

            // ---------------- PICKAXE ----------------
            {
                ToolType.Pickaxe,
                new List<ToolInfo>
                {
                    new ToolInfo(
                        "Cracked Pick",
                        "A dull, cracked pick.\nHarvests +1 stone per swing.",
                        new ResourceAmount[] { },
                        harvestAmount: 1
                    ),
                    new ToolInfo(
                        "Bronze Pickaxe",
                        "Cuts cleaner into rock.\nHarvests +4 stone per swing.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 5),
                            new ResourceAmount(ResourceType.Stone, 10)
                        },
                        harvestAmount: 4
                    ),
                    new ToolInfo(
                        "Iron Pickaxe",
                        "Stronger head, fewer bounces.\nHarvests +8 stone per swing.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 15),
                            new ResourceAmount(ResourceType.Stone, 25)
                        },
                        harvestAmount: 8
                    ),
                    new ToolInfo(
                        "Steel Miner’s Pick",
                        "Built for deep veins and hard rock.\nHarvests +13 stone per swing.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 25),
                            new ResourceAmount(ResourceType.Stone, 45)
                        },
                        harvestAmount: 13
                    ),
                    new ToolInfo(
                        "Masterwork Pickaxe",
                        "Perfectly weighted and wickedly sharp.\nHarvests +20 stone per swing.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 40),
                            new ResourceAmount(ResourceType.Stone, 80)
                        },
                        harvestAmount: 20
                    ),
                }
            },

            // ---------------- BOOTS ----------------
            {
                ToolType.Boots,
                new List<ToolInfo>
                {
                    new ToolInfo(
                        "Bare Feet",
                        "No boots.\n+1x movement speed.",
                        new ResourceAmount[] { },
                        movementSpeedBonus: 1.0f
                    ),
                    new ToolInfo(
                        "Leather Boots",
                        "Basic leather boots.\n+2x movement speed.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 10),
                            new ResourceAmount(ResourceType.Stone, 5)
                        },
                        movementSpeedBonus: 2.0f
                    ),
                    new ToolInfo(
                        "Reinforced Leather Boots",
                        "Stronger soles and tighter fit.\n+3x movement speed.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 20),
                            new ResourceAmount(ResourceType.Stone, 12)
                        },
                        movementSpeedBonus: 3.0f
                    ),
                    new ToolInfo(
                        "Steel-Plated Boots",
                        "Balanced for speed and protection.\n+4x movement speed.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 35),
                            new ResourceAmount(ResourceType.Stone, 25)
                        },
                        movementSpeedBonus: 4.0f
                    ),
                    new ToolInfo(
                        "Master Courier Boots",
                        "Legendary boots worn by royal messengers.\n+5x movement speed.",
                        new[]
                        {
                            new ResourceAmount(ResourceType.Wood, 60),
                            new ResourceAmount(ResourceType.Stone, 45)
                        },
                        movementSpeedBonus: 5.0f
                    ),
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
