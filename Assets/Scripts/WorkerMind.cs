using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerMind : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    public Transform target;

    public Vector3 nextLocation;

    PathFinding pathFinding;

    public float step = 1;

    private void Awake()
    {
        pathFinding = GetComponent<PathFinding>();
    }

    // Use this for initialization
    void Start()
    {
        initialPosition = transform.position;
        targetPosition = new Vector3();
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    // Update is called once per frame
    void Update()
    {
        target = GridManager.GetTarget();
        targetPosition = target.position;
        targetPosition.y = initialPosition.y;

        if (GridManager.isInitialized && pathFinding.HasRetraced && pathFinding.GetPath().Count > 0)
        {
            nextLocation = pathFinding.GetPath()[0].worldPosition;
            nextLocation.y = initialPosition.y;
        }

        // Move towards the next location in the path
        if (nextLocation != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextLocation, step);
        }
    }

}
