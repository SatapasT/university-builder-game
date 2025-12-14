using UnityEngine;

public class Workbench : MonoBehaviour, IInteractable
{
    private Renderer rend;
    private Color defaultColor;

    [SerializeField] private WorkshopUI workshopUI;

    public AudioClip clickSoundEffect;
    private AudioSource audioSource;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        defaultColor = rend.material.color;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void Interact()
    {
        if (workshopUI != null)
        {
            workshopUI.ToggleMenu();
            BasicTutorial.Instance?.NotifyWorkshopOpened();
            audioSource.PlayOneShot(clickSoundEffect);
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
