using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerMovement : MonoBehaviour
{
    PathFinding pathFinding;
    WorkerMind workerMind;

    bool closeEnough;
    float minimumDistance = 1.5f;

    public Vector3 nextLocation;
    private Vector3 initialPosition;

    public GameObject homeDesk;

    public float step = 1;

    private void Awake()
    {
        pathFinding = GetComponent<PathFinding>();
        workerMind = GetComponent<WorkerMind>();
    }

    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Advance along the path
    public void Advance()
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
        if (pathFinding.GetWaypoints().Count > 0)
        {
            if (Mathf.Abs(transform.position.x - pathFinding.GetWaypoints()[0].x) < 0.01 && Mathf.Abs(transform.position.z - pathFinding.GetWaypoints()[0].z) < 0.01)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsAtCoffeeMachine()
    {
        if (Vector3.Distance(transform.position, GridManager.Instance.GetCoffeeTarget().position) < minimumDistance)
        {
            workerMind.hasCoffee = true;
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
        return workerMind.hasCoffee;
    }
}
