using UnityEngine;

public class Workbench : MonoBehaviour, IInteractable
{
    private Renderer rend;
    private Color defaultColor;

    [SerializeField] private WorkshopUI workshopUI;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        defaultColor = rend.material.color;
    }

    public void Interact()
    {
        if (workshopUI != null)
        {
            workshopUI.ToggleMenu();
            BasicTutorial.Instance?.NotifyWorkshopOpened();
        }
    }

    public void OnFocus()
    {
        rend.material.color = Color.yellow;
    }

    public void OnLoseFocus()
    {
        rend.material.color = defaultColor;
    }
}
