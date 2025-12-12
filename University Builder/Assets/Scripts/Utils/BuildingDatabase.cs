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
                    "It serves as both a defensive bastion and a symbol of authority.",
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
                    "and easier access between settlements.",
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
                    "Castle Courtyard",
                    "An open gathering space within the castle grounds. " +
                    "Used for training, markets, and daily life within the walls.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 3),
                        new ResourceAmount(ResourceType.Stone, 4)
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