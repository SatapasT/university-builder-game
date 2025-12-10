using UnityEngine;

public class  RockInteractable : MonoBehaviour, IInteractable
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
        ResourcesManager.Instance.AddResource(ResourceType.Stone, 1);
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
