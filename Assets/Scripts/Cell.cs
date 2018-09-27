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

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
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
