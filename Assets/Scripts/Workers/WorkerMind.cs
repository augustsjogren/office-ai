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
        animator.Play("Movement");
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
        if (path != null)
        {
            // Stop moving when close enough
            if (path.Count > 0 && Vector3.Distance(transform.position, path[path.Count - 1].worldPosition) < minimumDistance)
            {
                closeEnough = true;
                animator.SetFloat("MoveSpeed", 0.0f);
            }
            else
            {
                closeEnough = false;
                animator.SetFloat("MoveSpeed", 0.5f);
            }

            // Remove tiles already traversed
            if (HasMoved())
            {
                path.RemoveAt(0);
                //pathFinding.RefreshTarget();
            }

            if (GridManager.isInitialized && path.Count > 0)
            {
                var wayPts = pathFinding.GetWaypoints();

                if (wayPts.Count > 1)
                {
                    nextLocation = wayPts[1];
                    nextLocation.y = initialPosition.y;
                }               
            }

            // Move towards the next location in the path
            if (path.Count > 0 && !closeEnough && !path[0].isOccupied)
            {
                Vector3 lTargetDir = nextLocation - transform.position;
                lTargetDir.y = 0.0f;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), Time.time * 0.6f);

                transform.position = Vector3.MoveTowards(transform.position, nextLocation, step);
            }

        }
    }

    // Has the worker moved to a new tile?
    bool HasMoved()
    {
        Cell newCell = GridManager.Instance.GetCellFromPosition(transform.position);

        if (ReferenceEquals(currentCell, newCell))
        {
            // Still at the same cell
            return false;
        }

        // Occupy new cell and node and de-occupy previous cell and node
        newCell.isOccupied = true;
        GridManager.Instance.OccupyNode(newCell.x, newCell.y);

        currentCell.isOccupied = false;
        GridManager.Instance.ClearNode(currentCell.x, currentCell.y);

        newCell.UpdateCell();
        currentCell.UpdateCell();

        currentCell = newCell;

        return true;
    }

    public void Work()
    {
        state = 0;
        pathFinding.RefreshTarget();
        path = pathFinding.GetPath();
    }

    public void GetCoffee()
    {
        state = 1;
        pathFinding.RefreshTarget();
        path = pathFinding.GetPath();
    }

    public void GoToRestaurant()
    {
        state = 2;
        pathFinding.RefreshTarget();
        path = pathFinding.GetPath();
    }

    // Function for drawing the path (Debug)
    void DrawPath()
    {
        Vector3 from = transform.position;
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
                Gizmos.DrawCube(from, new Vector3(0.1f, 0.1f, 0.1f));

                from = wayPoint;
            }
        }
    }
}
