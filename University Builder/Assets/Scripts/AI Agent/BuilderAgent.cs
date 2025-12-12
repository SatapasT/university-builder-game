using UnityEngine;
using UnityEngine.AI;

public class BuilderAgent : MonoBehaviour
{
    [Header("Arrival / Work")]
    [SerializeField] private float arriveDistance = 1.5f;
    [SerializeField] private float maxWorkDistance = 4.0f;

    [Header("Movement Feel (snappy)")]
    [SerializeField] private bool overrideAgentSettings = true;
    [SerializeField] private float agentSpeed = 6.5f;
    [SerializeField] private float agentAcceleration = 40f;
    [SerializeField] private float agentAngularSpeed = 720f;
    [SerializeField] private float agentStoppingDistance = 0.1f;

    [Header("NavMesh Robustness")]
    [SerializeField] private float spawnSampleRadius = 8f;
    [SerializeField] private float targetSampleRadius = 12f;
    [SerializeField] private float repathInterval = 0.75f;

    [Header("Stuck Detection")]
    [SerializeField] private float stuckVelocity = 0.08f;
    [SerializeField] private float stuckTimeToAccept = 0.6f;
    [SerializeField] private float progressEpsilon = 0.05f;

    private enum State { GoingToSite, Working }
    private State state = State.GoingToSite;

    private NavMeshAgent agent;
    private Transform target;
    private BuildingConstruction construction;

    private float repathTimer = 0f;
    private float bestDistanceToTarget = float.MaxValue;
    private float stuckTimer = 0f;

    public void Initialize(Transform targetTransform, BuildingConstruction targetConstruction)
    {
        target = targetTransform;
        construction = targetConstruction;

        agent = GetComponent<NavMeshAgent>();
        if (agent == null || target == null || construction == null)
        {
            Debug.LogError("BuilderAgent: Initialize missing components/refs.");
            enabled = false;
            return;
        }

        if (overrideAgentSettings)
            ApplySnappySettings();

        FixSpawnToNavMesh();
        ForceDestinationAsCloseAsPossible();

        state = State.GoingToSite;
        repathTimer = 0f;

        bestDistanceToTarget = Vector3.Distance(transform.position, target.position);
        stuckTimer = 0f;
    }

    private void ApplySnappySettings()
    {
        agent.speed = agentSpeed;
        agent.acceleration = agentAcceleration;
        agent.angularSpeed = agentAngularSpeed;
        agent.stoppingDistance = agentStoppingDistance;

        agent.autoBraking = false;
        agent.autoRepath = true;
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    }

    private void Update()
    {
        if (agent == null || target == null || construction == null)
            return;

        if (construction.IsFinished)
        {
            Destroy(gameObject);
            return;
        }

        if (!agent.isOnNavMesh)
            FixSpawnToNavMesh();

        switch (state)
        {
            case State.GoingToSite:
                UpdateGoingToSite();
                break;
            case State.Working:
                UpdateWorking();
                break;
        }
    }

    private void UpdateGoingToSite()
    {
        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            ForceDestinationAsCloseAsPossible();
        }

        float distToTarget = Vector3.Distance(transform.position, target.position);

        if (distToTarget <= arriveDistance)
        {
            BeginWork();
            return;
        }

        if (distToTarget + progressEpsilon < bestDistanceToTarget)
        {
            bestDistanceToTarget = distToTarget;
            stuckTimer = 0f;
        }
        else
        {
            if (agent.velocity.magnitude <= stuckVelocity)
                stuckTimer += Time.deltaTime;
            else
                stuckTimer = 0f;
        }

        bool pathBad = agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid;

        if (pathBad && distToTarget <= maxWorkDistance)
        {
            BeginWork();
            return;
        }

        if (stuckTimer >= stuckTimeToAccept && distToTarget <= maxWorkDistance)
        {
            BeginWork();
            return;
        }
    }

    private void BeginWork()
    {
        state = State.Working;
        agent.isStopped = true;
    }

    private void UpdateWorking()
    {
        float distToTarget = Vector3.Distance(transform.position, target.position);

        if (distToTarget > maxWorkDistance)
        {
            agent.isStopped = false;
            state = State.GoingToSite;

            stuckTimer = 0f;
            bestDistanceToTarget = distToTarget;

            ForceDestinationAsCloseAsPossible();
            return;
        }
    }

    private void FixSpawnToNavMesh()
    {
        if (agent == null) return;

        if (agent.isOnNavMesh)
            return;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, spawnSampleRadius, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
        else
        {
            Debug.LogError("BuilderAgent: Spawn is too far from NavMesh. Move AI spawn point closer to the baked NavMesh.");
        }
    }

    private void ForceDestinationAsCloseAsPossible()
    {
        if (agent == null || target == null) return;

        if (!NavMesh.SamplePosition(target.position, out NavMeshHit nearTarget, targetSampleRadius, NavMesh.AllAreas))
        {
            Vector3 fallback = transform.position + (target.position - transform.position).normalized * 4f;
            if (NavMesh.SamplePosition(fallback, out NavMeshHit nearFallback, targetSampleRadius, NavMesh.AllAreas))
                agent.SetDestination(nearFallback.position);

            return;
        }

        agent.isStopped = false;
        agent.SetDestination(nearTarget.position);
    }
}
