using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopUI : MonoBehaviour
{
    public static SelectedBuildTracker Instance { get; private set; }

    [SerializeField] private List<GameObject> AllWorkshopPanels = new();
    [SerializeField] private List<GameObject> MainWorkshopPanels = new();

    [SerializeField] private GameObject BuildMenu;
    [SerializeField] private GameObject ConfirmButton;

    [SerializeField] private GameObject CastleObject;

    [SerializeField] private GameObject BuilderPrefab;
    [SerializeField] private Transform BuilderSpawnPoint;


    public bool IsOpen { get; private set; }

    private void Awake()
    {
        foreach (var panel in AllWorkshopPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        if (CastleObject != null)
            CastleObject.SetActive(false);

        IsOpen = false;
    }


    public void ToggleMenu()
    {
        if (IsOpen) CloseMenu();
        else OpenMenu();
    }

    public void OpenMenu()
    {
        foreach (var panel in MainWorkshopPanels)
        {
            if (panel != null)
                panel.SetActive(true);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        IsOpen = true;
    }

    public void CloseMenu()
    {
        foreach (var panel in AllWorkshopPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        IsOpen = false;
    }

    private bool HasEnoughResources(BuildInfo buildInfo, Dictionary<ResourceType, int> playerResources)
    {
        if (buildInfo == null || playerResources == null)
            return false;
        foreach (var cost in buildInfo.Costs)
        {
            if (!playerResources.ContainsKey(cost.type) || playerResources[cost.type] < cost.amount)
            {
                return false;
            }
        }
        return true;
    }
    public void RenderConfirmButton()
    {
        if (ConfirmButton == null)
            return;

        Dictionary<ResourceType, int> playerResources = null;
        if (ResourcesManager.Instance != null)
        {
            playerResources = ResourcesManager.Instance.GetAllResources();
        }

        BuildInfo buildInfo = BuildDatabase.Get(SelectedBuildTracker.Instance.CurrentBuild);

        if (buildInfo == null || playerResources == null)
        {
            ConfirmButton.SetActive(false);
            return;
        }

        ConfirmButton.SetActive(true);

        Image buttonImage = ConfirmButton.GetComponent<Image>();
        if (buttonImage == null)
            return;

        bool hasEnoughResources = HasEnoughResources(buildInfo, playerResources);

        if (hasEnoughResources)
        {
            buttonImage.color = Color.green;
        }
        else
        {
            buttonImage.color = Color.gray;
        }
    }

    public void ClickConfirmButton()
    {
        // --- Validate managers ---
        if (ResourcesManager.Instance == null)
            return;

        BuildType selected = SelectedBuildTracker.Instance.CurrentBuild;
        BuildInfo buildInfo = BuildDatabase.Get(selected);
        Dictionary<ResourceType, int> playerResources = ResourcesManager.Instance.GetAllResources();

        // --- Validate build info & resources ---
        if (!HasEnoughResources(buildInfo, playerResources))
            return;

        // --- Deduct resources ---
        foreach (var cost in buildInfo.Costs)
            ResourcesManager.Instance.DeductResources(cost.type, cost.amount);

        // --- Ensure this build type is Castle ---
        if (selected != BuildType.Castle)
            return;

        if (CastleObject == null)
            return;

        // --- Start construction ---
        CastleObject.SetActive(true);

        BuildingConstruction buildingConstruction =
            CastleObject.GetComponent<BuildingConstruction>();

        if (buildingConstruction == null)
        {
            Debug.LogError("BuildingConstruction component not found on CastleObject");
            return;
        }

        buildingConstruction.StartConstruction();

        if (BuilderPrefab == null || BuilderSpawnPoint == null)
        {
            Debug.LogError("BuilderPrefab or BuilderSpawnPoint not assigned on WorkshopUI");
            return;
        }

        GameObject builderInstance = Instantiate(
            BuilderPrefab,
            BuilderSpawnPoint.position,
            BuilderSpawnPoint.rotation);

        BuilderAgent builderAgent = builderInstance.GetComponent<BuilderAgent>();

        if (builderAgent == null)
        {
            Debug.LogError("BuilderAgent component missing on BuilderPrefab");
            return;
        }

        builderAgent.Initialize(CastleObject.transform, buildingConstruction);
    }



    public void HideConfirmButton()
    {
        ConfirmButton.SetActive(false);
    }

    public void OpenBuildMenu()
    {
        BuildMenu.SetActive(true);
    }

    public void CloseBuildMenu()
    {
        BuildMenu.SetActive(false);
    }
}
