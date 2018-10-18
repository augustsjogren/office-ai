using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance = null;

    public Material goalMat;
    public Material pathMat;
    public Material obstacleMat;
    public Material celllMat;

    public GameObject ground;
    public GameObject[] desks;
    public GameObject cell;
    private static GameObject[,] cells;
    private static GameObject goal;

    private static Coordinate goalCoord;
    private Coordinate coffeeCoord;
    public Coordinate teaCoord;
    private Coordinate snackCoord;
    private Coordinate restaurantCoord;
    public Coordinate toiletCoord;
    public Coordinate sinkCoord;
    
    public List<Coordinate> coffeeCoords;

    private Node[,] grid;

    public int rows;
    public int cols;
    public float cellSize = 1.0f;
    public float spacing = 0.01f;

    public static bool isInitialized = false;

    public List<Node> path;

    private void Awake()
    {
        goal = cell;
        cells = new GameObject[rows, cols];
        grid = new Node[rows, cols];
    }

    // Use this for initialization
    void Start()
    {
        //Check if instance already exists
        if (Instance == null)
            Instance = this;

        //If instance already exists and it's not this:
        else if (Instance != this)
            Destroy(gameObject);

        desks = GameObject.FindGameObjectsWithTag("Desk");
        coffeeCoords = new List<Coordinate>();

        // The grid will always have the same size as the plane in the scene
        cellSize = ground.GetComponent<Renderer>().bounds.size.x / (cols);

        goalCoord = new Coordinate(6, 38);
        coffeeCoord = new Coordinate(1, 1);
        restaurantCoord = new Coordinate(1, 1);
        snackCoord = new Coordinate(1, 1);
        teaCoord = new Coordinate(1, 1);
        toiletCoord = new Coordinate(1,1);
        sinkCoord = new Coordinate(1, 1);

        cell.transform.localScale = new Vector3(cellSize, cell.transform.localScale.y, cellSize);

        CreateGrid();

        goal = cells[goalCoord.x, goalCoord.y];
        goal.gameObject.GetComponent<Renderer>().material = goalMat;

        cell.gameObject.SetActive(false);

        isInitialized = true;

        InitializeAllCells();
    }

    public void CreateGrid()
    {
        float positionX = -cellSize * rows / 2 - spacing + cellSize / 2;
        float positionZ = -cellSize * cols / 2 - spacing + cellSize / 2;

        Vector3 currentPosition = new Vector3();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                currentPosition.x = positionX;
                currentPosition.y = cell.transform.position.y;
                currentPosition.z = positionZ;

                GameObject newCell = Instantiate(cell);
                newCell.transform.SetParent(transform);
                newCell.transform.position = currentPosition;
                newCell.gameObject.name = "Cell [" + r + "," + c + "]";
                cells[r, c] = newCell;

                var cellComp = cells[r, c].GetComponent<Cell>();
                cellComp.x = r;
                cellComp.y = c;

                // Create nodes for A*
                grid[r, c] = new Node(true, newCell.transform.position, r, c);
                cellComp.walkable = true;

                positionZ += (cellSize + spacing);
            }

            positionX += (cellSize + spacing);
            positionZ = -cellSize * cols / 2 - spacing + cellSize / 2;
        }
    }

    void InitializeAllCells()
    {
        foreach (var cell in cells)
        {
            cell.GetComponent<Cell>().InitCell();
        }
    }

    public Coordinate GetCoffeeLocation()
    {
        return coffeeCoord;
    }

    public void SetCoffeeLocation(int x, int y)
    {
        coffeeCoord = new Coordinate(x, y);
    }

    public Vector3 GetCoffeeTarget()
    {
        return GetCell(coffeeCoord.x, coffeeCoord.y).transform.position;
    }

    public Coordinate GetRestaurantCoordinate()
    {
        return restaurantCoord;
    }

    public void SetRestaurantLocation(int x, int y)
    {
        restaurantCoord = new Coordinate(x, y);
    }

    public Vector3 GetRestaurantPosition()
    {
        return GetCell(restaurantCoord.x, restaurantCoord.y).transform.position;
    }

    public Vector3 GetSnackPosition()
    {
        return GetCell(snackCoord.x, snackCoord.y).transform.position;
    }

    public Coordinate GetSnackCoordinate()
    {
        return snackCoord;
    }

    public void SetSnackLocation(int x, int y)
    {
        snackCoord = new Coordinate(x, y);
    }

    public static Transform GetTarget()
    {
        return goal.transform;
    }

    public void SetWalkable(int x, int y, bool walkable)
    {
        grid[x, y].walkable = walkable;

        if (!walkable)
        {
            if (x > 0)
            {
                grid[x - 1, y].walkable = walkable;
            }

            if (x < cols - 1)
            {
                grid[x + 1, y].walkable = walkable;
            }

            if (y > 0)
            {
                grid[x, y - 1].walkable = walkable;
            }

            if (y < rows - 1)
            {
                grid[x, y + 1].walkable = walkable;
            }
        }
    }

    // Get grid coordinate from world position
    public Coordinate GetCoordinate(Vector3 position)
    {
        float minDistance = 1000;
        int x = 0, y = 0;

        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (Vector3.Distance(position, cells[i, j].transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(position, cells[i, j].transform.position);
                    x = i;
                    y = j;
                }
            }
        }

        return new Coordinate(x, y);
    }

    public Cell GetCellFromPosition(Vector3 position)
    {
        var coord = GetCoordinate(position);
        Cell cell = GetCell(coord.x, coord.y).GetComponent<Cell>();
        return cell;
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < rows && checkY >= 0 && checkY < cols)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public void OccupyNode(int x, int y)
    {
        grid[x, y].isOccupied = true;
    }

    public void ClearNode(int x, int y)
    {
        grid[x, y].isOccupied = false;
    }

    public static Coordinate GetGoalCoordinate()
    {
        return goalCoord;
    }

    public GameObject GetCell(int x, int y)
    {
        return cells[x, y];
    }

        public GameObject GetCell(Coordinate coord)
    {
        return cells[coord.x, coord.y];
    }

    // Reset color for cells not longer part of the path
    public void ClearPath()
    {
        foreach (var cell in cells)
        {
            var cellComp = cell.GetComponent<Cell>();

            if (cellComp.isPath)
            {
                cellComp.isPath = false;
            }
        }
    }

    public void SetCellToPath(int x, int y)
    {
        cells[x, y].GetComponent<Cell>().isPath = true;
    }
}
