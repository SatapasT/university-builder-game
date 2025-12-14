using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopUI : MonoBehaviour
{
    public static WorkshopUI Instance { get; private set; }

    [SerializeField] private List<GameObject> AllWorkshopPanels = new();
    [SerializeField] private List<GameObject> MainWorkshopPanels = new();

    [Header("Sub-menus")]
    [SerializeField] private GameObject BuildMenu;
    [SerializeField] private GameObject AssignWorkerMenu;
    [SerializeField] private GameObject UpgradeToolMenu;
    [SerializeField] private GameObject RefineMaterialMenu;

    [Header("Confirm")]
    [SerializeField] private GameObject ConfirmButton;

    public AudioClip writingSoundEffect;
    private AudioSource audioSource;

    public bool IsOpen { get; private set; }

    public bool IsAssignWorkerMenuOpen => AssignWorkerMenu != null && AssignWorkerMenu.activeInHierarchy;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (GameObject panel in AllWorkshopPanels)
            if (panel != null) panel.SetActive(false);

        IsOpen = false;
        audioSource = gameObject.AddComponent<AudioSource>();
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
            if (panel != null) panel.SetActive(true);

        IsOpen = true;
        RenderConfirmButton();
    }

    public void CloseMenu()
    {
        audioSource.Stop();
        AssignWorkerUI.Instance?.ClearSelectionUI();

        UIManager.Instance.SetMenuState(false);

        foreach (GameObject panel in AllWorkshopPanels)
            if (panel != null) panel.SetActive(false);

        IsOpen = false;
        HideConfirmButton();
    }

    // ---------------- CONFIRM ----------------

    public void RenderConfirmButton()
    {
        if (ConfirmButton == null) return;

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

        bool assignMode =
            !toolMode && !buildMode && !refineMode &&
            AssignWorkerMenu != null &&
            AssignWorkerMenu.activeInHierarchy &&
            AssignWorkerUI.Instance != null;

        if (!toolMode && !buildMode && !refineMode && !assignMode)
        {
            ConfirmButton.SetActive(false);
            return;
        }

        ConfirmButton.SetActive(true);

        Image buttonImage = ConfirmButton.GetComponent<Image>();
        if (buttonImage == null) return;

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
        audioSource.Stop();
        audioSource.PlayOneShot(writingSoundEffect);
        if (ToolSelectUpgrade.Instance != null && ToolSelectUpgrade.Instance.HasSelection)
        {
            ToolSelectUpgrade.Instance.TryApplyUpgrade();
            RenderConfirmButton();
            return;
        }

        if (SelectedBuildTracker.Instance != null && SelectedBuildTracker.Instance.HasSelection)
        {
            SelectedBuildTracker.Instance.TryStartConstruction();
            RenderConfirmButton();
            return;
        }

        if (RefineMaterialsUI.Instance != null && RefineMaterialsUI.Instance.HasSelection)
        {
            RefineMaterialsUI.Instance.TryApplyRefine();
            RenderConfirmButton();
            return;
        }

        if (AssignWorkerMenu != null && AssignWorkerMenu.activeInHierarchy &&
            AssignWorkerUI.Instance != null)
        {
            AssignWorkerUI.Instance.ConfirmAssign();
            RenderConfirmButton();
        }
    }

    public void HideConfirmButton()
    {
        if (ConfirmButton != null)
            ConfirmButton.SetActive(false);
    }

    // ---------------- MENUS ----------------

    public void OpenBuildMenu()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(writingSoundEffect);
        AssignWorkerUI.Instance?.ClearSelectionUI();

        if (BuildMenu != null) BuildMenu.SetActive(true);

        ToolSelectUpgrade.Instance?.ClearSelection();
        RefineMaterialsUI.Instance?.ClearSelection();

        RenderConfirmButton();
    }

    public void CloseBuildMenu()
    {
        if (BuildMenu != null) BuildMenu.SetActive(false);
        RenderConfirmButton();
    }

    public void OpenUpgradeToolMenu()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(writingSoundEffect);
        AssignWorkerUI.Instance?.ClearSelectionUI();

        if (UpgradeToolMenu != null) UpgradeToolMenu.SetActive(true);

        SelectedBuildTracker.Instance?.ClearSelection();
        RefineMaterialsUI.Instance?.ClearSelection();

        RenderConfirmButton();
    }

    public void CloseUpgradeToolMenu()
    {
        if (UpgradeToolMenu != null) UpgradeToolMenu.SetActive(false);

        ToolSelectUpgrade.Instance?.ClearSelection();
        RenderConfirmButton();
    }

    public void OpenRefineMaterialMenu()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(writingSoundEffect);
        AssignWorkerUI.Instance?.ClearSelectionUI();

        if (RefineMaterialMenu != null) RefineMaterialMenu.SetActive(true);

        RenderConfirmButton();
    }

    public void CloseRefineMaterialMenu()
    {
        if (RefineMaterialMenu != null) RefineMaterialMenu.SetActive(false);

        RefineMaterialsUI.Instance?.ClearSelection();
        RenderConfirmButton();
    }

    public void OpenAssignWorkerMenu()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(writingSoundEffect);
        if (AssignWorkerMenu != null)
            AssignWorkerMenu.SetActive(true);

        AssignWorkerUI.Instance?.RefreshDropdowns();
        AssignWorkerUI.Instance?.ClearSelectionUI();

        RenderConfirmButton();
    }


    public void CloseAssignWorkerMenu()
    {
        if (AssignWorkerMenu != null)
            AssignWorkerMenu.SetActive(false);

        AssignWorkerUI.Instance?.ClearSelectionUI();
        RenderConfirmButton();
    }
}
