using UnityEngine;

public class TreeLOD : MonoBehaviour
{
    public Transform player;
    public float hideDistance = 150f;

    private Renderer rend;
    private Collider col;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        col = GetComponentInChildren<Collider>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        bool shouldShow = dist < hideDistance;

        if (rend != null)
            rend.enabled = shouldShow;

        if (col != null)
            col.enabled = shouldShow;
    }
}
