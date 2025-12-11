using UnityEngine;

public class  RockInteractable : MonoBehaviour, IInteractable
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
        int playerHarvestAmount = PlayerStats.Instance.GetStoneHarvestAmount();
        ResourcesManager.Instance.AddResource(ResourceType.Stone, playerHarvestAmount);
        harvestAbleAmount -= playerHarvestAmount;
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
