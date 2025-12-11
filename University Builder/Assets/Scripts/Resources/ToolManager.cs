using UnityEngine;

public class ToolManager : MonoBehaviour
{
    public static ToolManager Instance { get; private set; }

    private int axeLevel = 0;
    private int pickaxeLevel = 0;
    private int bootsLevel = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetCurrentLevel(ToolType type)
    {
        return type switch
        {
            ToolType.Axe => axeLevel,
            ToolType.Pickaxe => pickaxeLevel,
            ToolType.Boots => bootsLevel,
            _ => 0
        };
    }

    public ToolInfo GetCurrentTool(ToolType type)
    {
        return ToolsDatabase.Get(type, GetCurrentLevel(type));
    }

    public ToolInfo GetNextUpgrade(ToolType type)
    {
        int next = GetCurrentLevel(type) + 1;
        return ToolsDatabase.Get(type, next);
    }

    public bool UpgradeTool(ToolType type)
    {
        int current = GetCurrentLevel(type);
        int max = ToolsDatabase.GetMaxLevel(type);

        if (current >= max)
            return false;

        int newLevel = current + 1;

        switch (type)
        {
            case ToolType.Axe: axeLevel = newLevel; break;
            case ToolType.Pickaxe: pickaxeLevel = newLevel; break;
            case ToolType.Boots: bootsLevel = newLevel; break;
        }

        return true;
    }
}
