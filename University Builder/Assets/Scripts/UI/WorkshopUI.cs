using System.Collections.Generic;
using UnityEngine;

public class WorkshopUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> workshopPanels = new();  // <— NOW A LIST!

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        foreach (var panel in workshopPanels)
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
        foreach (var panel in workshopPanels)
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
        foreach (var panel in workshopPanels)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        IsOpen = false;
    }
}
