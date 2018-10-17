using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerMovement : MonoBehaviour
{
    PathFinding pathFinding;
    WorkerMind workerMind;

    public bool closeEnough;
    float minimumDistance = 1.75f;

    public Vector3 nextLocation;
    private Vector3 initialPosition;
    Vector3[] previousPositions;
    int stuckCounter;

    public GameObject homeDesk;

    Animator animator;

    float step = 1;

    private void Awake()
    {
        pathFinding = GetComponent<PathFinding>();
        workerMind = GetComponent<WorkerMind>();
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;
        previousPositions = new Vector3[5];
        for (int i = 0; i < 5; i++)
        {
            previousPositions[i] = transform.position;
        }

        if (animator)
        {
            animator.Play("Movement");
        }

        step = Time.timeScale / 10.0f;
    }

    private void Update()
    {
        step = Time.timeScale / 10.0f;
    }

    // Advance along the path
    public void Advance()
    {
        if (pathFinding.GetWaypoints() != null)
        {
            float moveSpeed = 0.0f;

            // Stop moving when close enough
            if (pathFinding.GetWaypoints().Count > 0 && Vector3.Distance(transform.position, pathFinding.GetWaypoints()[pathFinding.GetWaypoints().Count - 1]) < minimumDistance)
            {
                closeEnough = true;
                moveSpeed = 0.0f;
            }
            else
            {
                closeEnough = false;
                moveSpeed = 0.5f;
            }

            if (animator)
            {
                animator.SetFloat("MoveSpeed", moveSpeed);
            }

            if (IsAtWaypoint())
            {
                // Remove previous waypoint
                pathFinding.RemoveWaypoint();
            }

            // Move towards the next location in the path
            if (pathFinding.GetWaypoints().Count > 0 && !closeEnough)
            {
                Vector3 lTargetDir = nextLocation - transform.position;

                AvoidWorker(lTargetDir);

                // Set target at ground level
                lTargetDir.y = 0.0f;

                if (GridManager.isInitialized && pathFinding.GetWaypoints().Count > 0)
                {
                    var wayPts = pathFinding.GetWaypoints();

                    if (wayPts.Count > 0)
                    {
                        nextLocation = wayPts[0];
                        nextLocation.y = initialPosition.y;
                    }
                }

                // Prevent console output stating the obvious
                if (lTargetDir != Vector3.zero)
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), Time.time * 0.6f);
                }

                // Refresh the unit path if the unit is stuck
                if (IsStuck())
                {
                    // print(gameObject.name + "is stuck");
                    pathFinding.RefreshTarget();
                }

                transform.position = Vector3.MoveTowards(transform.position, nextLocation, step);
            }
        }
    }

    // Check if a unit hasn't moved in the last 5 frames
    bool IsStuck()
    {
        if (IsAtDesk())
        {
            return false;
        }

        //Store the newest vector at the end of the list of vectors
        for (int i = 0; i < previousPositions.Length - 1; i++)
        {
            previousPositions[i] = previousPositions[i + 1];
        }

        previousPositions[previousPositions.Length - 1] = transform.position;

        if (Vector3.Distance(previousPositions[0], previousPositions[4]) < 0.1f)
        {
            return true;
        }

        return false;
    }

    //Check if this worker is colliding with another worker and avoid if collision
    void AvoidWorker(Vector3 direction)
    {
        Vector3 rayPosition = transform.position;
        rayPosition.y = 1.0f;

        // Debug.DrawRay(rayPosition + Vector3.Normalize(lTargetDir), Vector3.Normalize(lTargetDir) * 2.0f, Color.red, 0.1f);

        RaycastHit hit;
        //Check if this worker is colliding with another worker
        if (Physics.Raycast(rayPosition, Vector3.Normalize(direction), out hit, 1.0f))
        {
            if (hit.transform.gameObject.tag == "Worker")
            {
                // Path occupied by another worker
                Vector3 avoidanceDirection = Vector3.Cross(Vector3.Normalize(direction), Vector3.up);

                // Check if it is possible to move left or right
                if (Physics.Raycast(rayPosition, avoidanceDirection, out hit, 0.8f))
                {
                    if (hit.transform.tag == "Wall")
                    {
                        avoidanceDirection = -avoidanceDirection;
                    }
                    else if (Physics.Raycast(rayPosition, avoidanceDirection, out hit, 0.8f))
                    {
                        if (hit.transform.tag == "Wall")
                        {
                            // Move backwards if left and right are occupied
                            avoidanceDirection = -Vector3.Normalize(direction);
                        }
                    }
                }

                // Create a new waypoint not occupied
                pathFinding.AddWaypoint(transform.position + avoidanceDirection * 0.5f);
            }
        }
    }

    // Has the agent has arrived at the next waypoint?
    bool IsAtWaypoint()
    {
        if (pathFinding.GetWaypoints().Count > 0)
        {
            if (Mathf.Abs(transform.position.x - pathFinding.GetWaypoints()[0].x) < 0.01 && Mathf.Abs(transform.position.z - pathFinding.GetWaypoints()[0].z) < 0.01)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsAtDesk()
    {
        var dist = Vector3.Distance(transform.position, homeDesk.transform.position);

        if (dist < 2.0f)
        {
            return true;
        }

        return false;
    }

    public bool IsAtCoffeeMachine()
    {
        if (Vector3.Distance(transform.position, GridManager.Instance.GetCoffeeTarget()) < 2.0f)
        {
            return true;
        }

        return false;
    }

    public bool IsAtSnackMachine()
    {
        if (Vector3.Distance(transform.position, GridManager.Instance.GetSnackPosition()) < 2.2f)
        {
            return true;
        }

        return false;
    }

    public bool IsAtToilet()
    {
        if (Vector3.Distance(transform.position, GridManager.Instance.GetCell(GridManager.Instance.toiletCoord).transform.position) < 2.0f)
        {
            return true;
        }

        return false;
    }

    public bool IsAtRestaurant()
    {
        if (Vector3.Distance(transform.position, GridManager.Instance.GetCell(GridManager.Instance.GetRestaurantCoordinate()).transform.position) < 2.0f)
        {
            return true;
        }

        return false;
    }

    public bool IsAtSink()
    {
        return IsAtLocation(GridManager.Instance.GetCell(GridManager.Instance.sinkCoord).transform.position);
    }

    bool IsAtLocation(Vector3 location)
    {
        if (Vector3.Distance(transform.position, location) < 2.0f)
        {
            return true;
        }

        return false;
    }
}
