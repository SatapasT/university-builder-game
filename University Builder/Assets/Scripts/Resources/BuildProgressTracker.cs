using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildProgressTracker : MonoBehaviour
{
    public static BuildProgressTracker Instance { get; private set; }

    public enum BuildState
    {
        NotBuilt = 0,
        InProgress = 1,
        Built = 2,
    }

    public event Action<BuildType, BuildState> OnBuildStateChanged;

    private readonly Dictionary<BuildType, BuildState> states = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public BuildState GetState(BuildType type)
    {
        if (type == BuildType.None)
            return BuildState.NotBuilt;

        return states.TryGetValue(type, out var state) ? state : BuildState.NotBuilt;
    }

    public bool IsBuilt(BuildType type) => GetState(type) == BuildState.Built;
    public bool IsInProgress(BuildType type) => GetState(type) == BuildState.InProgress;

    public void MarkInProgress(BuildType type)
    {
        if (type == BuildType.None) return;
        states[type] = BuildState.InProgress;
        OnBuildStateChanged?.Invoke(type, BuildState.InProgress);
    }

    public void MarkBuilt(BuildType type)
    {
        if (type == BuildType.None) return;
        states[type] = BuildState.Built;
        OnBuildStateChanged?.Invoke(type, BuildState.Built);
    }
}
