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
    public void RenderConfirmButton()
    {
        if (ConfirmButton == null)
            return;

        if (SelectedBuildTracker.Instance == null ||
            !SelectedBuildTracker.Instance.HasSelection ||
            ResourcesManager.Instance == null)
        {
            ConfirmButton.SetActive(false);
            return;
        }

        ConfirmButton.SetActive(true);

        Image buttonImage = ConfirmButton.GetComponent<Image>();
        if (buttonImage == null)
            return;

        bool canAfford = SelectedBuildTracker.Instance.CanAffordCurrentBuild();
        buttonImage.color = canAfford ? Color.green : Color.gray;
    }

    public void ClickConfirmButton()
    {
        if (SelectedBuildTracker.Instance == null)
            return;

        SelectedBuildTracker.Instance.TryStartConstruction();
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

    public void OpenUpgradeToolMenu()
    {
        UpgradeToolMenu.SetActive(true);
    }

    public void CloseUpgradeToolMenu()
    {
        UpgradeToolMenu.SetActive(false);
    }
}
