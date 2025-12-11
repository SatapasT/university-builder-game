using UnityEngine;

public class ToolInfo
{
    public string Name { get; }
    public string Description { get; }
    public ResourceAmount[] UpgradeCosts { get; }
    public int HarvestAmount { get; }       
    public float MovementSpeedBonus { get; }

    public ToolInfo(
        string name,
        string description,
        ResourceAmount[] upgradeCosts,
        int harvestAmount = 0,
        float movementSpeedBonus = 0f)
    {
        Name = name;
        Description = description;
        UpgradeCosts = upgradeCosts;
        HarvestAmount = harvestAmount;
        MovementSpeedBonus = movementSpeedBonus;
    }
}
