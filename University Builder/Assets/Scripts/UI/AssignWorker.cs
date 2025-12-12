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
    private readonly List<WorkerManager.WorkerTask> taskByOptionIndex = new();

    private int selectedWorkerId = -1;
    private WorkerManager.WorkerTask selectedTask = WorkerManager.WorkerTask.None;

    public bool HasValidSelection =>
        selectedWorkerId > 0 &&
        selectedTask.type != WorkerManager.WorkerTaskType.None;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        RefreshDropdowns();
        ClearSelectionUI();

        if (BuildProgressTracker.Instance != null)
            BuildProgressTracker.Instance.OnBuildStateChanged += OnBuildStateChanged;
    }

    private void OnDisable()
    {
        if (BuildProgressTracker.Instance != null)
            BuildProgressTracker.Instance.OnBuildStateChanged -= OnBuildStateChanged;

        // Match other menus: when leaving, clear + hide
        ClearSelectionUI();
    }

    private void OnBuildStateChanged(BuildType type, BuildProgressTracker.BuildState state)
    {
        // tasks list can change based on build state
        RefreshTaskDropdown();
        // keep info consistent with current dropdown values
        selectedWorkerId = GetSelectedWorkerId();
        selectedTask = GetSelectedTask();
        UpdateInfoText();
        WorkshopUI.Instance?.RenderConfirmButton();
    }

    // ---------------- PUBLIC API (WorkshopUI calls this when switching menus) ----------------

    public void ClearSelectionUI()
    {
        selectedWorkerId = -1;
        selectedTask = WorkerManager.WorkerTask.None;

        if (workerDropdown != null)
        {
            workerDropdown.SetValueWithoutNotify(0);
            workerDropdown.RefreshShownValue();
        }

        if (taskDropdown != null)
        {
            taskDropdown.SetValueWithoutNotify(0);
            taskDropdown.RefreshShownValue();
        }

        HideInfo();
        WorkshopUI.Instance?.RenderConfirmButton();
    }

    // ---------------- Dropdown events (hook these in Inspector) ----------------

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

    // ---------------- Confirm ----------------

    public void ConfirmAssign()
    {
        if (!CanConfirmAssign())
            return;

        WorkerManager.Instance.AssignWorker(selectedWorkerId, selectedTask);

        // IMPORTANT: do NOT ClearSelectionUI here, otherwise it looks like nothing changed.
        // Just refresh info based on current dropdown selection.
        UpdateInfoText();
        WorkshopUI.Instance?.RenderConfirmButton();
    }

    public bool CanConfirmAssign()
    {
        if (WorkerManager.Instance == null) return false;
        if (!HasValidSelection) return false;

        if (selectedTask.type == WorkerManager.WorkerTaskType.Build)
        {
            if (BuildProgressTracker.Instance == null) return false;
            if (BuildProgressTracker.Instance.GetState(selectedTask.buildType) != BuildProgressTracker.BuildState.InProgress)
                return false;
        }

        return true;
    }

    // ---------------- Dropdown building ----------------

    public void RefreshDropdowns()
    {
        RefreshWorkerDropdown();
        RefreshTaskDropdown();
    }

    private void RefreshWorkerDropdown()
    {
        if (workerDropdown == null) return;

        workerDropdown.ClearOptions();
        workerIdByOptionIndex.Clear();

        var options = new List<string>();

        int max = WorkerManager.Instance != null ? WorkerManager.Instance.GetMaxWorkers() : 0;

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
        taskByOptionIndex.Add(WorkerManager.WorkerTask.None);

        options.Add("Gather Wood");
        taskByOptionIndex.Add(WorkerManager.WorkerTask.GatherWood());

        options.Add("Gather Stone");
        taskByOptionIndex.Add(WorkerManager.WorkerTask.GatherStone());

        if (BuildProgressTracker.Instance != null)
        {
            foreach (BuildType bt in System.Enum.GetValues(typeof(BuildType)))
            {
                if (bt == BuildType.None) continue;

                var state = BuildProgressTracker.Instance.GetState(bt);
                if (state == BuildProgressTracker.BuildState.InProgress)
                {
                    options.Add($"Build: {bt}");
                    taskByOptionIndex.Add(WorkerManager.WorkerTask.Build(bt));
                }
            }
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

    private WorkerManager.WorkerTask GetSelectedTask()
    {
        if (taskDropdown == null) return WorkerManager.WorkerTask.None;
        int idx = taskDropdown.value;
        if (idx < 0 || idx >= taskByOptionIndex.Count) return WorkerManager.WorkerTask.None;
        return taskByOptionIndex[idx];
    }

    // ---------------- Info display (dropdown-driven only) ----------------

    private void HideInfo()
    {
        if (infoText == null) return;
        infoText.text = string.Empty;
        infoText.gameObject.SetActive(false);
    }

    private void UpdateInfoText()
    {
        if (infoText == null || WorkerManager.Instance == null)
            return;

        bool hasWorkerSelected = selectedWorkerId > 0;
        bool hasTaskSelected = selectedTask.type != WorkerManager.WorkerTaskType.None;

        // ✅ exactly what you asked: show/hide based ONLY on dropdown selections
        if (!hasWorkerSelected && !hasTaskSelected)
        {
            HideInfo();
            return;
        }

        infoText.gameObject.SetActive(true);

        var sb = new StringBuilder();

        sb.AppendLine("<b><color=orange>Selected Worker</color></b>");
        if (!hasWorkerSelected)
        {
            sb.AppendLine("None selected.");
        }
        else
        {
            var current = WorkerManager.Instance.GetWorkerFullTask(selectedWorkerId);
            sb.AppendLine($"Worker {selectedWorkerId} is assigned to: <color=yellow>{current.DisplayName()}</color>");
        }

        sb.AppendLine();

        sb.AppendLine("<b><color=orange>Selected Task</color></b>");
        if (!hasTaskSelected)
        {
            sb.AppendLine("None selected.");
            infoText.text = sb.ToString();
            return;
        }

        sb.AppendLine($"Task: <color=yellow>{selectedTask.DisplayName()}</color>");

        var workers = WorkerManager.Instance.GetWorkersAssignedTo(selectedTask);
        sb.AppendLine("Workers assigned:");
        if (workers.Count == 0) sb.AppendLine("- None");
        else
        {
            foreach (int w in workers)
                sb.AppendLine($"- Worker {w}");
        }

        infoText.text = sb.ToString();
    }
}