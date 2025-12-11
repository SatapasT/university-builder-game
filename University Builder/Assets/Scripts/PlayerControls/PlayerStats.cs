using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("Tool Levels (0 = basic)")]
    [SerializeField] private int axeLevel = 0;
    [SerializeField] private int pickaxeLevel = 0;
    [SerializeField] private int bootsLevel = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public int GetToolLevel(ToolType type)
    {
        return type switch
        {
            ToolType.Axe => axeLevel,
            ToolType.Pickaxe => pickaxeLevel,
            ToolType.Boots => bootsLevel,
            _ => 0
        };
    }

    private void SetToolLevel(ToolType type, int level)
    {
        switch (type)
        {
            case ToolType.Axe: axeLevel = level; break;
            case ToolType.Pickaxe: pickaxeLevel = level; break;
            case ToolType.Boots: bootsLevel = level; break;
        }
    }

    public ToolInfo GetCurrentToolInfo(ToolType type)
    {
        int level = GetToolLevel(type);
        return ToolsDatabase.Get(type, level);
    }

    public ToolInfo GetNextUpgrade(ToolType type)
    {
        int nextLevel = GetToolLevel(type) + 1;
        return ToolsDatabase.Get(type, nextLevel);
    }

    public int GetMaxLevel(ToolType type)
    {
        return ToolsDatabase.GetMaxLevel(type);
    }

    public bool UpgradeToolLevel(ToolType type)
    {
        int current = GetToolLevel(type);
        int max = GetMaxLevel(type);

        if (current >= max)
            return false;

        SetToolLevel(type, current + 1);
        return true;
    }

    public int GetWoodHarvestAmount()
    {
        ToolInfo axe = GetCurrentToolInfo(ToolType.Axe);
        return axe != null && axe.HarvestAmount > 0 ? axe.HarvestAmount : 1;
    }

    public int GetStoneHarvestAmount()
    {
        ToolInfo pick = GetCurrentToolInfo(ToolType.Pickaxe);
        return pick != null && pick.HarvestAmount > 0 ? pick.HarvestAmount : 1;
    }

    public float GetMoveSpeedMultiplier()
    {
        ToolInfo boots = GetCurrentToolInfo(ToolType.Boots);
        return boots != null && boots.MovementSpeedBonus > 0f ? boots.MovementSpeedBonus : 1f;
    }
}
