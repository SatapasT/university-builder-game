using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> AllWorkshopPanels = new();
    [SerializeField] private List<GameObject> MainWorkshopPanels = new();

    [SerializeField] private GameObject BuildMenu;
    [SerializeField] private GameObject UpgradeToolMenu;
    [SerializeField] private GameObject ConfirmButton;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
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
        foreach (GameObject panel in MainWorkshopPanels)
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
        foreach (GameObject panel in AllWorkshopPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

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

        if (!toolMode && !buildMode)
        {
            ConfirmButton.SetActive(false);
            return;
        }

        ConfirmButton.SetActive(true);

        Image buttonImage = ConfirmButton.GetComponent<Image>();
        if (buttonImage == null)
            return;

        bool canAfford = false;

        if (toolMode)
        {
            canAfford = ToolSelectUpgrade.Instance.CanAffordSelectedUpgrade();
        }
        else if (buildMode)
        {
            canAfford = SelectedBuildTracker.Instance.CanAffordCurrentBuild();
        }

        buttonImage.color = canAfford ? Color.green : Color.gray;
    }

    public void ClickConfirmButton()
    {
        if (ToolSelectUpgrade.Instance != null &&
            ToolSelectUpgrade.Instance.HasSelection)
        {
            bool upgraded = ToolSelectUpgrade.Instance.TryApplyUpgrade();
            RenderConfirmButton();
            return;
        }

        if (SelectedBuildTracker.Instance != null &&
            SelectedBuildTracker.Instance.HasSelection)
        {
            SelectedBuildTracker.Instance.TryStartConstruction();
            RenderConfirmButton();
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
}
