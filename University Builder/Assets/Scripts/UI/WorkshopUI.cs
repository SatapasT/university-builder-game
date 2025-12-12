using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopUI : MonoBehaviour
{
    public static WorkshopUI Instance { get; private set; }

    [SerializeField] private List<GameObject> AllWorkshopPanels = new();
    [SerializeField] private List<GameObject> MainWorkshopPanels = new();

    [SerializeField] private GameObject BuildMenu;
    [SerializeField] private GameObject AssignWorkerMenu;
    [SerializeField] private GameObject UpgradeToolMenu;
    [SerializeField] private GameObject RefineMaterialMenu;
    [SerializeField] private GameObject ConfirmButton;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (GameObject panel in AllWorkshopPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        IsOpen = false;
    }

    public void ToggleMenu()
    {
        if (IsOpen) CloseMenu();
        else OpenMenu();
    }

    public void OpenMenu()
    {
        UIManager.Instance.SetMenuState(true);
        foreach (GameObject panel in MainWorkshopPanels)
        {
            if (panel != null)
                panel.SetActive(true);
        }

        IsOpen = true;
        RenderConfirmButton();
    }

    public void CloseMenu()
    {
        UIManager.Instance.SetMenuState(false);
        foreach (GameObject panel in AllWorkshopPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        IsOpen = false;
    }

    // --------- CONFIRM BUTTON ---------

    public void RenderConfirmButton()
    {
        if (ConfirmButton == null)
            return;

        bool toolMode =
            ToolSelectUpgrade.Instance != null &&
            ToolSelectUpgrade.Instance.HasSelection;

        bool buildMode =
            !toolMode &&
            SelectedBuildTracker.Instance != null &&
            SelectedBuildTracker.Instance.HasSelection;

        bool refineMode =
            !toolMode && !buildMode &&
            RefineMaterialsUI.Instance != null &&
            RefineMaterialsUI.Instance.HasSelection;

        // NEW: Assign Worker mode (only if menu is open + AssignWorkerUI has valid selection)
        bool assignMode =
            !toolMode && !buildMode && !refineMode &&
            AssignWorkerMenu != null && AssignWorkerMenu.activeInHierarchy &&
            AssignWorkerUI.Instance != null &&
            AssignWorkerUI.Instance.HasValidSelection;

        if (!toolMode && !buildMode && !refineMode && !assignMode)
        {
            ConfirmButton.SetActive(false);
            return;
        }

        ConfirmButton.SetActive(true);

        Image buttonImage = ConfirmButton.GetComponent<Image>();
        if (buttonImage == null)
            return;

        bool canConfirm = false;

        if (toolMode)
            canConfirm = ToolSelectUpgrade.Instance.CanAffordSelectedUpgrade();
        else if (buildMode)
            canConfirm = SelectedBuildTracker.Instance.CanAffordCurrentBuild();
        else if (refineMode)
            canConfirm = RefineMaterialsUI.Instance.CanAffordSelectedRefine();
        else if (assignMode)
            canConfirm = AssignWorkerUI.Instance.CanConfirmAssign();

        buttonImage.color = canConfirm ? Color.green : Color.gray;
    }

    public void ClickConfirmButton()
    {
        if (ToolSelectUpgrade.Instance != null &&
            ToolSelectUpgrade.Instance.HasSelection)
        {
            ToolSelectUpgrade.Instance.TryApplyUpgrade();
            RenderConfirmButton();
            return;
        }

        if (SelectedBuildTracker.Instance != null &&
            SelectedBuildTracker.Instance.HasSelection)
        {
            SelectedBuildTracker.Instance.TryStartConstruction();
            RenderConfirmButton();
            return;
        }

        if (RefineMaterialsUI.Instance != null &&
            RefineMaterialsUI.Instance.HasSelection)
        {
            RefineMaterialsUI.Instance.TryApplyRefine();
            RenderConfirmButton();
            return;
        }

        // NEW: Assign Worker confirm
        if (AssignWorkerMenu != null && AssignWorkerMenu.activeInHierarchy &&
            AssignWorkerUI.Instance != null)
        {
            AssignWorkerUI.Instance.ConfirmAssign();
            RenderConfirmButton();
            return;
        }
    }

    public void HideConfirmButton()
    {
        if (ConfirmButton != null)
            ConfirmButton.SetActive(false);
    }

    // --------- MENUS ---------

    public void OpenBuildMenu()
    {
        if (BuildMenu != null)
            BuildMenu.SetActive(true);

        if (ToolSelectUpgrade.Instance != null)
            ToolSelectUpgrade.Instance.ClearSelection();

        RenderConfirmButton();
    }

    public void CloseBuildMenu()
    {
        if (BuildMenu != null)
            BuildMenu.SetActive(false);

        RenderConfirmButton();
    }

    public void OpenUpgradeToolMenu()
    {
        if (UpgradeToolMenu != null)
            UpgradeToolMenu.SetActive(true);

        if (SelectedBuildTracker.Instance != null)
            SelectedBuildTracker.Instance.ClearSelection();

        RenderConfirmButton();
    }

    public void CloseUpgradeToolMenu()
    {
        if (UpgradeToolMenu != null)
            UpgradeToolMenu.SetActive(false);

        if (ToolSelectUpgrade.Instance != null)
            ToolSelectUpgrade.Instance.ClearSelection();

        RenderConfirmButton();
    }

    public void OpenRefineMaterialMenu()
    {
        if (RefineMaterialMenu != null)
            RefineMaterialMenu.SetActive(true);

        RenderConfirmButton();
    }

    public void CloseRefineMaterialMenu()
    {
        if (RefineMaterialMenu != null)
            RefineMaterialMenu.SetActive(false);

        RenderConfirmButton();
    }

    public void OpenAssignWorkerMenu()
    {
        if (AssignWorkerMenu != null)
            AssignWorkerMenu.SetActive(true);

        // refresh + show confirm if valid
        AssignWorkerUI.Instance?.RefreshAll();
        RenderConfirmButton();
    }

    public void CloseAssignWorkerMenu()
    {
        if (AssignWorkerMenu != null)
            AssignWorkerMenu.SetActive(false);

        RenderConfirmButton();
    }
}
