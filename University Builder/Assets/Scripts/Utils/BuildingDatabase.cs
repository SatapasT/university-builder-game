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
    public ResourceAmount[] Costs { get; }

    public BuildInfo(string nickname, string info, ResourceAmount[] costs)
    {
        Nickname = nickname;
        Info = info;
        Costs = costs;
    }
}

public static class BuildDatabase
{
    private static readonly Dictionary<BuildType, BuildInfo> data =
        new Dictionary<BuildType, BuildInfo>
        {
            // ---------------- CASTLE ----------------
            {
                BuildType.Castle,
                new BuildInfo(
                    "Durham Castle",
                    "A fortified Norman stronghold overlooking the River Wear. " +
                    "Seat of power and military control within the region.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 5),
                        new ResourceAmount(ResourceType.Stone, 5)
                    }
                )
            },

            // ---------------- BRIDGE ----------------
            {
                BuildType.Bridge,
                new BuildInfo(
                    "Elvet Bridge",
                    "A sturdy stone bridge spanning the River Wear, enabling trade, travel, " +
                    "and reliable passage into the city.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 4),
                        new ResourceAmount(ResourceType.Stone, 6)
                    }
                )
            },

            // ---------------- COURTYARD ----------------
            {
                BuildType.Courtyard,
                new BuildInfo(
                    "Inner Courtyard",
                    "The central open space of the castle complex. " +
                    "Used for training, gatherings, and daily life within the walls.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 3),
                        new ResourceAmount(ResourceType.Stone, 4)
                    }
                )
            },

            // ---------------- MOTTE ----------------
            {
                BuildType.Motte,
                new BuildInfo(
                    "Castle Motte",
                    "A raised earthen mound forming the oldest part of the fortress. " +
                    "Provides height, visibility, and a strong defensive advantage.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 2),
                        new ResourceAmount(ResourceType.Stone, 3)
                    }
                )
            },

            // ---------------- GRAND HALL ----------------
            {
                BuildType.GrandHall,
                new BuildInfo(
                    "Grand Hall",
                    "The heart of castle life. A place for feasts, councils, and ceremonies, " +
                    "where nobles and officials gather under one roof.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 4),
                        new ResourceAmount(ResourceType.Stone, 4)
                    }
                )
            },

            // ---------------- LIBRARY ----------------
            {
                BuildType.Libary,
                new BuildInfo(
                    "Monastic Library",
                    "A quiet hall of manuscripts and knowledge, maintained by scribes and clerics. " +
                    "Preserves learning, records, and history.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 3),
                        new ResourceAmount(ResourceType.Stone, 2)
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