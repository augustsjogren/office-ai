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

    Renderer rend;

    LayerMask layerMask = 1 << 9;

    RaycastHit hit;


    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    // Use this for initialization
    void Start()
    {
        CheckForObstacle();
    }

    // Update is called once per frame
    void Update()
    {
       
        CheckForObstacle();

        if (!walkable)
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

    private void CheckForObstacle(){
         // An obstacle is placed in this location, make the cell unwalkable.
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity, layerMask))
        {
            walkable = false;
            GridManager.Instance.SetWalkable(x, y, false);
        }
        else
        {
            walkable = true;
            GridManager.Instance.SetWalkable(x, y, true);
        }
    }
}
