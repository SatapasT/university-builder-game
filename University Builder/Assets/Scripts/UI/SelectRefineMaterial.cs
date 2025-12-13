using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class RefineMaterialsUI : MonoBehaviour
{
    public static RefineMaterialsUI Instance { get; private set; }

    [SerializeField] private GameObject refineInfoTextObject;

    private TextMeshProUGUI refineInfoText;

    public RefineType CurrentRefine { get; private set; } = RefineType.None;
    public bool HasSelection => CurrentRefine != RefineType.None;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (refineInfoTextObject != null)
        {
            refineInfoText = refineInfoTextObject.GetComponent<TextMeshProUGUI>();
            refineInfoTextObject.SetActive(false);
        }
    }

    public void SelectIron() => SelectRefineMaterial(RefineType.Iron);
    public void SelectPlanks() => SelectRefineMaterial(RefineType.Planks);

    private void SelectRefineMaterial(RefineType type)
    {
        CurrentRefine = type;

        ToolSelectUpgrade.Instance?.ClearSelection();
        SelectedBuildTracker.Instance?.ClearSelection();

        if (refineInfoText == null)
            return;

        RefineMaterialInfo info = RefineMaterialDatabase.Get(type);
        if (info == null)
        {
            ClearSelection();
            return;
        }

        refineInfoTextObject.SetActive(true);

        StringBuilder sb = new StringBuilder();

        // ---------- TITLE ----------
        sb.AppendLine($"<b><color=yellow>{info.Name}</color></b>");
        sb.AppendLine();

        // ---------- DESCRIPTION ----------
        if (!string.IsNullOrWhiteSpace(info.Description))
        {
            sb.AppendLine(info.Description);
            sb.AppendLine();
        }

        // ---------- OUTPUT ----------
        sb.AppendLine("<b><color=orange>Produces</color></b>");
        sb.AppendLine($"- +{info.Output.amount} {info.Output.type}");
        sb.AppendLine();

        // ---------- COSTS ----------
        sb.AppendLine("<b><color=orange>Requirements</color></b>");

        Dictionary<ResourceType, int> resources =
            ResourcesManager.Instance != null
                ? ResourcesManager.Instance.GetAllResources()
                : null;

        foreach (var cost in info.InputCosts)
        {
            int have = 0;
            resources?.TryGetValue(cost.type, out have);
            sb.AppendLine($"- {have}/{cost.amount} {cost.type}");
        }

        // ---------- REQUIRED BUILDINGS ----------
        if (info.RequiredBuildings.Length > 0)
        {
            sb.AppendLine();
            sb.AppendLine("<b><color=orange>Requires Buildings</color></b>");

            foreach (var req in info.RequiredBuildings)
            {
                bool has = PlayerStats.Instance != null && PlayerStats.Instance.HasBuilding(req);
                string color = has ? "green" : "red";
                sb.AppendLine($"- <color={color}>{req}</color>");
            }
        }

        refineInfoText.text = sb.ToString();

        WorkshopUI.Instance?.RenderConfirmButton();
    }

    // -------- CONFIRM BUTTON LOGIC --------
    public bool CanAffordSelectedRefine()
    {
        if (!HasSelection || ResourcesManager.Instance == null || PlayerStats.Instance == null)
            return false;

        RefineMaterialInfo info = RefineMaterialDatabase.Get(CurrentRefine);
        if (info == null)
            return false;

        foreach (var req in info.RequiredBuildings)
        {
            if (!PlayerStats.Instance.HasBuilding(req))
                return false;
        }

        var resources = ResourcesManager.Instance.GetAllResources();
        foreach (var cost in info.InputCosts)
        {
            if (!resources.TryGetValue(cost.type, out int have) || have < cost.amount)
                return false;
        }

        return true;
    }

    public bool TryApplyRefine()
    {
        if (!CanAffordSelectedRefine())
            return false;

        RefineMaterialInfo info = RefineMaterialDatabase.Get(CurrentRefine);
        if (info == null)
            return false;

        foreach (var cost in info.InputCosts)
            ResourcesManager.Instance.DeductResources(cost.type, cost.amount);

        ResourcesManager.Instance.AddResource(info.Output.type, info.Output.amount);

        SelectRefineMaterial(CurrentRefine);
        return true;
    }

    public void ClearSelection()
    {
        CurrentRefine = RefineType.None;

        if (refineInfoText != null)
            refineInfoText.text = "";

        if (refineInfoTextObject != null)
            refineInfoTextObject.SetActive(false);

        WorkshopUI.Instance?.RenderConfirmButton();
    }
}
