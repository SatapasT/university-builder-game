using UnityEngine;

public class  RockInteractable : MonoBehaviour, IInteractable
{
    private Renderer rend;
    private Color defaultColor;

    public int harvestAbleAmount = 30;

    public AudioClip rockSoundClip;
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
        audioSource.PlayOneShot(rockSoundClip);
        int playerHarvestAmount = PlayerStats.Instance.GetStoneHarvestAmount();
        ResourcesManager.Instance.AddResource(ResourceType.Stone, playerHarvestAmount);
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
