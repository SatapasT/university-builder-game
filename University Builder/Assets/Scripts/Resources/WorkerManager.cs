using System.Collections.Generic;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public static WorkerManager Instance { get; private set; }

    public enum WorkerTaskType
    {
        None = 0,
        Build = 1,
        GatherWood = 2,
        GatherStone = 3,
    }

    public struct WorkerTask
    {
        public WorkerTaskType type;
        public BuildType buildType;

        public static WorkerTask None => new WorkerTask { type = WorkerTaskType.None, buildType = BuildType.None };
        public static WorkerTask Build(BuildType bt) => new WorkerTask { type = WorkerTaskType.Build, buildType = bt };
        public static WorkerTask GatherWood() => new WorkerTask { type = WorkerTaskType.GatherWood, buildType = BuildType.None };
        public static WorkerTask GatherStone() => new WorkerTask { type = WorkerTaskType.GatherStone, buildType = BuildType.None };

        public string DisplayName()
        {
            return type switch
            {
                WorkerTaskType.None => "Idle",
                WorkerTaskType.Build => buildType.ToString(),
                WorkerTaskType.GatherWood => "Gather Wood",
                WorkerTaskType.GatherStone => "Gather Stone",
                _ => "Unknown"
            };
        }

        public string Key()
        {
            return type == WorkerTaskType.Build ? $"BUILD:{buildType}" : type.ToString();
        }
    }

    [Header("Starting workers")]
    [SerializeField] private int startingWorkers = 3;

    [Header("Workers gained per building completed")]
    [SerializeField] private int workersPerBuildingCompleted = 1;

    [Header("Worker Agent Spawning")]
    [SerializeField] private GameObject builderPrefab;
    [SerializeField] private Transform builderSpawnPoint;

    [Header("Gather targets")]
    [SerializeField] private string treeTag = "Tree";
    [SerializeField] private string rockTag = "Rock";

    private int maxWorkers;

    private readonly Dictionary<int, WorkerTask> workerToTask = new();
    private readonly Dictionary<int, BuilderAgent> workerToAgent = new();
    private readonly Dictionary<int, float> gatherTimer = new();

    private readonly HashSet<BuildType> appliedBuilderUnlocks = new();
    private readonly HashSet<BuildType> rewardedCompletionWorkers = new();

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
            workerToTask[i] = WorkerTask.None;
            gatherTimer[i] = 0f;
        }

        SpawnMissingAgents();
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

    private void Update()
    {
        if (ResourcesManager.Instance == null) return;

        foreach (var kvp in workerToTask)
        {
            int workerId = kvp.Key;
            WorkerTask task = kvp.Value;

            if (task.type != WorkerTaskType.GatherWood && task.type != WorkerTaskType.GatherStone)
                continue;

            gatherTimer[workerId] += Time.deltaTime;

            while (gatherTimer[workerId] >= 1f)
            {
                gatherTimer[workerId] -= 1f;

                if (task.type == WorkerTaskType.GatherWood)
                    ResourcesManager.Instance.AddResource(ResourceType.Wood, 1);
                else if (task.type == WorkerTaskType.GatherStone)
                    ResourcesManager.Instance.AddResource(ResourceType.Stone, 1);
            }
        }
    }

    private void HandleBuildStateChanged(BuildType type, BuildProgressTracker.BuildState state)
    {
        if (state != BuildProgressTracker.BuildState.Built)
            return;

        if (!rewardedCompletionWorkers.Contains(type))
        {
            rewardedCompletionWorkers.Add(type);

            if (workersPerBuildingCompleted > 0)
                AddWorkerSlots(workersPerBuildingCompleted);
        }

        var info = BuildDatabase.Get(type);
        if (info != null && info.UnlocksBuilderSlots > 0 && !appliedBuilderUnlocks.Contains(type))
        {
            appliedBuilderUnlocks.Add(type);
            AddWorkerSlots(info.UnlocksBuilderSlots);
        }

        foreach (var kvp in new Dictionary<int, WorkerTask>(workerToTask))
        {
            if (kvp.Value.type == WorkerTaskType.Build && kvp.Value.buildType == type)
                UnassignWorker(kvp.Key);
        }
    }

    private void AddWorkerSlots(int amount)
    {
        if (amount <= 0) return;

        int oldMax = maxWorkers;
        maxWorkers += amount;

        for (int i = oldMax + 1; i <= maxWorkers; i++)
        {
            workerToTask[i] = WorkerTask.None;
            gatherTimer[i] = 0f;
        }

        SpawnMissingAgents();
    }

    public int GetMaxWorkers() => maxWorkers;

    public WorkerTask GetWorkerFullTask(int workerId)
    {
        return workerToTask.TryGetValue(workerId, out var task) ? task : WorkerTask.None;
    }

    public void AssignWorkerToBuild(int workerId, BuildType buildType) => AssignWorker(workerId, WorkerTask.Build(buildType));
    public void AssignWorkerToGatherWood(int workerId) => AssignWorker(workerId, WorkerTask.GatherWood());
    public void AssignWorkerToGatherStone(int workerId) => AssignWorker(workerId, WorkerTask.GatherStone());
    public void UnassignWorker(int workerId) => AssignWorker(workerId, WorkerTask.None);

    public void AssignWorker(int workerId, WorkerTask task)
    {
        if (!workerToTask.ContainsKey(workerId))
            return;

        if (task.type == WorkerTaskType.Build && BuildProgressTracker.Instance != null)
        {
            if (BuildProgressTracker.Instance.GetState(task.buildType) != BuildProgressTracker.BuildState.InProgress)
                return;
        }

        workerToTask[workerId] = task;
        gatherTimer[workerId] = 0f;

        SpawnMissingAgents();

        if (!workerToAgent.TryGetValue(workerId, out var agent) || agent == null)
            return;

        if (task.type == WorkerTaskType.None)
        {
            agent.SetIdle(builderSpawnPoint);
            return;
        }

        if (task.type == WorkerTaskType.Build)
        {
            if (SelectedBuildTracker.Instance != null &&
                SelectedBuildTracker.Instance.TryGetSiteAndConstruction(task.buildType, out var site, out var construction))
            {
                agent.SetBuildTarget(site, construction);
            }
            return;
        }

        if (task.type == WorkerTaskType.GatherWood)
        {
            Transform t = FindClosestWithTag(treeTag, agent.transform.position);
            agent.SetGatherTarget(t != null ? t : builderSpawnPoint);
            return;
        }

        if (task.type == WorkerTaskType.GatherStone)
        {
            Transform tag = FindClosestWithTag(rockTag, agent.transform.position);
            agent.SetGatherTarget(tag != null ? tag : builderSpawnPoint);
            return;
        }
    }

    private void SpawnMissingAgents()
    {
        if (builderPrefab == null || builderSpawnPoint == null)
        {
            Debug.LogWarning("WorkerManager: builderPrefab or builderSpawnPoint not assigned.");
            return;
        }

        for (int i = 1; i <= maxWorkers; i++)
        {
            if (workerToAgent.TryGetValue(i, out var existing) && existing != null)
                continue;

            GameObject go = Instantiate(builderPrefab, builderSpawnPoint.position, builderSpawnPoint.rotation);
            BuilderAgent agent = go.GetComponent<BuilderAgent>();
            if (agent == null)
            {
                Debug.LogError("WorkerManager: BuilderAgent missing on builderPrefab.");
                Destroy(go);
                continue;
            }

            workerToAgent[i] = agent;
            agent.SetIdle(builderSpawnPoint);
        }
    }

    private Transform FindClosestWithTag(string tag, Vector3 fromPos)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return null;

        GameObject[] objs = GameObject.FindGameObjectsWithTag(tag);
        if (objs == null || objs.Length == 0)
            return null;

        Transform best = null;
        float bestDist = float.MaxValue;

        foreach (var obj in objs)
        {
            if (obj == null) continue;
            float d = Vector3.Distance(fromPos, obj.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = obj.transform;
            }
        }

        return best;
    }

    public void SetWorkerCount(int newCount)
    {
        newCount = Mathf.Max(1, newCount);

        if (newCount == maxWorkers)
            return;

        if (newCount < maxWorkers)
        {
            for (int id = newCount + 1; id <= maxWorkers; id++)
            {
                workerToTask.Remove(id);
                gatherTimer.Remove(id);

                if (workerToAgent.TryGetValue(id, out var agent) && agent != null)
                    Destroy(agent.gameObject);

                workerToAgent.Remove(id);
            }

            maxWorkers = newCount;

            AssignWorkerUI.Instance?.RefreshDropdowns();
            PlayerMenu.Instance?.RefreshWorkerAssignmentText();
            return;
        }

        int oldMax = maxWorkers;
        maxWorkers = newCount;

        for (int id = oldMax + 1; id <= maxWorkers; id++)
        {
            workerToTask[id] = WorkerTask.None;
            gatherTimer[id] = 0f;
        }

        SpawnMissingAgents();

        AssignWorkerUI.Instance?.RefreshDropdowns();
        PlayerMenu.Instance?.RefreshWorkerAssignmentText();
    }

    public int GetAssignedCount(BuildType buildType)
    {
        int count = 0;

        foreach (var kvp in workerToTask)
        {
            if (kvp.Value.type == WorkerTaskType.Build &&
                kvp.Value.buildType == buildType)
            {
                count++;
            }
        }

        return count;
    }
    public List<int> GetWorkersAssignedTo(WorkerTask task)
    {
        List<int> result = new List<int>();
        string key = task.Key();

        foreach (var kvp in workerToTask)
        {
            if (kvp.Value.Key() == key)
                result.Add(kvp.Key);
        }

        return result;
    }

}
