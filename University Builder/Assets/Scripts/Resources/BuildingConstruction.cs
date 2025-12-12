using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuildingConstruction : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float maxBuildTime = 30f;
    [SerializeField] private float builderWorkTime = 5f;

    [Header("Translucent Visual")]
    [Range(0.1f, 1f)]
    [SerializeField] private float constructionAlpha = 0.35f;

    public bool IsFinished { get; private set; }
    public bool IsBuilding { get; private set; }

    private float builderWorkTimer;
    private Coroutine fallbackRoutine;

    private readonly List<Material> cachedMaterials = new();
    private readonly Dictionary<Material, Color> originalColors = new();

    private BuildType buildType = BuildType.None;

    public void SetBuildType(BuildType type)
    {
        buildType = type;
    }

    public void BeginConstruction()
    {
        if (IsBuilding || IsFinished)
            return;

        CacheMaterials();

        IsBuilding = true;
        IsFinished = false;
        builderWorkTimer = 0f;

        ApplyTranslucent(true);

        if (fallbackRoutine != null)
            StopCoroutine(fallbackRoutine);

        fallbackRoutine = StartCoroutine(FallbackFinishRoutine());
    }

    public void NotifyBuilderWorking(float deltaTime)
    {
        if (!IsBuilding || IsFinished)
            return;

        builderWorkTimer += deltaTime;

        if (builderWorkTimer >= builderWorkTime)
            FinishConstruction();
    }

    private IEnumerator FallbackFinishRoutine()
    {
        yield return new WaitForSeconds(maxBuildTime);

        if (!IsFinished)
            FinishConstruction();
    }

    public void FinishConstruction()
    {
        if (IsFinished)
            return;

        IsFinished = true;
        IsBuilding = false;

        if (fallbackRoutine != null)
            StopCoroutine(fallbackRoutine);

        ApplyTranslucent(false);

        if (BuildProgressTracker.Instance != null && buildType != BuildType.None)
            BuildProgressTracker.Instance.MarkBuilt(buildType);
    }

    private void CacheMaterials()
    {
        cachedMaterials.Clear();
        originalColors.Clear();

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (Renderer r in renderers)
        {
            if (r == null) continue;

            foreach (Material mat in r.materials)
            {
                if (mat == null || cachedMaterials.Contains(mat))
                    continue;

                cachedMaterials.Add(mat);

                if (mat.HasProperty("_Color"))
                    originalColors[mat] = mat.color;
            }
        }
    }

    private void ApplyTranslucent(bool translucent)
    {
        foreach (Material mat in cachedMaterials)
        {
            if (mat == null || !mat.HasProperty("_Color"))
                continue;

            if (!mat.HasProperty("_Mode"))
                continue;

            if (translucent)
            {
                SetMaterialTransparent(mat);
                Color c = mat.color;
                c.a = constructionAlpha;
                mat.color = c;
            }
            else
            {
                SetMaterialOpaque(mat);

                if (originalColors.TryGetValue(mat, out var original))
                    mat.color = original;
            }
        }
    }

    private void SetMaterialTransparent(Material mat)
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

    private void SetMaterialOpaque(Material mat)
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
