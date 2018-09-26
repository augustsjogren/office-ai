using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerMind : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3 targetPosition;

    public Transform target;

    public float step = 1;

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

        // TODO: implement A* here



        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }
    }

}
