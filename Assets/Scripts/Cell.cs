using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Material obstacleMat;
    public Material celllMat;
    public Material pathMat;

    public int x;
    public int y;

    public bool walkable;
    public bool isPath;
    public bool isGoal;
    public bool isOccupied;

    Renderer rend;
    RaycastHit hit;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void InitCell()
    {

        CheckForObstacle();

        if (!walkable || isOccupied)
        {
            rend.material = obstacleMat;
        }
        else if (isPath)
        {
            rend.material = pathMat;
        }
        else
        {
            rend.material = celllMat;
        }

        //Disable the object to improve performance
        //gameObject.SetActive(false);
    }

    public void UpdateCell()
    {
        CheckForObstacle();

        if (!walkable || isOccupied)
        {
            rend.material = obstacleMat;
        }
        else if (isPath)
        {
            rend.material = pathMat;
        }
        else
        {
            rend.material = celllMat;
        }
    }

    // Check if the tile is walkable, and set its status accordingly
    private void CheckForObstacle()
    {
        // An obstacle is placed in this location, make the cell unwalkable.
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity))
        {
            var hitObj = hit.transform.gameObject;

            if (hitObj.tag == "CoffeeMachine")
            {
                GridManager.Instance.SetCoffeeLocation(x, y);

                Coordinate newCoord = (new Coordinate(x, y));

                if (!GridManager.Instance.coffeeCoords.Contains(newCoord))
                {
                    GridManager.Instance.coffeeCoords.Add(newCoord);
                }
            }
            else if (hitObj.name == "SnackMachine")
            {
                GridManager.Instance.SetSnackLocation(x, y);
            }
            else if (hitObj.tag == "Toilet")
            {
                GridManager.Instance.toiletCoord = new Coordinate(x, y);
            }
            else if (hitObj.tag == "Sink")
            {
                GridManager.Instance.sinkCoord = new Coordinate(x, y);
            }
            else if (hit.transform.tag == "Restaurant")
            {
                GridManager.Instance.SetRestaurantLocation(x, y);
            }

            if (hitObj.layer == 9)
            {
                walkable = false;
                GridManager.Instance.SetWalkable(x, y, false);
            }
        }
        else
        {
            walkable = true;
            GridManager.Instance.SetWalkable(x, y, true);
        }
    }
}
