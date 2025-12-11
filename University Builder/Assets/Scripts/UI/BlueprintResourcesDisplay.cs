using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlueprintResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resourcesDisplay;

    private void Update()
    {
        UpdateUI();
    }

    private Dictionary<ResourceType, int> UpdateAvailableResources()
    {
        return ResourcesManager.Instance.GetAllResources();
    }

    private void UpdateUI()
    {
        if (ResourcesManager.Instance == null) return;

        resourcesDisplay.text = "";

        var allResources = UpdateAvailableResources();

        foreach (var pair in allResources)
        {
            resourcesDisplay.text += $"{pair.Key} - {pair.Value}\n";
            
        }
    }
}
