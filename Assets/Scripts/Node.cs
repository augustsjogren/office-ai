﻿using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;
    public bool isOccupied;

    int heapIndex;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public Node(bool _walkable, Vector3 _worldPos, Coordinate coord)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = coord.x;
        gridY = coord.y;
    }

    public Node(Coordinate coord)
    {
        walkable = true;
        worldPosition = GridManager.Instance.GetCell(coord).transform.position;
        gridX = coord.x;
        gridY = coord.y;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }
}