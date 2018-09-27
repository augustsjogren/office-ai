using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static Material goalMat;

    public GameObject cell;
    private static GameObject[,] cells;
    private static GameObject goal;
    private static Coordinate goalCoord;

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
        goalCoord = new Coordinate(6, 18);

        cell.transform.localScale = new Vector3(cellSize, 1.0f, cellSize);

        float positionX = -cellSize * rows / 2 - spacing;
        float positionZ = -cellSize * cols / 2 - spacing;

        Vector3 currentPosition = new Vector3();

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                currentPosition.x = positionX;
                currentPosition.z = positionZ;

                GameObject newCell = Instantiate(cell);
                newCell.transform.position = currentPosition;
                newCell.gameObject.name = "Cell [" + r + "," + c + "]";
                cells[r, c] = newCell;
                grid[r, c] = new Node(true, newCell.transform.position, r, c);

                positionZ += (cellSize + spacing);
            }

            positionX += (cellSize + spacing);
            positionZ = -cellSize * cols / 2 - spacing;
        }

        //goal = cells[5, 15];
        goal = cells[goalCoord.x, goalCoord.y];

        goal.gameObject.GetComponent<Renderer>().material = goalMat;

        cell.gameObject.SetActive(false);

        isInitialized = true;
    }

    public static Transform GetTarget()
    {
        return goal.transform;
    }

    public static Coordinate GetCoordinate(Vector3 position)
    {

        float minDistance = 1000;
        //GameObject closestCell = new GameObject();
        int x = 0, y = 0;

        for (int i = 0; i < cells.GetLength(0); i++)
        {
            for (int j = 0; j < cells.GetLength(1); j++)
            {
                if (Vector3.Distance(position, cells[i,j].transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(position, cells[i, j].transform.position);
                    //closestCell = cells[i, j];
                    x = i;
                    y = j;
                }
            }
        }

        return new Coordinate(x,y);
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

    public static Coordinate GetGoalCoordinate()
    {
        return goalCoord;
    }

    public static GameObject GetCell(int x, int y){
        return cells[x,y];
    }

    public static void SetCellColor(int x, int y){
        cells[x,y].GetComponent<Renderer>().material = goalMat;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
