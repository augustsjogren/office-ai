using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinding : MonoBehaviour
{
    public Transform seeker, target;
    GridManager grid;

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
    }

    private void Start()
    {
        seeker = transform;
        target = GridManager.GetTarget();
    }

    void Update()
    {
        if(target != null && GridManager.isInitialized)
        {
            FindPath(seeker.position, target.position);
        }   
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // Get start and target node

        Coordinate startCoord = GridManager.GetCoordinate(transform.position);
        Node startNode = new Node(true, transform.position, startCoord.x, startCoord.y);

        Coordinate goalCoord = GridManager.GetGoalCoordinate();
        Coordinate coffeeCoord = GridManager.Instance.GetCoffeeLocation();

        Node targetNode;

        switch (GetComponent<WorkerMind>().state)
        {
            case 0:
                // Do nothing special
                targetNode = new Node(true, GridManager.GetTarget().position, goalCoord.x, goalCoord.y);
                break;

            case 1:
                // Get a coffee
                targetNode = new Node(true, GridManager.Instance.GetCoffeeTarget().position, coffeeCoord.x, coffeeCoord.y);
                break;

            default:
                targetNode = new Node(true, transform.position, 0, 0);
                break;
        }

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node.gridX == targetNode.gridX && node.gridY == targetNode.gridY)
            {
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

    public List<Node> GetPath() {
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