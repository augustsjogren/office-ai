using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Material goalMat;

    public GameObject cell;
    private GameObject[,] cells;
    private static GameObject goal;

    public int rows;
    public int cols;
    public float cellSize = 1.0f;

    // Use this for initialization
    void Start()
    {
        goal = new GameObject();

        cells = new GameObject[rows, cols];

        cell.transform.localScale = new Vector3(cellSize, 1.0f, cellSize);

        float positionX = -cellSize * rows / 2;
        float positionZ = -cellSize * cols / 2;

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

                positionZ += cellSize;
            }

            positionX += cellSize;
            positionZ = -cellSize * cols / 2;
        }

        cells[5, 5].gameObject.GetComponent<Renderer>().material = goalMat;

        goal = cells[5, 5];

        cell.gameObject.SetActive(false);
    }

    public static Transform GetTarget()
    {
        return goal.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
