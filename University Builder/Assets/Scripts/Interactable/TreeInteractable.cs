using UnityEngine;

public class TreeInteractable : MonoBehaviour, IInteractable
{
    private Renderer rend;
    private Color defaultColor;

    public int harvestAbleAmount = 30;

    public AudioClip treeSoundClip;
    private AudioSource audioSource;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        defaultColor = rend.material.color;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Interact()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(treeSoundClip);
        int playerHarvestAmount = PlayerStats.Instance.GetWoodHarvestAmount();
        ResourcesManager.Instance.AddResource(ResourceType.Wood, playerHarvestAmount);
        harvestAbleAmount -= playerHarvestAmount;
        audioSource.Play();
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
