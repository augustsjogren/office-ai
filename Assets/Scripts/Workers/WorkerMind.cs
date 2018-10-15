using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerMind : MonoBehaviour
{
    public Transform target;
    
    PathFinding pathFinding;
    WorkerMovement workerMovement;
    Cell currentCell;

    bool gotPath;
    
    public bool hasCoffee;

    public bool shouldRefresh;    

    // Variable for deciding what to do
    // 0 is idle
    public int state;

    public int hunger;

    List<Node> path;

    Animator animator;

    private void Awake()
    {
        pathFinding = GetComponent<PathFinding>();
        workerMovement = GetComponent<WorkerMovement>();
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start()
    {
       
        path = pathFinding.GetPath();
        //animator.Play("Movement");

        //shouldRefresh = true;
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    // Update is called once per frame
    void Update()
    {
        // Initialize the worker with working state
        if (GridManager.isInitialized && !gotPath)
        {
            currentCell = GridManager.Instance.GetCellFromPosition(transform.position);
            pathFinding.RefreshTarget();
            gotPath = true;
        }

        if (workerMovement.IsAtDesk())
        {
            hasCoffee = false;
        }

        workerMovement.Advance();
    }

    private void OnDrawGizmosSelected()
    {
        // Only draw debug lines when in play mode
        if (Application.isPlaying)
        {
            DrawPath();
        }
    }

    

    public void Work()
    {
        state = 0;
        RefreshPathIfNeeded();
    }

    public void GetCoffee()
    {
        state = 1;
        RefreshPathIfNeeded();
    }

    public void GoToRestaurant()
    {
        state = 2;
        RefreshPathIfNeeded();
    }

    // Only refresh the path if needed to avoid jittering and performance issues
    public void RefreshPathIfNeeded()
    {
        if (shouldRefresh)
        {
            pathFinding.RefreshTarget();
        }

        shouldRefresh = false;
    }

    // Function for drawing the path (Debug)
    void DrawPath()
    {
        Vector3 from = transform.position;
        //Vector3 from = pathFinding.GetWaypoints()[0];

        Vector3 to;

        if (pathFinding.GetWaypoints().Count > 0)
        {
            foreach (var wayPoint in pathFinding.GetWaypoints())
            {
                to = wayPoint;
                to.y = 0.5f;
                from.y = 0.5f;

                if (to != null)
                {
                    // Draws a blue line
                    Gizmos.color = Color.blue;
                    Gizmos.DrawLine(from, to);
                }

                // Mark the waypoints
                Gizmos.DrawCube(from, new Vector3(0.2f, 0.1f, 0.2f));

                Gizmos.color = Color.red;
                Gizmos.DrawCube(workerMovement.nextLocation, new Vector3(0.1f, 0.1f, 0.1f));

                from = wayPoint;
            }
        }
    }
}
