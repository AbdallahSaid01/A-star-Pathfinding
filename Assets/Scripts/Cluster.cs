using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster
{
    public Transform location;
    public int clusterNumber;
    public int[] costToClusters;

    public Cluster(int _clusterNumber, int[] _costToClusters, Transform _location)
    {
        clusterNumber = _clusterNumber;
        costToClusters = _costToClusters;
        location = _location;
    }
}
