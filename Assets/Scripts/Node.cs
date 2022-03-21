using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public int cluster;
    public Vector3 worldPosition;
    public int gCost;
    public int hCost;
    public Node parent;

    public int gridX;
    public int gridY;

    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY, int _cluster)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
        cluster = _cluster;
    }

    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
