using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class SelectedBuildTracker : MonoBehaviour
{
    public static SelectedBuildTracker Instance { get; private set; }

    [SerializeField] private GameObject InfoText;

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
    }

    public void SelectBuild(BuildType buildType)
    {
        CurrentBuild = buildType;
        BuildInfo buildInfo = BuildDatabase.Get(buildType);

        if (buildInfo == null || infoTextMesh == null)
            return;

        InfoText.SetActive(true);

        StringBuilder displayTextBuilder = new StringBuilder();

        displayTextBuilder.AppendLine(
            $"<b><color=yellow>{buildInfo.Nickname}</color></b>");

        if (!string.IsNullOrWhiteSpace(buildInfo.Info))
        {
            displayTextBuilder.AppendLine();
            displayTextBuilder.AppendLine(buildInfo.Info);
        }

        Dictionary<ResourceType, int> playerResources = null;
        if (ResourcesManager.Instance != null)
        {
            playerResources = ResourcesManager.Instance.GetAllResources();
        }

        displayTextBuilder.AppendLine();
        displayTextBuilder.AppendLine("<b><color=orange>Costs</color></b>");

        foreach (ResourceAmount resourceAmount in buildInfo.Costs)
        {
            string resourceName = resourceAmount.type.ToString();
            int requiredAmount = resourceAmount.amount;

            playerResources.TryGetValue(resourceAmount.type, out int currentAmount);
            string costLine = $"- {currentAmount}/{requiredAmount} {resourceName}";

            displayTextBuilder.AppendLine(costLine);
        }

        infoTextMesh.text = displayTextBuilder.ToString();
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

    public bool HasSelection => CurrentBuild != BuildType.None;
}
