using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform target;

    GameObject player;

    GridManhattan gridManhattan;
    GridCluster gridCluster;
    public bool isGridManhattan;

    public float maxVelocity;
    public float maxRotationVelocity;
    public float maxAcceleration;
    public float maxRotationAcceleration;

    public float currentRotationVelocity;
    public float currentAcceleration;
    public float currentVelocity;

    public float distanceFromTarget;
    public float distanceFromNextNode;

    public float stopRadius;
    public float slowRadius;

    int manhattanNodeCounter;
    int clusterNodeCounter;

    public Animator anim;
    Vector3 directionVector = new Vector3(0, 0, 0);

    private void Awake()
    {
            gridManhattan = GetComponent<GridManhattan>();
            gridCluster = GetComponent<GridCluster>();
    }

    private void Start()
    {
        player = GameObject.Find("NPC");
        if (isGridManhattan)
            manhattanNodeCounter = gridManhattan.path.Count - 1;
        else
            clusterNodeCounter = gridCluster.path.Count - 1;
    }

    private void Update()
    {
        findPath(player.transform.position, target.position);
        //if (isGridManhattan)
        //    //Debug.Log(gridManhattan.path.Count);
        //else
        //    //Debug.Log(gridCluster.path.Count);
        distanceFromTarget = (player.transform.position - target.position).magnitude;
        if (isGridManhattan)
        {
            SteeringBehaviours(player, gridManhattan.path[manhattanNodeCounter].worldPosition);
            if (player.transform.position == gridManhattan.path[manhattanNodeCounter].worldPosition && manhattanNodeCounter != 0)
                manhattanNodeCounter--;
            //Debug.Log(manhattanNodeCounter);
        }
        else
        {
            SteeringBehaviours(player, gridCluster.path[clusterNodeCounter].worldPosition);
            if (player.transform.position == gridCluster.path[clusterNodeCounter].worldPosition && clusterNodeCounter != 0)
                clusterNodeCounter--;
            //Debug.Log(clusterNodeCounter);
        }
        if(currentVelocity == 0)
        {
            anim.SetBool("walking", false);
            anim.SetBool("running", false);
        }
        else if (currentVelocity <= 1 && currentVelocity > 0)
        {
            anim.SetBool("walking", true);
            anim.SetBool("running", false);
        }
        else if(currentVelocity > 1)
        {
            anim.SetBool("walking", false);
            anim.SetBool("running", true);
        }
        
            
        
    }

    void findPath(Vector3 startPosition, Vector3 targetPosition)
    {
        if (isGridManhattan)
        {
            Node startNode = gridManhattan.NodeFromWorldPoint(startPosition);
            Node targetNode = gridManhattan.NodeFromWorldPoint(targetPosition);

            List<Node> openSet = new List<Node>();
            List<Node> closedSet = new List<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    retracePath(startNode, targetNode);
                    return;
                }

                foreach (Node neighbour in gridManhattan.getNeighbors(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + + getDistanceManhattan(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = getDistanceManhattan(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
                gridManhattan.openSet = openSet;
                gridManhattan.closedSet = closedSet;
            }
        }
        else
        {
            Node startNode = gridCluster.NodeFromWorldPoint(startPosition);
            Node targetNode = gridCluster.NodeFromWorldPoint(targetPosition);

            List<Node> openSet = new List<Node>();
            List<Node> closedSet = new List<Node>();

            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                    {
                        currentNode = openSet[i];
                    }
                }
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    retracePath(startNode, targetNode);
                    return;
                }

                foreach (Node neighbour in gridCluster.getNeighbors(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + getDistanceCluster(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        //if(getDistanceCluster(neighbour, targetNode) != 0)
                            //neighbour.hCost = getDistanceManhattan(neighbour, targetNode);
                        //else
                            neighbour.hCost = getDistanceCluster(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
                gridCluster.openSet = openSet;
                gridCluster.closedSet = closedSet;
            }
        }
    }

    void retracePath(Node startNode, Node targetNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();
        if (isGridManhattan)
            gridManhattan.path = path;
        else
            gridCluster.path = path;
    }

    int getDistanceManhattan(Node nodeA, Node nodeB)
    {
        int distance = Mathf.Abs(nodeA.gridX - nodeB.gridX) + Mathf.Abs(nodeA.gridY - nodeB.gridY);
        //int distance = Mathf.RoundToInt(Mathf.Sqrt((nodeA.gridX - nodeB.gridX)^2 + Mathf.Abs(nodeA.gridY - nodeB.gridY)^2));
        return distance;
    }

    int getDistanceCluster(Node nodeA, Node nodeB)
    {
        Cluster cluster = (gridCluster.clusterTable[nodeA.cluster-1]);
        int cost = cluster.costToClusters[nodeB.cluster-1];
        Debug.Log((nodeA.cluster) + "     " + (nodeB.cluster) + "    " + cost);
        return cost;
    }

    public void SteeringBehaviours(GameObject player, Vector3 targetPosition)
    {
        distanceFromNextNode = (targetPosition - player.transform.position).magnitude;
        directionVector = (targetPosition - player.transform.position).normalized;
        currentRotationVelocity = Mathf.Min(currentRotationVelocity + maxRotationAcceleration, maxRotationVelocity);
        currentAcceleration = Mathf.Min((maxVelocity - currentVelocity)/20, maxAcceleration);
        if (distanceFromTarget <= slowRadius)
        {
            if (distanceFromTarget <= stopRadius)
            {
                //Debug.Log("Distance from target = " + distanceFromTarget + "\nStop radius = " + stopRadius);
                currentVelocity *= 0.0f;
                //Debug.Log("Distance from target = " + distanceFromTarget + "\nSlow radius = " + slowRadius);
            }
            else
                currentVelocity = distanceFromTarget/slowRadius;
        }
        else if (distanceFromTarget > slowRadius)
        {
            currentVelocity = Mathf.Min(currentVelocity + currentAcceleration, maxVelocity);
        }
        Quaternion targetRotation = Quaternion.LookRotation(directionVector);
        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, currentRotationVelocity * Time.deltaTime);
        player.transform.position += ((currentVelocity * Time.deltaTime) * player.transform.forward);
    }
}
