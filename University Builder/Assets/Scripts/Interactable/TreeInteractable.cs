using UnityEngine;

public class TreeInteractable : MonoBehaviour, IInteractable
{
    private Renderer rend;
    private Color defaultColor;

    public int harvestAbleAmount = 30;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        defaultColor = rend.material.color;
    }

    public void Interact()
    {
        ResourcesManager.Instance.AddResource(ResourceType.Wood, 1);
        harvestAbleAmount -= 1;
        if (harvestAbleAmount <= 0)
        {
            Destroy(gameObject);
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
