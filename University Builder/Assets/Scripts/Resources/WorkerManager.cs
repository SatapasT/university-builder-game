using System.Collections.Generic;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public static WorkerManager Instance { get; private set; }

    [Header("Starting workers (capacity)")]
    [SerializeField] private int startingWorkers = 1;

    [Header("Builder Spawning")]
    [SerializeField] private GameObject builderPrefab;
    [SerializeField] private Transform builderSpawnPoint;

    private int maxWorkers;

    private readonly Dictionary<int, BuildType> workerToTask = new();

    private readonly Dictionary<int, GameObject> workerToBuilder = new();

    private readonly HashSet<BuildType> appliedBuilderUnlocks = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        maxWorkers = Mathf.Max(1, startingWorkers);
        for (int i = 1; i <= maxWorkers; i++)
        {
            workerToTask[i] = BuildType.None;
            workerToBuilder[i] = null;
        }
    }

    private void OnEnable()
    {
        if (BuildProgressTracker.Instance != null)
            BuildProgressTracker.Instance.OnBuildStateChanged += HandleBuildStateChanged;
    }

    private void OnDisable()
    {
        if (BuildProgressTracker.Instance != null)
            BuildProgressTracker.Instance.OnBuildStateChanged -= HandleBuildStateChanged;
    }

    private void HandleBuildStateChanged(BuildType type, BuildProgressTracker.BuildState state)
    {
        if (state == BuildProgressTracker.BuildState.Built)
        {
            var info = BuildDatabase.Get(type);
            if (info != null && info.UnlocksBuilderSlots > 0 && !appliedBuilderUnlocks.Contains(type))
            {
                appliedBuilderUnlocks.Add(type);
                AddWorkerSlots(info.UnlocksBuilderSlots);
            }

            foreach (var kvp in new Dictionary<int, BuildType>(workerToTask))
            {
                if (kvp.Value == type)
                    UnassignWorker(kvp.Key);
            }
        }
    }

    private void AddWorkerSlots(int amount)
    {
        if (amount <= 0) return;

        int oldMax = maxWorkers;
        maxWorkers += amount;

        for (int i = oldMax + 1; i <= maxWorkers; i++)
        {
            workerToTask[i] = BuildType.None;
            workerToBuilder[i] = null;
        }
    }

    public int GetMaxWorkers() => maxWorkers;
    public string GetWorkerName(int workerId) => $"Worker {workerId}";

    public BuildType GetWorkerTask(int workerId)
    {
        if (!workerToTask.TryGetValue(workerId, out var t))
            return BuildType.None;
        return t;
    }

    public void AssignWorker(int workerId, BuildType task)
    {
        if (!workerToTask.ContainsKey(workerId))
            return;

        if (task != BuildType.None && BuildProgressTracker.Instance != null)
        {
            if (BuildProgressTracker.Instance.GetState(task) != BuildProgressTracker.BuildState.InProgress)
                return;
        }

        workerToTask[workerId] = task;

        DestroyWorkerBuilder(workerId);

        if (task != BuildType.None)
        {
            SpawnWorkerBuilder(workerId, task);
        }
    }

    public void UnassignWorker(int workerId)
    {
        if (!workerToTask.ContainsKey(workerId))
            return;

        workerToTask[workerId] = BuildType.None;
        DestroyWorkerBuilder(workerId);
    }

    private void DestroyWorkerBuilder(int workerId)
    {
        if (workerToBuilder.TryGetValue(workerId, out var existing) && existing != null)
        {
            Destroy(existing);
        }
        workerToBuilder[workerId] = null;
    }

    private void SpawnWorkerBuilder(int workerId, BuildType task)
    {
        if (builderPrefab == null || builderSpawnPoint == null)
        {
            Debug.LogWarning("WorkerManager: builderPrefab or builderSpawnPoint not assigned.");
            return;
        }

        if (SelectedBuildTracker.Instance == null)
        {
            Debug.LogWarning("WorkerManager: SelectedBuildTracker missing.");
            return;
        }

        if (!SelectedBuildTracker.Instance.TryGetSiteAndConstruction(task, out var site, out var construction))
        {
            Debug.LogWarning($"WorkerManager: Could not find site/construction for {task}.");
            return;
        }

        GameObject builderInstance = Instantiate(builderPrefab, builderSpawnPoint.position, builderSpawnPoint.rotation);
        workerToBuilder[workerId] = builderInstance;

        BuilderAgent agent = builderInstance.GetComponent<BuilderAgent>();
        if (agent == null)
        {
            Debug.LogError("WorkerManager: BuilderAgent missing on builderPrefab.");
            Destroy(builderInstance);
            workerToBuilder[workerId] = null;
            return;
        }

        agent.Initialize(site, construction);
    }

    public List<int> GetWorkersAssignedTo(BuildType task)
    {
        List<int> result = new();
        foreach (var kvp in workerToTask)
        {
            if (kvp.Value == task)
                result.Add(kvp.Key);
        }
        return result;
    }

    public int GetAssignedCount(BuildType task)
    {
        int count = 0;
        foreach (var kvp in workerToTask)
        {
            if (kvp.Value == task)
                count++;
        }
        return count;
    }
}
