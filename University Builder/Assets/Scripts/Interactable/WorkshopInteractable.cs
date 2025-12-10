using UnityEngine;

public class Workbench : MonoBehaviour, IInteractable
{
    private Renderer rend;
    private Color defaultColor;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        defaultColor = rend.material.color;
    }

    public void Interact()
    {
        Debug.Log("Using the workbench!");
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
