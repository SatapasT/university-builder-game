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
        // ================= AXE (WOOD) =================
        {
            ToolType.Axe,
            new List<ToolInfo>
            {
                // L0 – STARTER
                new ToolInfo(
                    "Rusted Hatchet",
                    "A basic hatchet. Harvests +1 wood per action.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 15),
                        new ResourceAmount(ResourceType.Stone, 5)
                    },
                    harvestAmount: 1,
                    movementSpeedBonus: 1f,
                    requiredBuildings: new BuildType[] { }
                ),

                // L1 – BRONZE
                new ToolInfo(
                    "Bronze Axe",
                    "A sturdier edge. Harvests +5 wood per action.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 40),
                        new ResourceAmount(ResourceType.Stone, 20),
                        new ResourceAmount(ResourceType.Gold, 10)
                    },
                    harvestAmount: 5,
                    movementSpeedBonus: 1f,
                    requiredBuildings: new BuildType[] { }
                ),

                // L2 – IRON
                new ToolInfo(
                    "Iron Axe",
                    "Reliable iron head. Harvests +10 wood per action.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 80),
                        new ResourceAmount(ResourceType.Stone, 40),
                        new ResourceAmount(ResourceType.Iron, 10),
                        new ResourceAmount(ResourceType.Gold, 25)
                    },
                    harvestAmount: 10,
                    movementSpeedBonus: 1f,
                    requiredBuildings: new[] { BuildType.Motte }
                ),

                // L3 – STEEL
                new ToolInfo(
                    "Steel Axe",
                    "Balanced and sharp. Harvests +15 wood per action.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 140),
                        new ResourceAmount(ResourceType.Stone, 80),
                        new ResourceAmount(ResourceType.Planks, 25),
                        new ResourceAmount(ResourceType.Iron, 25),
                        new ResourceAmount(ResourceType.Gold, 75)
                    },
                    harvestAmount: 15,
                    movementSpeedBonus: 1f,
                    requiredBuildings: new[] { BuildType.GrandHall }
                ),

                // L4 – MASTERWORK (requires Libary)
                new ToolInfo(
                    "Masterwork Axe",
                    "A master-crafted tool. Harvests +20 wood per action.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 220),
                        new ResourceAmount(ResourceType.Stone, 140),
                        new ResourceAmount(ResourceType.Planks, 60),
                        new ResourceAmount(ResourceType.Iron, 60),
                        new ResourceAmount(ResourceType.Gold, 250)
                    },
                    harvestAmount: 20,
                    movementSpeedBonus: 1f,
                    requiredBuildings: new[] { BuildType.Libary }
                )
            }
        },

        // ================= PICKAXE (STONE) =================
        {
            ToolType.Pickaxe,
            new List<ToolInfo>
            {
                // L0 – STARTER
                new ToolInfo(
                    "Cracked Pick",
                    "A cheap pick. Harvests +1 stone per action.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 10),
                        new ResourceAmount(ResourceType.Stone, 15)
                    },
                    harvestAmount: 1,
                    movementSpeedBonus: 1f,
                    requiredBuildings: new BuildType[] { }
                ),

                // L1 – BRONZE
                new ToolInfo(
                    "Bronze Pickaxe",
                    "Better leverage. Harvests +5 stone per action.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 30),
                        new ResourceAmount(ResourceType.Stone, 45),
                        new ResourceAmount(ResourceType.Gold, 10)
                    },
                    harvestAmount: 5,
                    movementSpeedBonus: 1f,
                    requiredBuildings: new BuildType[] { }
                ),

                // L2 – IRON
                new ToolInfo(
                    "Iron Pickaxe",
                    "Harder head, fewer chips. Harvests +10 stone per action.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 60),
                        new ResourceAmount(ResourceType.Stone, 90),
                        new ResourceAmount(ResourceType.Iron, 10),
                        new ResourceAmount(ResourceType.Gold, 25)
                    },
                    harvestAmount: 10,
                    movementSpeedBonus: 1f,
                    requiredBuildings: new[] { BuildType.Motte }
                ),

                // L3 – STEEL
                new ToolInfo(
                    "Steel Pickaxe",
                    "Digs deep and fast. Harvests +15 stone per action.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 120),
                        new ResourceAmount(ResourceType.Stone, 180),
                        new ResourceAmount(ResourceType.Planks, 25),
                        new ResourceAmount(ResourceType.Iron, 25),
                        new ResourceAmount(ResourceType.Gold, 75)
                    },
                    harvestAmount: 15,
                    movementSpeedBonus: 1f,
                    requiredBuildings: new[] { BuildType.GrandHall }
                ),

                // L4 – MASTERWORK
                new ToolInfo(
                    "Masterwork Pickaxe",
                    "Master-crafted for heavy stone. Harvests +20 stone per action.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 180),
                        new ResourceAmount(ResourceType.Stone, 260),
                        new ResourceAmount(ResourceType.Planks, 60),
                        new ResourceAmount(ResourceType.Iron, 60),
                        new ResourceAmount(ResourceType.Gold, 250)
                    },
                    harvestAmount: 20,
                    movementSpeedBonus: 1f,
                    requiredBuildings: new[] { BuildType.Libary }
                )
            }
        },

        // ================= BOOTS (MOVE SPEED) =================
        {
            ToolType.Boots,
            new List<ToolInfo>
            {
                // L0
                new ToolInfo(
                    "Worn Shoes",
                    "Standard movement speed.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 10),
                        new ResourceAmount(ResourceType.Stone, 5)
                    },
                    harvestAmount: 0,
                    movementSpeedBonus: 1.00f,
                    requiredBuildings: new BuildType[] { }
                ),

                // L1
                new ToolInfo(
                    "Leather Boots",
                    "A little quicker. Movement speed x2.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 25),
                        new ResourceAmount(ResourceType.Stone, 15),
                        new ResourceAmount(ResourceType.Gold, 10)
                    },
                    harvestAmount: 0,
                    movementSpeedBonus: 2.00f,
                    requiredBuildings: new BuildType[] { }
                ),

                // L2
                new ToolInfo(
                    "Iron-Toed Boots",
                    "Sturdy and fast. Movement speed x3.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 50),
                        new ResourceAmount(ResourceType.Stone, 30),
                        new ResourceAmount(ResourceType.Iron, 10),
                        new ResourceAmount(ResourceType.Gold, 25)
                    },
                    harvestAmount: 0,
                    movementSpeedBonus: 3.00f,
                    requiredBuildings: new[] { BuildType.Motte }
                ),

                // L3
                new ToolInfo(
                    "Steel-Plated Boots",
                    "Confident stride. Movement speed x4.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 90),
                        new ResourceAmount(ResourceType.Stone, 60),
                        new ResourceAmount(ResourceType.Planks, 25),
                        new ResourceAmount(ResourceType.Iron, 25),
                        new ResourceAmount(ResourceType.Gold, 75)
                    },
                    harvestAmount: 0,
                    movementSpeedBonus: 4.00f,
                    requiredBuildings: new[] { BuildType.GrandHall }
                ),

                // L4
                new ToolInfo(
                    "Master Courier Boots",
                    "Master-made. Movement speed x5.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 140),
                        new ResourceAmount(ResourceType.Stone, 100),
                        new ResourceAmount(ResourceType.Planks, 60),
                        new ResourceAmount(ResourceType.Iron, 60),
                        new ResourceAmount(ResourceType.Gold, 250)
                    },
                    harvestAmount: 0,
                    movementSpeedBonus: 5.00f,
                    requiredBuildings: new[] { BuildType.Libary }
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
