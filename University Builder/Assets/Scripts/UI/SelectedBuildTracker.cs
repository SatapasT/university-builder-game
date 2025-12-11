using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class SelectedBuildTracker : MonoBehaviour
{
    public static SelectedBuildTracker Instance { get; private set; }

    [SerializeField] private GameObject InfoText;

    [Header("Building Execution")]
    [SerializeField] private GameObject castleObject;
    [SerializeField] private GameObject builderPrefab;
    [SerializeField] private Transform builderSpawnPoint;

    public BuildType CurrentBuild { get; private set; } = BuildType.None;

    private TextMeshProUGUI infoTextMesh;

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

        if (castleObject != null)
        {
            castleObject.SetActive(false);
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

    public bool CanAffordCurrentBuild()
    {
        if (ResourcesManager.Instance == null)
            return false;

        if (CurrentBuild == BuildType.None)
            return false;

        BuildInfo buildInfo = BuildDatabase.Get(CurrentBuild);
        if (buildInfo == null)
            return false;

        Dictionary<ResourceType, int> playerResources = ResourcesManager.Instance.GetAllResources();
        return HasEnoughResources(buildInfo, playerResources);
    }
    public bool TryStartConstruction()
    {
        if (ResourcesManager.Instance == null)
            return false;

        if (CurrentBuild == BuildType.None)
            return false;

        BuildInfo buildInfo = BuildDatabase.Get(CurrentBuild);
        if (buildInfo == null)
            return false;

        Dictionary<ResourceType, int> playerResources = ResourcesManager.Instance.GetAllResources();
        if (!HasEnoughResources(buildInfo, playerResources))
            return false;

        foreach (ResourceAmount cost in buildInfo.Costs)
        {
            ResourcesManager.Instance.DeductResources(cost.type, cost.amount);
        }

        if (CurrentBuild == BuildType.Castle)
        {
            StartCastleConstruction(buildInfo);
        }

        return true;
    }

    private void StartCastleConstruction(BuildInfo buildInfo)
    {
        if (castleObject == null)
        {
            Debug.LogError("SelectedBuildTracker: castleObject is not assigned.");
            return;
        }

        castleObject.SetActive(true);

        BuildingConstruction buildingConstruction =
            castleObject.GetComponent<BuildingConstruction>();

        if (buildingConstruction == null)
        {
            Debug.LogError("SelectedBuildTracker: BuildingConstruction component missing on castleObject.");
            return;
        }

        buildingConstruction.StartConstruction();

        if (builderPrefab != null && builderSpawnPoint != null)
        {
            GameObject builderInstance = Instantiate(
                builderPrefab,
                builderSpawnPoint.position,
                builderSpawnPoint.rotation);

            BuilderAgent builderAgent = builderInstance.GetComponent<BuilderAgent>();
            if (builderAgent != null)
            {
                builderAgent.Initialize(castleObject.transform, buildingConstruction);
            }
            else
            {
                Debug.LogError("SelectedBuildTracker: BuilderAgent missing on builderPrefab.");
            }
        }
    }



    public void ClearSelection()
    {
        CurrentBuild = BuildType.None;

        if (infoTextMesh != null)
        {
            infoTextMesh.text = string.Empty;
            InfoText.SetActive(false);
        }
    }

    public void SelectBuild(BuildType buildType)
    {
        CurrentBuild = buildType;

        if (infoTextMesh == null)
            return;

        BuildInfo buildInfo = BuildDatabase.Get(buildType);

        if (buildInfo == null)
        {
            infoTextMesh.text = string.Empty;
            InfoText.SetActive(false);
            return;
        }

        InfoText.SetActive(true);

        StringBuilder displayTextBuilder = new StringBuilder();

        // Title
        displayTextBuilder.AppendLine($"<b><color=yellow>{buildInfo.Nickname}</color></b>");

        // Description
        if (!string.IsNullOrWhiteSpace(buildInfo.Info))
        {
            displayTextBuilder.AppendLine();
            displayTextBuilder.AppendLine(buildInfo.Info);
        }

        // Player resources (for current/required display)
        Dictionary<ResourceType, int> playerResources = null;
        if (ResourcesManager.Instance != null)
        {
            playerResources = ResourcesManager.Instance.GetAllResources();
        }

        // Costs
        displayTextBuilder.AppendLine();
        displayTextBuilder.AppendLine("<b><color=orange>Costs</color></b>");

        foreach (ResourceAmount resourceAmount in buildInfo.Costs)
        {
            string resourceName = resourceAmount.type.ToString();
            int requiredAmount = resourceAmount.amount;

            int currentAmount = 0;
            playerResources?.TryGetValue(resourceAmount.type, out currentAmount);

            string costLine = $"- {currentAmount}/{requiredAmount} {resourceName}";
            displayTextBuilder.AppendLine(costLine);
        }

        infoTextMesh.text = displayTextBuilder.ToString();
    }


    public bool HasSelection => CurrentBuild != BuildType.None;
}
