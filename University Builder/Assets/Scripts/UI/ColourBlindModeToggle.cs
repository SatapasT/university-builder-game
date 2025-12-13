using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class ColourblindModeToggle : MonoBehaviour
{
    [SerializeField] private PostProcessVolume colourblindVolume;
    [SerializeField] private Toggle toggle;

    private void Awake()
    {
        if (toggle == null)
            toggle = GetComponent<Toggle>(); 

        if (colourblindVolume != null) colourblindVolume.weight = 0f;

        if (toggle != null) toggle.SetIsOnWithoutNotify(false);

        if (toggle != null) toggle.onValueChanged.AddListener(SetColourblind);
    }

    private void OnDestroy()
    {
        if (toggle != null) toggle.onValueChanged.RemoveListener(SetColourblind);
    }

    public void SetColourblind(bool enabled)
    {
        Debug.Log($"[ColourblindModeToggle] enabled = {enabled}");
        if (colourblindVolume == null) return;
        colourblindVolume.weight = enabled ? 1f : 0f;
    }
}
