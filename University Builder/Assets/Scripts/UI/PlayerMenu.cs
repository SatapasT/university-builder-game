using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class PlayerMenu : MonoBehaviour
{
    public static PlayerMenu Instance { get; private set; }

    [SerializeField] private List<GameObject> AllMenuItem = new();
    [SerializeField] private List<GameObject> MainMenuItem = new();

    [SerializeField] private GameObject MenuBackdrop;
    [SerializeField] private GameObject MainButtonContainer;

    [SerializeField] private GameObject PlayerStatsContainer;
    [SerializeField] private GameObject WorkerAssignmentContainer;
    [SerializeField] private GameObject AccessibilityContainer;

    [SerializeField] private GameObject PlayerModel;
    [SerializeField] private GameObject TeleportLocation;

    [Header("Player Stats UI")]
    [SerializeField] private TextMeshProUGUI playerAxeInfo;
    [SerializeField] private TextMeshProUGUI playerPickaxeInfo;
    [SerializeField] private TextMeshProUGUI playerBootsInfo;

    [Header("Worker Assignment UI")]
    [SerializeField] private TextMeshProUGUI workerAssignmentInfoText;

    public bool IsOpen { get; private set; }

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (GameObject panel in AllMenuItem)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        IsOpen = false;
    }

    public void ToggleMenu()
    {
        if (IsOpen) CloseMenu();
        else OpenMenu();
    }

    public void OpenMenu()
    {
        UIManager.Instance.SetMenuState(true);
        foreach (GameObject panel in MainMenuItem)
        {
            if (panel != null)
                panel.SetActive(true);
        }

        IsOpen = true;
    }

    public void CloseMenu()
    {
        UIManager.Instance.SetMenuState(false);
        foreach (GameObject panel in AllMenuItem)
        {
            if (panel != null)
                panel.SetActive(false);
        }

        IsOpen = false;
    }

    public void TeleportToWorkbench()
    {
        var controller = PlayerModel.GetComponent<CharacterController>();
        if (controller != null)
            controller.enabled = false;

        PlayerModel.transform.position = TeleportLocation.transform.position;

        if (controller != null)
            controller.enabled = true;

        CloseMenu();
    }

    public void openMainMenu()
    {
        MainButtonContainer.SetActive(true);
    }
    public void closeMainMenu()
    {
        MainButtonContainer.SetActive(false);
    }

    private string BuildToolLine(ToolType type)
    {
        if (PlayerStats.Instance == null)
            return $"<color=red><b>{type}</b>: PlayerStats missing</color>";

        ToolInfo info = PlayerStats.Instance.GetCurrentToolInfo(type);

        if (info == null)
            return $"<color=red><b>{type}</b>: None</color>";

        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"<b><color=yellow>{info.Name}</color></b>");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(info.Description))
        {
            sb.AppendLine($"<color=#DDDDDD>{info.Description}</color>");
            sb.AppendLine();
        }

        sb.AppendLine("<b><color=orange>STATS</color></b>");

        bool hasStat = false;

        if (info.HarvestAmount > 0)
        {
            hasStat = true;
            sb.AppendLine($"<color=#90EE90>- Harvest:</color> <b>+{info.HarvestAmount}</b> per action");
        }

        if (info.MovementSpeedBonus > 0f && info.MovementSpeedBonus != 1f)
        {
            hasStat = true;
            sb.AppendLine($"<color=#ADD8E6>- Move Speed:</color> <b>x{info.MovementSpeedBonus:0.00}</b>");
        }

        if (!hasStat)
        {
            sb.AppendLine("<color=grey>- No bonuses</color>");
        }

        return sb.ToString().TrimEnd();
    }

    public void openPlayerStats()
    {
        if (PlayerStatsContainer != null)
            PlayerStatsContainer.SetActive(true);

        if (PlayerStats.Instance == null)
        {
            if (playerAxeInfo != null) playerAxeInfo.text = "No stats";
            if (playerPickaxeInfo != null) playerPickaxeInfo.text = "No stats";
            if (playerBootsInfo != null) playerBootsInfo.text = "No stats";
            return;
        }

        if (playerAxeInfo != null)
            playerAxeInfo.text = BuildToolLine(ToolType.Axe);

        if (playerPickaxeInfo != null)
            playerPickaxeInfo.text = BuildToolLine(ToolType.Pickaxe);

        if (playerBootsInfo != null)
            playerBootsInfo.text = BuildToolLine(ToolType.Boots);
    }

    public void closePlayerStats()
    {
        PlayerStatsContainer.SetActive(false);
    }

    private string BuildWorkerAssignmentText()
    {
        if (WorkerManager.Instance == null)
            return "<color=red><b>WorkerManager missing</b></color>";

        int max = WorkerManager.Instance.GetMaxWorkers();
        if (max <= 0)
            return "<color=grey>No workers available.</color>";

        int idle = 0, build = 0, wood = 0, stone = 0;

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("<b><color=orange>WORKER ASSIGNMENTS</color></b>");
        sb.AppendLine("<color=#BBBBBB>Who is doing what right now</color>");
        sb.AppendLine();

        for (int i = 1; i <= max; i++)
        {
            var task = WorkerManager.Instance.GetWorkerFullTask(i);

            string taskColor;
            switch (task.type)
            {
                case WorkerManager.WorkerTaskType.None:
                    taskColor = "grey";
                    idle++;
                    break;

                case WorkerManager.WorkerTaskType.Build:
                    taskColor = "orange";
                    build++;
                    break;

                case WorkerManager.WorkerTaskType.GatherWood:
                    taskColor = "#90EE90"; 
                    wood++;
                    break;

                case WorkerManager.WorkerTaskType.GatherStone:
                    taskColor = "#ADD8E6";
                    stone++;
                    break;

                default:
                    taskColor = "white";
                    break;
            }

            sb.AppendLine(
                $"<b>Worker {i}</b>  <color=#777777>→</color>  <color={taskColor}><b>{task.DisplayName()}</b></color>"
            );
        }

        sb.AppendLine();
        sb.AppendLine("<b><color=orange>SUMMARY</color></b>");
        sb.AppendLine($"<color=grey>- Idle:</color> <b>{idle}</b>");
        sb.AppendLine($"<color=orange>- Building:</color> <b>{build}</b>");
        sb.AppendLine($"<color=#90EE90>- Gathering Wood:</color> <b>{wood}</b>");
        sb.AppendLine($"<color=#ADD8E6>- Gathering Stone:</color> <b>{stone}</b>");

        return sb.ToString().TrimEnd();
    }

    public void openWorkerAssignment()
    {
        if (WorkerAssignmentContainer != null)
            WorkerAssignmentContainer.SetActive(true);

        if (workerAssignmentInfoText != null)
            workerAssignmentInfoText.text = BuildWorkerAssignmentText();
    }

    public void closeWorkerAssignment()
    {
        if (WorkerAssignmentContainer != null)
            WorkerAssignmentContainer.SetActive(false);

    }

    public void openAccessibilityOptions()
    {
        if (AccessibilityContainer != null)
            AccessibilityContainer.SetActive(true);
    }

    public void closeAccessibilityOptions()
    {
        if (AccessibilityContainer != null)
            AccessibilityContainer.SetActive(false);
    }

    public void RefreshWorkerAssignmentText()
    {
        if (WorkerAssignmentContainer != null && WorkerAssignmentContainer.activeSelf)
            openWorkerAssignment();
    }

}
