using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class PathFinding : MonoBehaviour
{
    public Transform seeker, target;
    GridManager grid;
    WorkerMind mind;
    WorkerMovement movement;

    Coordinate startCoord;
    Coordinate goalCoord;
    Coordinate coffeeCoord;
    Coordinate snackCoord;
    Coordinate deskCoord;
    Coordinate restaurantCoord;

    List<Node> path;
    List<Vector3> wayPoints;

    bool hasRetraced;

    public bool HasRetraced
    {
        get
        {
            return hasRetraced;
        }

        set
        {
            hasRetraced = value;
        }
    }

    void Awake()
    {
        grid = GameObject.Find("Grid").GetComponent<GridManager>();
        mind = GetComponent<WorkerMind>();
        movement = GetComponent<WorkerMovement>();
    }

    private void Start()
    {
        seeker = transform;
        target = GridManager.GetTarget();

        if (target != null && GridManager.isInitialized)
        {
            FindPath(seeker.position, target.position);
        }
    }

    public void RefreshTarget()
    {
        if (target != null && GridManager.isInitialized)
        {
            FindPath(seeker.position, target.position);
            mind.SetTarget(target);
        }
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        // Get start and target node
        startCoord = GridManager.Instance.GetCoordinate(transform.position);
        Node startNode = new Node(true, transform.position, startCoord.x, startCoord.y);

        goalCoord = GridManager.GetGoalCoordinate();
        coffeeCoord = GridManager.Instance.GetCoffeeLocation();
        snackCoord = GridManager.Instance.GetSnackCoordinate();

        deskCoord = GridManager.Instance.GetCoordinate(movement.homeDesk.transform.position);
        restaurantCoord = GridManager.Instance.GetRestaurantCoordinate();

        Node targetNode;

        switch (GetComponent<WorkerMind>().state)
        {
            case 0:
                // Work
                targetNode = new Node(true, movement.transform.position, deskCoord.x, deskCoord.y);
                break;

            case 1:
                // Get a coffee
                targetNode = new Node(true, GridManager.Instance.GetCoffeeTarget(), coffeeCoord.x, coffeeCoord.y);
                break;

            case 2:
                // Go to the lunch room
                targetNode = new Node(true, GridManager.Instance.GetRestaurantPosition(), restaurantCoord.x, restaurantCoord.y);
                break;

            case 3:
                // Go to the lunch room
                targetNode = new Node(true, GridManager.Instance.GetSnackPosition(), snackCoord.x, snackCoord.y);
                break;

            default: 
                targetNode = new Node(true, transform.position, 0, 0);
                break;
        }

        Heap<Node> openSet = new Heap<Node>(grid.rows * grid.cols);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node node = openSet.RemoveFirst();

            closedSet.Add(node);

            if (node.gridX == targetNode.gridX && node.gridY == targetNode.gridY)
            {
                sw.Stop();
                //print(sw.ElapsedMilliseconds + " ms");
                RetracePath(startNode, node);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(node))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour) || neighbour.isOccupied)
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode && currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;

        GridManager.Instance.ClearPath();
        foreach (var node in path)
        {
            GridManager.Instance.SetCellToPath(node.gridX, node.gridY);
        }

        PathToWaypoints();
        SmoothPath();

        hasRetraced = true;
    }

    // Smooth the a* path to be more natural, remove unneccesary waypoints if there is a line of sight
    void SmoothPath()
    {
        var checkPoint = transform.position;
        int pathIndex = 0;
        var currentPoint = wayPoints[pathIndex];

        while (wayPoints.Count > pathIndex + 1)
        {
            if (Walkable(checkPoint, wayPoints[pathIndex + 1]))
            {
                currentPoint = wayPoints[pathIndex + 1];
                wayPoints.RemoveAt(pathIndex);
                path.RemoveAt(pathIndex);
            }
            else
            {
                checkPoint = currentPoint;
                currentPoint = wayPoints[pathIndex + 1];
                pathIndex++;
            } 
        }
    }

    public void RemoveWaypoint()
    {
        wayPoints.RemoveAt(0);
    }

    // Convert path of nodes to waypoints (Vector3)
    List<Vector3> PathToWaypoints()
    {
        wayPoints = new List<Vector3>();

        if (path != null)
        {
            foreach (var node in path)
            {
                wayPoints.Add(node.worldPosition);
            }
        }

        return wayPoints;
    }

    public List<Vector3> GetWaypoints()
    {
        return wayPoints;
    }

    // Check if the straight line between a and b is walkable (no obstacles crossing the line)
    bool Walkable(Vector3 a, Vector3 b)
    {
        float granularity = GridManager.Instance.cellSize / 5.0f;
        float unitWidth = 1.0f;

        Vector3 direction = Vector3.Normalize(a - b);
        Vector3 perpendicularVector = Vector3.Normalize(Vector3.Cross(Vector3.up, direction));

        Vector3 samplePoint = a;

        while (Vector3.Distance(samplePoint, b) > granularity * 1.5)
        {
            RaycastHit hit;
            GameObject sampleCell;

            if (Physics.Raycast(samplePoint + Vector3.up * 5, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
            {
                if (hit.transform.gameObject.tag == "Cell")
                {
                    sampleCell = hit.transform.gameObject;
                }
                else
                {
                    // Hit sometheing that is not a cell
                    return false;
                }

                if (!sampleCell.GetComponent<Cell>().walkable)
                {
                    // Hit an unwalkable cell
                    return false;
                }
            }

            samplePoint = Vector3.MoveTowards(samplePoint, b, granularity);
        }

        return true;
    }

    public List<Node> GetPath()
    {
        return path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}