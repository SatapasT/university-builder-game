using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class SelectedBuildTracker : MonoBehaviour
{
    [System.Serializable]
    public class BuildObjectEntry
    {
        public BuildType type;
        public GameObject buildingObject;
        public Transform buildSitePoint;
    }

    public static SelectedBuildTracker Instance { get; private set; }

    [SerializeField] private GameObject InfoText;

    [Header("Building Execution")]
    [SerializeField] private List<BuildObjectEntry> buildObjects = new();
    [SerializeField] private GameObject builderPrefab;
    [SerializeField] private Transform builderSpawnPoint;

    public BuildType CurrentBuild { get; private set; } = BuildType.None;

    private TextMeshProUGUI infoTextMesh;
    private readonly Dictionary<BuildType, BuildObjectEntry> buildMap = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (InfoText != null)
        {
            infoTextMesh = InfoText.GetComponent<TextMeshProUGUI>();
            InfoText.SetActive(false);
        }

        buildMap.Clear();
        foreach (var entry in buildObjects)
        {
            if (entry == null || entry.buildingObject == null) continue;

            buildMap[entry.type] = entry;
            entry.buildingObject.SetActive(false);
        }
    }

    private bool HasEnoughResources(BuildInfo buildInfo, Dictionary<ResourceType, int> playerResources)
    {
        if (buildInfo == null || playerResources == null)
            return false;

        foreach (ResourceAmount cost in buildInfo.Costs)
        {
            if (!playerResources.TryGetValue(cost.type, out int currentAmount) ||
                currentAmount < cost.amount)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsBlockedByBuildState(BuildType type)
    {
        if (BuildProgressTracker.Instance == null)
            return false;

        var state = BuildProgressTracker.Instance.GetState(type);
        return state == BuildProgressTracker.BuildState.Built ||
               state == BuildProgressTracker.BuildState.InProgress;
    }

    public bool CanAffordCurrentBuild()
    {
        if (ResourcesManager.Instance == null) return false;
        if (CurrentBuild == BuildType.None) return false;

        if (IsBlockedByBuildState(CurrentBuild))
            return false;

        BuildInfo buildInfo = BuildDatabase.Get(CurrentBuild);
        if (buildInfo == null) return false;

        var playerResources = ResourcesManager.Instance.GetAllResources();
        return HasEnoughResources(buildInfo, playerResources);
    }

    public bool TryStartConstruction()
    {
        if (ResourcesManager.Instance == null) return false;
        if (CurrentBuild == BuildType.None) return false;

        if (IsBlockedByBuildState(CurrentBuild))
            return false;

        BuildInfo buildInfo = BuildDatabase.Get(CurrentBuild);
        if (buildInfo == null) return false;

        var playerResources = ResourcesManager.Instance.GetAllResources();
        if (!HasEnoughResources(buildInfo, playerResources)) return false;

        foreach (ResourceAmount cost in buildInfo.Costs)
            ResourcesManager.Instance.DeductResources(cost.type, cost.amount);

        StartConstruction(CurrentBuild);
        return true;
    }

    private void StartConstruction(BuildType buildType)
    {
        if (!buildMap.TryGetValue(buildType, out var entry) || entry == null || entry.buildingObject == null)
        {
            Debug.LogError($"SelectedBuildTracker: No building object assigned for {buildType}. Add it to buildObjects list in Inspector.");
            return;
        }

        if (BuildProgressTracker.Instance != null)
            BuildProgressTracker.Instance.MarkInProgress(buildType);

        GameObject buildingObject = entry.buildingObject;
        buildingObject.SetActive(true);

        var buildingConstruction = buildingObject.GetComponent<BuildingConstruction>();
        if (buildingConstruction == null)
        {
            Debug.LogError($"SelectedBuildTracker: BuildingConstruction missing on {buildType} object ({buildingObject.name}).");
            return;
        }

        buildingConstruction.SetBuildType(buildType);

        buildingConstruction.BeginConstruction();

        if (builderPrefab == null || builderSpawnPoint == null)
        {
            Debug.LogWarning("SelectedBuildTracker: builderPrefab or builderSpawnPoint not assigned.");
            return;
        }

        GameObject builderInstance = Instantiate(builderPrefab, builderSpawnPoint.position, builderSpawnPoint.rotation);

        BuilderAgent builderAgent = builderInstance.GetComponent<BuilderAgent>();
        if (builderAgent == null)
        {
            Debug.LogError("SelectedBuildTracker: BuilderAgent missing on builderPrefab.");
            return;
        }

        Transform site = entry.buildSitePoint != null ? entry.buildSitePoint : buildingObject.transform;
        builderAgent.Initialize(site, buildingConstruction);
    }

    public void ClearSelection()
    {
        CurrentBuild = BuildType.None;

        if (infoTextMesh != null)
        {
            infoTextMesh.text = string.Empty;
            if (InfoText != null) InfoText.SetActive(false);
        }
    }

    public void SelectBuild(BuildType buildType)
    {
        CurrentBuild = buildType;

        if (infoTextMesh == null) return;

        BuildInfo buildInfo = BuildDatabase.Get(buildType);
        if (buildInfo == null)
        {
            infoTextMesh.text = string.Empty;
            if (InfoText != null) InfoText.SetActive(false);
            return;
        }

        if (InfoText != null) InfoText.SetActive(true);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"<b><color=yellow>{buildInfo.Nickname}</color></b>");

        if (BuildProgressTracker.Instance != null)
        {
            var st = BuildProgressTracker.Instance.GetState(buildType);
            if (st == BuildProgressTracker.BuildState.Built)
                sb.AppendLine("<color=green><b>BUILT</b></color>");
            else if (st == BuildProgressTracker.BuildState.InProgress)
                sb.AppendLine("<color=orange><b>IN PROGRESS</b></color>");
        }

        if (!string.IsNullOrWhiteSpace(buildInfo.Info))
        {
            sb.AppendLine();
            sb.AppendLine(buildInfo.Info);
        }

        Dictionary<ResourceType, int> playerResources =
            ResourcesManager.Instance != null ? ResourcesManager.Instance.GetAllResources() : null;

        sb.AppendLine();
        sb.AppendLine("<b><color=orange>Costs</color></b>");

        foreach (ResourceAmount resourceAmount in buildInfo.Costs)
        {
            int currentAmount = 0;
            playerResources?.TryGetValue(resourceAmount.type, out currentAmount);

            sb.AppendLine($"- {currentAmount}/{resourceAmount.amount} {resourceAmount.type}");
        }

        infoTextMesh.text = sb.ToString();
    }

    public bool HasSelection => CurrentBuild != BuildType.None;
}
