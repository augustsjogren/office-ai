using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

    public Material goalMat;
    public Material pathMat;
    public Material obstacleMat;
    public Material celllMat;


    private int x;
    private int y;

    public bool walkable;
    public bool isPath;

    Renderer renderer;

    LayerMask layerMask = 1 << 9;

    RaycastHit hit;


    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    // Use this for initialization
    void Start () {

        
       

    }
	
	// Update is called once per frame
	void Update () {

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Did Hit obstacle");
            walkable = false;
            GridManager.Instance.SetWalkable(x, y, false);
        }


        if (!walkable)
        {
            renderer.material = obstacleMat;
        }
        else if (isPath)
        {
            renderer.material = pathMat;
        }
        else
        {
            renderer.material = celllMat;
        }
	}
}
