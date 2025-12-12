using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class AssignWorkerUI : MonoBehaviour
{
    public static AssignWorkerUI Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TMP_Dropdown workerDropdown;
    [SerializeField] private TMP_Dropdown taskDropdown;
    [SerializeField] private TextMeshProUGUI infoText;

    private readonly List<int> workerIdByOptionIndex = new();
    private readonly List<BuildType> taskByOptionIndex = new();

    private int selectedWorkerId = -1;
    private BuildType selectedTask = BuildType.None;

    public bool HasValidSelection => selectedWorkerId > 0 && selectedTask != BuildType.None;

    [SerializeField] private float uiRefreshInterval = 0.2f;
    private float uiRefreshTimer = 0f;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        RefreshAll();

        if (BuildProgressTracker.Instance != null)
            BuildProgressTracker.Instance.OnBuildStateChanged += OnBuildStateChanged;
    }

    private void OnDisable()
    {
        if (BuildProgressTracker.Instance != null)
            BuildProgressTracker.Instance.OnBuildStateChanged -= OnBuildStateChanged;
    }

    private void Update()
    {
        uiRefreshTimer -= Time.deltaTime;
        if (uiRefreshTimer <= 0f)
        {
            uiRefreshTimer = uiRefreshInterval;
            UpdateInfoText();
        }
    }

    private void OnBuildStateChanged(BuildType type, BuildProgressTracker.BuildState state)
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        RefreshWorkerDropdown();
        RefreshTaskDropdown();

        selectedWorkerId = -1;
        selectedTask = BuildType.None;

        if (workerDropdown != null) workerDropdown.value = 0;
        if (taskDropdown != null) taskDropdown.value = 0;

        workerDropdown?.RefreshShownValue();
        taskDropdown?.RefreshShownValue();

        UpdateInfoText();
        WorkshopUI.Instance?.RenderConfirmButton();
    }

    public void OnWorkerDropdownChanged(int _)
    {
        selectedWorkerId = GetSelectedWorkerId();
        UpdateInfoText();
        WorkshopUI.Instance?.RenderConfirmButton();
    }

    public void OnTaskDropdownChanged(int _)
    {
        selectedTask = GetSelectedTask();
        UpdateInfoText();
        WorkshopUI.Instance?.RenderConfirmButton();
    }

    public void ConfirmAssign()
    {
        if (!CanConfirmAssign())
            return;

        WorkerManager.Instance.AssignWorker(selectedWorkerId, selectedTask);

        UpdateInfoText();
        WorkshopUI.Instance?.RenderConfirmButton();
    }

    public bool CanConfirmAssign()
    {
        if (WorkerManager.Instance == null) return false;
        if (!HasValidSelection) return false;

        if (BuildProgressTracker.Instance == null) return false;
        if (BuildProgressTracker.Instance.GetState(selectedTask) != BuildProgressTracker.BuildState.InProgress)
            return false;

        return true;
    }

    private void RefreshWorkerDropdown()
    {
        if (workerDropdown == null) return;

        workerDropdown.ClearOptions();
        workerIdByOptionIndex.Clear();

        var options = new List<string>();

        int max = WorkerManager.Instance != null ? WorkerManager.Instance.GetMaxWorkers() : 0;
        if (max <= 0)
        {
            options.Add("No Workers");
            workerIdByOptionIndex.Add(-1);
            workerDropdown.AddOptions(options);
            workerDropdown.interactable = false;
            return;
        }

        workerDropdown.interactable = true;

        options.Add("Select Worker");
        workerIdByOptionIndex.Add(-1);

        for (int i = 1; i <= max; i++)
        {
            options.Add($"Worker {i}");
            workerIdByOptionIndex.Add(i);
        }

        workerDropdown.AddOptions(options);
    }

    private void RefreshTaskDropdown()
    {
        if (taskDropdown == null) return;

        taskDropdown.ClearOptions();
        taskByOptionIndex.Clear();

        var options = new List<string>();

        options.Add("Select Task");
        taskByOptionIndex.Add(BuildType.None);

        bool anyInProgress = false;

        if (BuildProgressTracker.Instance != null)
        {
            foreach (BuildType bt in System.Enum.GetValues(typeof(BuildType)))
            {
                if (bt == BuildType.None) continue;

                var state = BuildProgressTracker.Instance.GetState(bt);

                if (state == BuildProgressTracker.BuildState.InProgress)
                {
                    anyInProgress = true;
                    options.Add(bt.ToString());
                    taskByOptionIndex.Add(bt);
                }
            }
        }

        if (!anyInProgress)
        {
            options.Add("No Active Construction");
            taskByOptionIndex.Add(BuildType.None);
        }

        taskDropdown.AddOptions(options);
    }

    private int GetSelectedWorkerId()
    {
        if (workerDropdown == null) return -1;
        int idx = workerDropdown.value;
        if (idx < 0 || idx >= workerIdByOptionIndex.Count) return -1;
        return workerIdByOptionIndex[idx];
    }

    private BuildType GetSelectedTask()
    {
        if (taskDropdown == null) return BuildType.None;
        int idx = taskDropdown.value;
        if (idx < 0 || idx >= taskByOptionIndex.Count) return BuildType.None;
        return taskByOptionIndex[idx];
    }

    private void UpdateInfoText()
    {
        if (infoText == null || WorkerManager.Instance == null)
            return;

        if (!infoText.gameObject.activeSelf)
            infoText.gameObject.SetActive(true);

        var sb = new StringBuilder();

        // ---------------- CURRENT WORKER ASSIGNMENT ----------------
        sb.AppendLine("<b><color=orange>Selected Worker</color></b>");
        if (selectedWorkerId <= 0)
        {
            sb.AppendLine("None selected.");
        }
        else
        {
            BuildType current = WorkerManager.Instance.GetWorkerTask(selectedWorkerId);
            sb.AppendLine(
                $"Worker {selectedWorkerId} is assigned to: " +
                $"<color=yellow>{(current == BuildType.None ? "Idle" : current.ToString())}</color>"
            );
        }

        sb.AppendLine();

        // ---------------- TASK ----------------
        sb.AppendLine("<b><color=orange>Selected Task</color></b>");
        if (selectedTask == BuildType.None)
        {
            sb.AppendLine("No task selected.");
            infoText.text = sb.ToString();
            return;
        }

        sb.AppendLine($"Task: <color=yellow>{selectedTask}</color>");

        var workers = WorkerManager.Instance.GetWorkersAssignedTo(selectedTask);
        sb.AppendLine("Workers assigned:");
        if (workers.Count == 0)
        {
            sb.AppendLine("- None");
        }
        else
        {
            foreach (int w in workers)
                sb.AppendLine($"- Worker {w}");
        }

        // ---------------- REMAINING TIME ----------------
        if (SelectedBuildTracker.Instance != null &&
            SelectedBuildTracker.Instance.TryGetSiteAndConstruction(selectedTask, out _, out var construction) &&
            construction != null && construction.IsBuilding && !construction.IsFinished)
        {
            float remaining = construction.GetRemainingSeconds();
            int workerCount = WorkerManager.Instance.GetAssignedCount(selectedTask);

            sb.AppendLine();
            sb.AppendLine("<b><color=orange>Construction</color></b>");
            sb.AppendLine($"Remaining Time: <color=yellow>{remaining:0.0}s</color>");
            sb.AppendLine($"Active Workers: <color=yellow>{workerCount}</color> (1 worker = 1s/s)");
        }

        infoText.text = sb.ToString();
    }
}
