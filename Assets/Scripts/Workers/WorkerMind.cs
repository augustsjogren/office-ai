using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerMind : MonoBehaviour
{
    private Vector3 initialPosition;
    public Transform target;
    public Vector3 nextLocation;

    PathFinding pathFinding;
    GameObject currentCell;
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

    private void Awake()
    {
        pathFinding = GetComponent<PathFinding>();
    }

    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;
        path = new List<Node>();
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
            currentCell = GetCellBelow();
            gotPath = true;
        }

        if (IsAtDesk())
        {
            hasCoffee = false;
        }

        Advance();
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

    public bool HasCoffee()
    {
        return hasCoffee;
    }

    public bool isCloseEnough()
    {
        if (path.Count > 0 && Vector3.Distance(transform.position, path[path.Count - 1].worldPosition) < minimumDistance)
        {
            return true;
        }

        return closeEnough;
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

    // Advance along the path
    void Advance()
    {
        // Stop moving when close enough
        if (path.Count > 0 && Vector3.Distance(transform.position, path[path.Count - 1].worldPosition) < minimumDistance)
        {
            closeEnough = true;
        }
        else
        {
            closeEnough = false;
        }

        // Remove tiles already traversed
        if (HasMoved())
        {
            path.RemoveAt(0);
        }

        if (GridManager.isInitialized && path.Count > 0)
        {
            nextLocation = path[0].worldPosition;
            nextLocation.y = initialPosition.y;
        }

        // Move towards the next location in the path
        if (path.Count > 0 && !closeEnough)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextLocation, step);
        }
    }

    // Has the worker moved to a new tile? In that case remove the previous tile from the worker path
    bool HasMoved()
    {
        var newCell = GetCellBelow();

        if (ReferenceEquals(newCell, currentCell))
        {
            // Has not moved
            return false;
        }

        currentCell = newCell;
        return true;
    }

    // Get the object underneath the worker
    GameObject GetCellBelow()
    {
        RaycastHit hit;
        GameObject GO = null;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
        {
            GO = hit.transform.gameObject;
        }

        return GO;
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
        //Debug.Log(path[path.Count - 1].gridX + "," + path[path.Count - 1].gridY);
    }

    public void GoToRestaurant()
    {
        state = 2;
        pathFinding.RefreshTarget();
        path = pathFinding.GetPath();
    }
}
