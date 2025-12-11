using UnityEngine;

using System.Text;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolSelectUpgrade : MonoBehaviour
{
    public static ToolSelectUpgrade Instance { get; private set; }

    [SerializeField] private GameObject toolInfoTextObject;

    public ToolType CurrentTool { get; private set; } = ToolType.None;

    private TextMeshProUGUI toolInfoText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (toolInfoTextObject != null)
        {
            toolInfoText = toolInfoTextObject.GetComponent<TextMeshProUGUI>();
            toolInfoTextObject.SetActive(false);
        }
    }

    public void SelectAxe() => SelectTool(ToolType.Axe);
    public void SelectPickaxe() => SelectTool(ToolType.Pickaxe);
    public void SelectBoots() => SelectTool(ToolType.Boots);

    private void SelectTool(ToolType toolType)
    {
        CurrentTool = toolType;

        if (toolInfoText == null)
            return;

        if (ToolManager.Instance == null)
        {
            Debug.LogError("ToolManager.Instance is null – add a ToolManager GameObject to the scene.");
            return;
        }

        int level = ToolManager.Instance.GetCurrentLevel(toolType);

        ToolInfo current = ToolsDatabase.Get(toolType, level);
        ToolInfo next = ToolManager.Instance.GetNextUpgrade(toolType); // may be null at max level

        if (current == null)
            return;

        toolInfoTextObject.SetActive(true);
        StringBuilder builder = new StringBuilder();

        // ---------- TITLE ----------
        if (next != null)
            builder.AppendLine($"<b><color=yellow>{next.Name}</color></b>");   // show NEXT upgrade name
        else
            builder.AppendLine($"<b><color=yellow>{current.Name} (Max Level)</color></b>");

        builder.AppendLine();

        // ---------- DESCRIPTION ----------
        if (next != null && !string.IsNullOrWhiteSpace(next.Description))
        {
            builder.AppendLine(next.Description);
            builder.AppendLine();
        }
        else if (!string.IsNullOrWhiteSpace(current.Description))
        {
            builder.AppendLine(current.Description);
            builder.AppendLine();
        }

        // ---------- STATS ----------
        builder.AppendLine("<b><color=orange>Stats</color></b>");

        int currentHarvest = current.HarvestAmount;
        int nextHarvest = next != null ? next.HarvestAmount : currentHarvest;

        float currentSpeed = current.MovementSpeedBonus <= 0f ? 1f : current.MovementSpeedBonus;
        float nextSpeed = next != null && next.MovementSpeedBonus > 0f
                                ? next.MovementSpeedBonus
                                : currentSpeed;

        if (currentHarvest > 0 || nextHarvest > 0)
        {
            if (next != null && nextHarvest != currentHarvest)
                builder.AppendLine($"- Harvest: +{currentHarvest}  >  +{nextHarvest}");
            else
                builder.AppendLine($"- Harvest: +{currentHarvest}");
        }

        if (currentSpeed > 0f || nextSpeed > 0f)
        {
            if (next != null && Mathf.Abs(nextSpeed - currentSpeed) > 0.001f)
                builder.AppendLine($"- Move Speed: x{currentSpeed:0.00}  >  x{nextSpeed:0.00}");
            else
                builder.AppendLine($"- Move Speed: x{currentSpeed:0.00}");
        }

        builder.AppendLine();

        // ---------- UPGRADE COST ----------
        builder.AppendLine("<b><color=orange>Upgrade Cost</color></b>");

        if (next == null)
        {
            builder.AppendLine("Max level reached.");
        }
        else
        {
            Dictionary<ResourceType, int> playerResources =
                ResourcesManager.Instance != null
                    ? ResourcesManager.Instance.GetAllResources()
                    : null;

            foreach (ResourceAmount cost in next.UpgradeCosts)
            {
                int currentAmount = 0;
                playerResources?.TryGetValue(cost.type, out currentAmount);
                builder.AppendLine($"- {currentAmount}/{cost.amount} {cost.type}");
            }
        }

        toolInfoText.text = builder.ToString();
    }



    public void ClearSelection()
    {
        CurrentTool = ToolType.None;

        if (toolInfoText != null)
        {
            toolInfoText.text = string.Empty;
            toolInfoTextObject.SetActive(false);
        }
    }
}
