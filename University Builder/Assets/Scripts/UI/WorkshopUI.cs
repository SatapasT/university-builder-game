using System.Collections.Generic;
using UnityEngine;

public class WorkshopUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> AllWorkshopPanels = new();
    [SerializeField] private List<GameObject> MainWorkshopPanels = new();

    [SerializeField] private GameObject BuildMenu;
    [SerializeField] private GameObject ConfirmButton;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        foreach (var panel in AllWorkshopPanels)
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

    public void RenderConfirmButton()
    {
        ConfirmButton.SetActive(true);
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
