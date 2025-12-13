using System.Collections.Generic;
using UnityEngine;

public struct ResourceAmount
{
    public ResourceType type;
    public int amount;

    public ResourceAmount(ResourceType type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }
}

public class BuildInfo
{
    public string Nickname { get; }
    public string Info { get; }
    public string ProvidesInfo { get; }

    public ResourceAmount[] Costs { get; }

    public BuildType[] RequiredBuildings { get; }
    public ResourceAmount[] PassiveIncome { get; }
    public ResourceType[] UnlocksProcessing { get; }

    public float BuildTimeSeconds { get; }
    public int UnlocksBuilderSlots { get; }

    public BuildInfo(
        string nickname,
        string info,
        string providesInfo,
        ResourceAmount[] costs,
        float buildTimeSeconds,
        BuildType[] requiredBuildings = null,
        ResourceAmount[] passiveIncome = null,
        ResourceType[] unlocksProcessing = null)
    {
        Nickname = nickname;
        Info = info;
        ProvidesInfo = providesInfo;

        Costs = costs;
        BuildTimeSeconds = buildTimeSeconds;

        RequiredBuildings = requiredBuildings ?? new BuildType[0];
        PassiveIncome = passiveIncome ?? new ResourceAmount[0];
        UnlocksProcessing = unlocksProcessing ?? new ResourceType[0];
    }
}




public static class BuildDatabase
{
    private static readonly Dictionary<BuildType, BuildInfo> data =
        new Dictionary<BuildType, BuildInfo>
        {
            // ================= CASTLE =================
            {
                BuildType.Castle,
                new BuildInfo(
                    "Durham Castle",
                    "The fortified seat of power overlooking the River Wear.",
                    "Generates Gold over time.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 50),
                        new ResourceAmount(ResourceType.Stone, 50)
                    },
                    buildTimeSeconds: 20f,
                    passiveIncome: new[]
                    {
                        new ResourceAmount(ResourceType.Gold, 2)
                    }
                )
            },

            // ================= BRIDGE =================
            {
                BuildType.Bridge,
                new BuildInfo(
                    "Elvet Bridge",
                    "A stone bridge enabling trade and travel.",
                    "Generates Gold over time.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 50),
                        new ResourceAmount(ResourceType.Stone, 300)
                    },
                    buildTimeSeconds: 15f,
                    passiveIncome: new[]
                    {
                        new ResourceAmount(ResourceType.Gold, 10)
                    }
                )
            },

            // ================= MOTTE =================
            {
                BuildType.Motte,
                new BuildInfo(
                    "Castle Motte",
                    "The original defensive mound of the fortress.",
                    "Unlocks iron processing.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 200),
                        new ResourceAmount(ResourceType.Stone, 300),
                        new ResourceAmount(ResourceType.Gold, 50)
                    },
                    buildTimeSeconds: 25f,
                    passiveIncome: new[]
                    {
                        new ResourceAmount(ResourceType.Gold, 5)
                    },
                    requiredBuildings: new[]
                    {
                        BuildType.Castle
                    },
                    unlocksProcessing: new[]
                    {
                        ResourceType.Iron
                    }
                )
            },

            // ================= COURTYARD =================
            {
                BuildType.Courtyard,
                new BuildInfo(
                    "Inner Courtyard",
                    "A central space for daily life and training.",
                    "Unlocks plank processing.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 500),
                        new ResourceAmount(ResourceType.Stone, 200),
                        new ResourceAmount(ResourceType.Gold, 100),
                        new ResourceAmount(ResourceType.Iron, 30)
                    },
                    buildTimeSeconds: 30f,
                    passiveIncome: new[]
                    {
                        new ResourceAmount(ResourceType.Gold, 30)
                    },
                    requiredBuildings: new[]
                    {
                        BuildType.Motte
                    },
                    unlocksProcessing: new[]
                    {
                        ResourceType.Planks
                    }
                )
            },

            // ================= GRAND HALL =================
            {
                BuildType.GrandHall,
                new BuildInfo(
                    "Grand Hall",
                    "The administrative and ceremonial heart of the castle.",
                    "Unlocks advanced equipment upgrades.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Stone, 400),
                        new ResourceAmount(ResourceType.Gold, 1000),
                        new ResourceAmount(ResourceType.Iron, 50),
                        new ResourceAmount(ResourceType.Planks, 100)
                    },
                    buildTimeSeconds: 40f,
                    passiveIncome: new[]
                    {
                        new ResourceAmount(ResourceType.Gold, 100)
                    },
                    requiredBuildings: new[]
                    {
                        BuildType.Courtyard
                    }
                )
            },

            // ================= LIBRARY =================
            {
                BuildType.Libary,
                new BuildInfo(
                    "Monastic Library",
                    "A place of preserved knowledge and mastery.",
                    "Unlocks masterwork tools.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Stone, 600),
                        new ResourceAmount(ResourceType.Gold, 3000),
                        new ResourceAmount(ResourceType.Iron, 100),
                        new ResourceAmount(ResourceType.Planks, 100)
                    },
                    buildTimeSeconds: 45f,
                    requiredBuildings: new[]
                    {
                        BuildType.GrandHall
                    }
                )
            }
        };

    public static BuildInfo Get(BuildType type)
    {
        if (data.TryGetValue(type, out var info))
            return info;

        Debug.LogWarning($"No BuildInfo defined for type {type}");
        return null;
    }
}