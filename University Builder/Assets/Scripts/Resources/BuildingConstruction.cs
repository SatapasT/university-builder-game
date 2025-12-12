using UnityEngine;
using System.Collections.Generic;

public class BuildingConstruction : MonoBehaviour
{
    [Header("Translucent Visual")]
    [Range(0.1f, 1f)]
    [SerializeField] private float constructionAlpha = 0.35f;

    public bool IsFinished { get; private set; }
    public bool IsBuilding { get; private set; }

    private readonly List<Material> cachedMaterials = new();
    private readonly Dictionary<Material, Color> originalColors = new();

    private BuildType buildType = BuildType.None;

    private float requiredSeconds = 10f;
    private float progressSeconds = 0f;

    public void SetBuildType(BuildType type)
    {
        buildType = type;
    }

    public float GetRemainingSeconds()
    {
        if (!IsBuilding || IsFinished) return 0f;
        return Mathf.Max(0f, requiredSeconds - progressSeconds);
    }

    public float GetRequiredSeconds() => requiredSeconds;
    public float GetProgressSeconds() => progressSeconds;

    public void BeginConstruction()
    {
        if (IsBuilding || IsFinished)
            return;

        var info = BuildDatabase.Get(buildType);
        requiredSeconds = info != null ? Mathf.Max(1f, info.BuildTimeSeconds) : 10f;

        CacheMaterials();

        IsBuilding = true;
        IsFinished = false;

        progressSeconds = 0f;
        ApplyTranslucent(true);
    }

    private void Update()
    {
        if (!IsBuilding || IsFinished)
            return;

        int workers = WorkerManager.Instance != null ? WorkerManager.Instance.GetAssignedCount(buildType) : 0;
        if (workers <= 0)
            return; 

        progressSeconds += workers * Time.deltaTime;

        if (progressSeconds >= requiredSeconds)
            FinishConstruction();
    }

    public void FinishConstruction()
    {
        if (IsFinished)
            return;

        IsFinished = true;
        IsBuilding = false;

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
