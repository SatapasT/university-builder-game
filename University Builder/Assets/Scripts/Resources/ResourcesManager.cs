using System.Collections;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ResourcesManager : MonoBehaviour
{
    public static ResourcesManager Instance { get; private set; }

    [Header("Popup UI")]
    [SerializeField] private TextMeshProUGUI popupText;

    [Header("Popup Icons (5 GameObjects)")]
    [SerializeField] private GameObject woodIcon;
    [SerializeField] private GameObject stoneIcon;
    [SerializeField] private GameObject goldIcon;
    [SerializeField] private GameObject ironIcon;
    [SerializeField] private GameObject planksIcon;

    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();
    private Coroutine hideRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (popupText != null)
            popupText.gameObject.SetActive(false);

        HideAllIcons();

        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            resources[type] = 0;
        }
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (!resources.ContainsKey(type))
            resources[type] = 0;

        resources[type] += amount;
        ShowPopup(type);
    }

    public void DeductResources(ResourceType type, int amount)
    {
        if (!resources.ContainsKey(type))
            resources[type] = 0;

        resources[type] -= amount;
        ShowPopup(type);
    }

    private void ShowPopup(ResourceType type)
    {
        // Text
        if (popupText != null)
        {
            popupText.text = $"{type}: {resources[type]}";
            popupText.gameObject.SetActive(true);
        }

        // Icon
        HideAllIcons();
        GetIconObject(type)?.SetActive(true);

        // Reset timer
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private GameObject GetIconObject(ResourceType type)
    {
        return type switch
        {
            ResourceType.Wood => woodIcon,
            ResourceType.Stone => stoneIcon,
            ResourceType.Gold => goldIcon,
            ResourceType.Iron => ironIcon,
            ResourceType.Planks => planksIcon,
            _ => null
        };
    }

    private void HideAllIcons()
    {
        if (woodIcon != null) woodIcon.SetActive(false);
        if (stoneIcon != null) stoneIcon.SetActive(false);
        if (goldIcon != null) goldIcon.SetActive(false);
        if (ironIcon != null) ironIcon.SetActive(false);
        if (planksIcon != null) planksIcon.SetActive(false);
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(2f);

        if (popupText != null)
            popupText.gameObject.SetActive(false);

        HideAllIcons();
        hideRoutine = null;
    }

    public Dictionary<ResourceType, int> GetAllResources()
    {
        return resources;
    }
}
