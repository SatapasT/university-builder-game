using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ToolSelectUpgrade : MonoBehaviour
{
    public static ToolSelectUpgrade Instance { get; private set; }

    [SerializeField] private GameObject toolInfoTextObject;

    public ToolType CurrentTool { get; private set; } = ToolType.None;

    private TextMeshProUGUI toolInfoText;

    public bool HasSelection => CurrentTool != ToolType.None;

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

    // ---------------- SELECTION + INFO DISPLAY ----------------

    private void SelectTool(ToolType toolType)
    {
        CurrentTool = toolType;

        if (toolInfoText == null || PlayerStats.Instance == null)
            return;

        int level = PlayerStats.Instance.GetToolLevel(toolType);

        ToolInfo current = ToolsDatabase.Get(toolType, level);
        ToolInfo next = PlayerStats.Instance.GetNextUpgrade(toolType);

        if (current == null)
            return;

        toolInfoTextObject.SetActive(true);
        StringBuilder builder = new StringBuilder();

        // ---------- TITLE ----------
        if (next != null)
            builder.AppendLine($"<b><color=yellow>{next.Name}</color></b>");
        else
            builder.AppendLine($"<b><color=yellow>{current.Name} (Max Level)</color></b>");

        builder.AppendLine();

        // ---------- DESCRIPTION ----------
        ToolInfo shown = next ?? current;
        if (!string.IsNullOrWhiteSpace(shown.Description))
        {
            builder.AppendLine(shown.Description);
            builder.AppendLine();
        }

        // ---------- STATS ----------
        builder.AppendLine("<b><color=orange>Stats</color></b>");

        if (toolType == ToolType.Axe || toolType == ToolType.Pickaxe)
        {
            if (next != null && next.HarvestAmount != current.HarvestAmount)
                builder.AppendLine($"- Harvest: +{current.HarvestAmount} → +{next.HarvestAmount}");
            else
                builder.AppendLine($"- Harvest: +{current.HarvestAmount}");
        }

        if (toolType == ToolType.Boots)
        {
            if (next != null && Mathf.Abs(next.MovementSpeedBonus - current.MovementSpeedBonus) > 0.01f)
                builder.AppendLine($"- Move Speed: x{current.MovementSpeedBonus} → x{next.MovementSpeedBonus}");
            else
                builder.AppendLine($"- Move Speed: x{current.MovementSpeedBonus}");
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
            var resources = ResourcesManager.Instance.GetAllResources();
            foreach (var cost in next.Costs)
            {
                resources.TryGetValue(cost.type, out int have);
                builder.AppendLine($"- {have}/{cost.amount} {cost.type}");
            }
        }

        // ---------- REQUIRED BUILDINGS ----------
        if (next != null && next.RequiredBuildings.Length > 0)
        {
            builder.AppendLine();
            builder.AppendLine("<b><color=orange>Requires</color></b>");

            foreach (BuildType req in next.RequiredBuildings)
            {
                bool has = PlayerStats.Instance.HasBuilding(req);
                string color = has ? "green" : "red";
                builder.AppendLine($"- <color={color}>{req}</color>");
            }
        }

        toolInfoText.text = builder.ToString();
        WorkshopUI.Instance?.RenderConfirmButton();
    }


    public bool CanAffordSelectedUpgrade()
    {
        if (!HasSelection || PlayerStats.Instance == null || ResourcesManager.Instance == null)
            return false;

        ToolInfo next = PlayerStats.Instance.GetNextUpgrade(CurrentTool);
        if (next == null) return false;

        var resources = ResourcesManager.Instance.GetAllResources();
        foreach (var cost in next.Costs)
        {
            if (!resources.TryGetValue(cost.type, out int have) || have < cost.amount)
                return false;
        }

        return true;
    }

    public bool TryApplyUpgrade()
    {
        if (!CanAffordSelectedUpgrade())
            return false;

        ToolInfo next = PlayerStats.Instance.GetNextUpgrade(CurrentTool);
        foreach (var cost in next.Costs)
            ResourcesManager.Instance.DeductResources(cost.type, cost.amount);

        PlayerStats.Instance.UpgradeToolLevel(CurrentTool);
        SelectTool(CurrentTool);
        return true;
    }

    public void ClearSelection()
    {
        CurrentTool = ToolType.None;
        toolInfoText.text = "";
        toolInfoTextObject.SetActive(false);
    }
}
