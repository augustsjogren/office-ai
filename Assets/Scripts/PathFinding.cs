using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class PathFinding : MonoBehaviour
{
    public Transform seeker, target;
    GridManager grid;
    WorkerMind mind;

    Coordinate startCoord;
    Coordinate goalCoord;
    Coordinate coffeeCoord;
    Coordinate deskCoord;

    List<Node> path;

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
        }
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // Get start and target node

        Stopwatch sw = new Stopwatch();
        sw.Start();

        startCoord = GridManager.GetCoordinate(transform.position);
        Node startNode = new Node(true, transform.position, startCoord.x, startCoord.y);

        goalCoord = GridManager.GetGoalCoordinate();
        coffeeCoord = GridManager.Instance.GetCoffeeLocation();
        deskCoord = GridManager.GetCoordinate(mind.homeDesk.transform.position);

        Node targetNode;

        switch (GetComponent<WorkerMind>().state)
        {
            case 0:
                // Work
                targetNode = new Node(true, mind.homeDesk.transform.position, deskCoord.x, deskCoord.y);
                break;

            case 1:
                // Get a coffee
                targetNode = new Node(true, GridManager.Instance.GetCoffeeTarget().position, coffeeCoord.x, coffeeCoord.y);
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
                if (!neighbour.walkable || closedSet.Contains(neighbour))
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

        hasRetraced = true;
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