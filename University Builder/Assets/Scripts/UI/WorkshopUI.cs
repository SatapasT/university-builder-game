using UnityEngine;

public class WorkshopUI : MonoBehaviour
{
    [SerializeField] private GameObject workshopPanel;

    public void OpenMenu()
    {
        workshopPanel.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseMenu()
    {
        workshopPanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
