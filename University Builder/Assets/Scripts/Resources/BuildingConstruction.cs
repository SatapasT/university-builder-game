using System.Collections;
using UnityEngine;

public class BuildingConstruction : MonoBehaviour
{
    [SerializeField] private float buildTimeSeconds = 30f;

    private MeshRenderer meshRenderer;
    private Material originalMaterial;
    private Color originalColor;
    private bool isBuilding = false;

    public bool IsBuilding => isBuilding;
    public bool IsFinished { get; private set; }

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            originalMaterial = meshRenderer.material;
            originalColor = originalMaterial.color;
        }

        // IMPORTANT: do NOT SetActive(false) here anymore.
        // WorkshopUI will hide/show the castle.
    }

    public void StartConstruction()
    {
        if (isBuilding)
            return;

        isBuilding = true;
        IsFinished = false;

        StartCoroutine(BuildRoutine());
    }

    private IEnumerator BuildRoutine()
    {
        // Construction look (blue + transparent)
        if (meshRenderer != null)
        {
            Color buildColor = new Color(0.2f, 0.4f, 1f, 0.4f);
            meshRenderer.material.color = buildColor;
            SetMaterialToTransparent(meshRenderer.material);
        }

        yield return new WaitForSeconds(buildTimeSeconds);

        // Finished look
        if (meshRenderer != null)
        {
            meshRenderer.material.color = originalColor;
            SetMaterialToOpaque(meshRenderer.material);
        }

        isBuilding = false;
        IsFinished = true;
    }

    private void SetMaterialToTransparent(Material mat)
    {
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }

    private void SetMaterialToOpaque(Material mat)
    {
        mat.SetFloat("_Mode", 0);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);
        mat.EnableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = -1;
    }
}
