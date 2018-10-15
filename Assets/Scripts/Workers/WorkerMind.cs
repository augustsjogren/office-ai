using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerMind : MonoBehaviour
{
    private Vector3 initialPosition;
    public Transform target;
    public Vector3 nextLocation;

    PathFinding pathFinding;
    Cell currentCell;
    public GameObject homeDesk;

    public float step = 1;
    bool gotPath;
    bool closeEnough;
    public bool hasCoffee;

    public bool shouldRefresh;

    float minimumDistance = 1.5f;

    // Variable for deciding what to do
    // 0 is idle
    public int state;

    public int hunger;

    List<Node> path;

    Animator animator;

    private void Awake()
    {
        pathFinding = GetComponent<PathFinding>();
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;
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
            Work();
            currentCell = GridManager.Instance.GetCellFromPosition(transform.position);
            pathFinding.RefreshTarget();
            gotPath = true;
        }

        if (IsAtDesk())
        {
            hasCoffee = false;
        }

        Advance();

    }

    private void OnDrawGizmosSelected()
    {
        // Only draw debug lines when in play mode
        if (Application.isPlaying)
        {
            DrawPath();
        }
    }

    public bool IsAtCoffeeMachine()
    {
        if (Vector3.Distance(transform.position, GridManager.Instance.GetCoffeeTarget().position) < minimumDistance)
        {
            hasCoffee = true;
            return true;
        }

        return false;
    }

    public bool IsAtDesk()
    {
        var dist = Vector3.Distance(transform.position, homeDesk.transform.position);

        if (dist < minimumDistance)
        {
            return true;
        }

        return false;
    }

    public bool HasCoffee()
    {
        return hasCoffee;
    }

    // Advance along the path
    void Advance()
    {
        if (pathFinding.GetWaypoints() != null)
        {
            // Stop moving when close enough
            if (pathFinding.GetWaypoints().Count > 0 && Vector3.Distance(transform.position, pathFinding.GetWaypoints()[pathFinding.GetWaypoints().Count - 1]) < minimumDistance)
            {
                closeEnough = true;
                //animator.SetFloat("MoveSpeed", 0.0f);
            }
            else
            {
                closeEnough = false;
                //animator.SetFloat("MoveSpeed", 0.5f);
            }

            if (IsAtWaypoint())
            {
                //shouldRefresh = true;

                // Remove previous waypoint
                pathFinding.RemoveWaypoint();
            }

            if (GridManager.isInitialized && pathFinding.GetWaypoints().Count > 0)
            {
                var wayPts = pathFinding.GetWaypoints();

                if (wayPts.Count > 0)
                {
                    nextLocation = wayPts[0];
                    nextLocation.y = initialPosition.y;
                }
            }

            // Move towards the next location in the path
            if (pathFinding.GetWaypoints().Count > 0 && !closeEnough)
            {
                Vector3 lTargetDir = nextLocation - transform.position;
                lTargetDir.y = 0.0f;

                // Prevent console output stating the obvious
                if (lTargetDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), Time.time * 0.6f);
                }

                transform.position = Vector3.MoveTowards(transform.position, nextLocation, step);
            }

        }
    }

    // Has the agent has arrived at the next waypoint?
    bool IsAtWaypoint()
    {
        if (transform.position.x == pathFinding.GetWaypoints()[0].x && transform.position.z == pathFinding.GetWaypoints()[0].z)
        {
            //print("At waypoint");
            return true;
        }

        return false;
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
                Gizmos.DrawCube(nextLocation, new Vector3(0.1f, 0.1f, 0.1f));

                from = wayPoint;
            }
        }
    }
}
