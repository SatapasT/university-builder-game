using UnityEngine;
using UnityEngine.AI;

public class BuilderAgent : MonoBehaviour
{
    [SerializeField] private float workRadius = 10f;         
    [SerializeField] private float arrivalThreshold = 0.3f; 
    [SerializeField] private float movePause = 0.3f;       

    private enum BuilderState { GoingToSite, Working }
    private BuilderState state = BuilderState.GoingToSite;

    private NavMeshAgent navAgent;
    private Transform targetTransform;
    private BuildingConstruction targetConstruction;
    private float pauseTimer = 0f;

    public void Initialize(Transform target, BuildingConstruction construction)
    {
        targetTransform = target;
        targetConstruction = construction;

        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent == null || targetTransform == null || targetConstruction == null)
        {
            Debug.LogError("BuilderAgent: Initialize called with missing components.");
            enabled = false;
            return;
        }

        navAgent.stoppingDistance = 0f;
        navAgent.autoBraking = false;
        navAgent.isStopped = false;

        navAgent.SetDestination(targetTransform.position);
        state = BuilderState.GoingToSite;
    }

    private void Update()
    {
        if (navAgent == null || targetConstruction == null)
            return;

        if (targetConstruction.IsFinished)
        {
            Destroy(gameObject);
            return;
        }

        switch (state)
        {
            case BuilderState.GoingToSite:
                UpdateGoingToSite();
                break;

            case BuilderState.Working:
                UpdateWorking();
                break;
        }
    }

    private void UpdateGoingToSite()
    {
        if (navAgent.pathPending)
            return;

        if (navAgent.remainingDistance <= arrivalThreshold)
        {
            state = BuilderState.Working;
            pauseTimer = 0f;
            SetNewWorkDestination();
        }
    }

    private void UpdateWorking()
    {
        if (navAgent.pathPending)
            return;

        if (pauseTimer > 0f)
        {
            pauseTimer -= Time.deltaTime;
            return;
        }

        if (navAgent.remainingDistance <= arrivalThreshold)
        {
            SetNewWorkDestination();
        }
    }

    private void SetNewWorkDestination()
    {
        if (targetTransform == null)
            return;

        pauseTimer = movePause;

        Vector3 randomDirection = Random.insideUnitSphere * workRadius;
        randomDirection.y = 0f;

        Vector3 rawDestination = targetTransform.position + randomDirection;

        if (NavMesh.SamplePosition(rawDestination, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
        {
            navAgent.SetDestination(hit.position);
        }
    }
}
