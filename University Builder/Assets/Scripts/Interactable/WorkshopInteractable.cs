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
        Debug.Log("Workbench Interact");
        if (workshopUI != null)
        {
            workshopUI.ToggleMenu();
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
