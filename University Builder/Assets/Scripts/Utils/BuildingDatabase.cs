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
            {
                BuildType.Castle,
                new BuildInfo(
                    "Durham Castle",
                    // Info / description text
                    "A fortified stronghold overlooking the river. " +
                    "One of Durham's landmarks.",
                    new[]
                    {
                        new ResourceAmount(ResourceType.Wood, 5),
                        new ResourceAmount(ResourceType.Stone, 5)
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