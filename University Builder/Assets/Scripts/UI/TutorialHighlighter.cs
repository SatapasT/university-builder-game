using UnityEngine;

public class TutorialHighlighter : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private float pulseScale = 1.15f;
    [SerializeField] private float pulseSpeed = 3f;

    [Header("Glow")]
    [SerializeField] private bool enableGlow = false;
    [SerializeField] private Color glowColor = Color.yellow;

    private Vector3 originalScale;
    private Renderer[] renderers;
    private bool isActive;

    private void Awake()
    {
        originalScale = transform.localScale;
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Update()
    {
        if (!isActive) return;

        float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * (pulseScale - 1f);
        transform.localScale = originalScale * scale;
    }

    public void EnableHighlight()
    {
        isActive = true;

        if (enableGlow)
        {
            foreach (var r in renderers)
            {
                if (r.material.HasProperty("_EmissionColor"))
                {
                    r.material.EnableKeyword("_EMISSION");
                    r.material.SetColor("_EmissionColor", glowColor);
                }
            }
        }
    }

    public void DisableHighlight()
    {
        isActive = false;
        transform.localScale = originalScale;

        if (enableGlow)
        {
            foreach (var r in renderers)
            {
                if (r.material.HasProperty("_EmissionColor"))
                {
                    r.material.SetColor("_EmissionColor", Color.black);
                }
            }
        }
    }
}
