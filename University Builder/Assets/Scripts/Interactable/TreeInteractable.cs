using UnityEngine;

public class Oak_Tree : MonoBehaviour, IInteractable
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
        ResourcesManager.Instance.AddResource(ResourceType.Wood, 1);
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
