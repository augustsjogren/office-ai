using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    //void Update()
    //{

    //    RaycastHit hit;

    //    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity))
    //    {
    //        var hitObj = hit.transform.gameObject;
    //        print(hitObj.name);

    //        if (hitObj.layer == 10)
    //        {

    //            var hitCell = hitObj.GetComponent<Cell>();
    //            hitCell.walkable = false;
    //            GridManager.Instance.SetWalkable(hitCell.x, hitCell.y, false);
    //        }
    //    }

    //}
}
