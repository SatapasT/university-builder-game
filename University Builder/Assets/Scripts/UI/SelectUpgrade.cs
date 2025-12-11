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

    // --------- SELECTION + INFO DISPLAY ---------

    private void SelectTool(ToolType toolType)
    {
        CurrentTool = toolType;

        if (toolInfoText == null)
            return;

        if (PlayerStats.Instance == null)
        {
            Debug.LogError("PlayerStats.Instance is null – attach PlayerStats to the Player.");
            return;
        }

        int level = PlayerStats.Instance.GetToolLevel(toolType);

        ToolInfo current = ToolsDatabase.Get(toolType, level);
        ToolInfo next = PlayerStats.Instance.GetNextUpgrade(toolType); // may be null

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

        // ---------- STATS (current -> next) ----------
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

        // Refresh confirm button immediately
        if (WorkshopUI.Instance != null)
        {
            WorkshopUI.Instance.RenderConfirmButton();
        }
    }

    // --------- CONFIRM BUTTON HELPERS ---------

    public bool CanAffordSelectedUpgrade()
    {
        if (!HasSelection ||
            PlayerStats.Instance == null ||
            ResourcesManager.Instance == null)
            return false;

        ToolInfo next = PlayerStats.Instance.GetNextUpgrade(CurrentTool);
        if (next == null)
            return false; // max level

        Dictionary<ResourceType, int> playerResources =
            ResourcesManager.Instance.GetAllResources();

        foreach (ResourceAmount cost in next.UpgradeCosts)
        {
            if (!playerResources.TryGetValue(cost.type, out int have) ||
                have < cost.amount)
            {
                return false;
            }
        }

        return true;
    }

    public bool TryApplyUpgrade()
    {
        if (!HasSelection ||
            PlayerStats.Instance == null ||
            ResourcesManager.Instance == null)
            return false;

        ToolInfo next = PlayerStats.Instance.GetNextUpgrade(CurrentTool);
        if (next == null)
            return false; // max level

        Dictionary<ResourceType, int> playerResources =
            ResourcesManager.Instance.GetAllResources();

        // Check again (defensive)
        foreach (ResourceAmount cost in next.UpgradeCosts)
        {
            if (!playerResources.TryGetValue(cost.type, out int have) ||
                have < cost.amount)
            {
                return false;
            }
        }

        // Deduct cost
        foreach (ResourceAmount cost in next.UpgradeCosts)
        {
            ResourcesManager.Instance.DeductResources(cost.type, cost.amount);
        }

        // Bump level on the player
        PlayerStats.Instance.UpgradeToolLevel(CurrentTool);

        // Refresh info + confirm button
        SelectTool(CurrentTool);
        return true;
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
