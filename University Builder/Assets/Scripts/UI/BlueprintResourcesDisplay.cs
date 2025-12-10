using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlueprintResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resourcesDisplay;

    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        resourcesDisplay.text = "";

        foreach (var item in resources)
        {
            resourcesDisplay.text += $"{item.Key}: {item.Value}\n";
        }
    }
    public void SetResource(ResourceType type, int amount)
    {
        resources[type] = amount;
        UpdateUI();
    }
}
