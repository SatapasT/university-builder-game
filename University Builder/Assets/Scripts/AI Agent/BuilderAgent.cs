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

    private enum Mode { Idle, Build, Gather }
    private Mode mode = Mode.Idle;

    private NavMeshAgent agent;

    private Transform idlePoint;

    private Transform target;
    private BuildingConstruction construction;

    private float repathTimer = 0f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("BuilderAgent: NavMeshAgent missing.");
            enabled = false;
            return;
        }

        if (overrideAgentSettings)
            ApplySnappySettings();
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

    public void SetIdle(Transform idleTransform)
    {
        idlePoint = idleTransform;
        target = idleTransform;
        construction = null;
        mode = Mode.Idle;

        FixToNavMesh();
        ForceDestinationAsCloseAsPossible();
    }

    public void SetBuildTarget(Transform site, BuildingConstruction bc)
    {
        target = site;
        construction = bc;
        mode = Mode.Build;

        FixToNavMesh();
        ForceDestinationAsCloseAsPossible();
    }

    public void SetGatherTarget(Transform gatherTarget)
    {
        target = gatherTarget;
        construction = null;
        mode = Mode.Gather;

        FixToNavMesh();
        ForceDestinationAsCloseAsPossible();
    }

    private void Update()
    {
        if (agent == null || target == null)
            return;

        if (!agent.isOnNavMesh)
            FixToNavMesh();

        if (mode == Mode.Build && construction != null && construction.IsFinished)
        {
            SetIdle(idlePoint != null ? idlePoint : target);
            return;
        }

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            ForceDestinationAsCloseAsPossible();
        }

        float distToTarget = Vector3.Distance(transform.position, target.position);

        if (distToTarget <= arriveDistance)
        {
            agent.isStopped = true;
        }
        else if (distToTarget > maxWorkDistance)
        {
            agent.isStopped = false;
        }
    }

    private void FixToNavMesh()
    {
        if (agent.isOnNavMesh)
            return;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, spawnSampleRadius, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
    }

    private void ForceDestinationAsCloseAsPossible()
    {
        if (target == null)
            return;

        if (!NavMesh.SamplePosition(target.position, out NavMeshHit nearTarget, targetSampleRadius, NavMesh.AllAreas))
            return;

        agent.isStopped = false;
        agent.SetDestination(nearTarget.position);
    }
}
