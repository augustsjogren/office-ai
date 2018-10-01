using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Material goalMat;
    public Material pathMat;
    public Material obstacleMat;
    public Material celllMat;

    public int x;
    public int y;

    public bool walkable;
    public bool isPath;
    public bool isGoal;

    Renderer rend;
    RaycastHit hit;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForObstacle();

        if (!walkable)
        {
            rend.material = obstacleMat;
        }
        //else if (isPath)
        //{
        //    rend.material = pathMat;
        //}
        else
        {
            rend.material = celllMat;
        }
    }

    // Check if the tile is walkable, and set its status accordingly
    private void CheckForObstacle(){
         // An obstacle is placed in this location, make the cell unwalkable.
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity))
        {
            if (hit.transform.gameObject.name == "CoffeeMachine")
            {
                isGoal = true;
                GridManager.Instance.SetCoffeeLocation(x, y);
            }

            if(hit.transform.gameObject.layer == 9)
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
