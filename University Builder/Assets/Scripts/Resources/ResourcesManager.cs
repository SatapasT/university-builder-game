using System.Collections;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class ResourcesManager : MonoBehaviour
{
    public static ResourcesManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI popupText;

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

        popupText.gameObject.SetActive(false);

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

        popupText.text = $"{type}: {resources[type]}";
        popupText.gameObject.SetActive(true);

        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    public void DeductResources(ResourceType type, int amount)
    {
        if (!resources.ContainsKey(type))
            resources[type] = 0;

        resources[type] -= amount;

        popupText.text = $"{type}: {resources[type]}";
        popupText.gameObject.SetActive(true);

        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        popupText.gameObject.SetActive(false);
        hideRoutine = null;
    }

    public Dictionary<ResourceType, int> GetAllResources()
    {
        return resources;
    }
}
