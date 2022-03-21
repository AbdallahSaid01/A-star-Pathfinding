using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCluster : MonoBehaviour
{
    public Transform player;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    public Transform[] clusterTransforms;
    public Cluster[] clusterTable;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    public LayerMask[] cluster;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        createGrid();
        //Debug.Log(grid.Length);

        clusterTable = new Cluster[7];
        clusterTable[0] = new Cluster(1, new int[7], clusterTransforms[0]) ;
        clusterTable[1] = new Cluster(2, new int[7] , clusterTransforms[1]);
        clusterTable[2] = new Cluster(3, new int[7] , clusterTransforms[2]);
        clusterTable[3] = new Cluster(4, new int[7] , clusterTransforms[3]);
        clusterTable[4] = new Cluster(5, new int[7] , clusterTransforms[4]);
        clusterTable[5] = new Cluster(6, new int[7] , clusterTransforms[5]);
        clusterTable[6] = new Cluster(7, new int[7] , clusterTransforms[6]);

        fillClusterTable(clusterTable);
    }

    void createGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkabke = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                int nodeCluster = 0;
                if (Physics.CheckSphere(worldPoint, nodeRadius, cluster[0]))
                    nodeCluster = 1;
                else if (Physics.CheckSphere(worldPoint, nodeRadius, cluster[1]))
                    nodeCluster = 2;
                else if (Physics.CheckSphere(worldPoint, nodeRadius, cluster[2]))
                    nodeCluster = 3;
                else if (Physics.CheckSphere(worldPoint, nodeRadius, cluster[3]))
                    nodeCluster = 4;
                else if (Physics.CheckSphere(worldPoint, nodeRadius, cluster[4]))
                    nodeCluster = 5;
                else if (Physics.CheckSphere(worldPoint, nodeRadius, cluster[5]))
                    nodeCluster = 6;
                else if (Physics.CheckSphere(worldPoint, nodeRadius, cluster[6]))
                    nodeCluster = 7;
                grid[x, y] = new Node(walkabke, worldPoint, x, y, nodeCluster);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> getNeighbors(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    public void fillClusterTable(Cluster[] table)
    {
        for (int i = 0; i < table.Length; i++)

        {
            for (int j = 0; j < table.Length; j++)
            {
                int distance = table[i].costToClusters[j] = Mathf.RoundToInt( Mathf.Abs(table[i].location.position.x - table[j].location.position.x) + Mathf.Abs(table[i].location.position.z - table[j].location.position.z));
                Debug.Log("Cost from cluster " + i + " to cluster " + j + " is: " + distance);
            }
        }
    }

    public List<Node> path;
    public List<Node> openSet;
    public List<Node> closedSet;
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null)
        {
            Debug.Log("grid gizmos is on!");
            Node playerNode = NodeFromWorldPoint(player.position);
            foreach (Node node in grid)
            {
                if (!node.walkable)
                    Gizmos.color = Color.red;

                if (node.walkable && node.cluster == 1)
                    Gizmos.color = Color.blue;
                if (node.walkable && node.cluster == 2)
                    Gizmos.color = Color.yellow;
                if (node.walkable && node.cluster == 3)
                    Gizmos.color = Color.magenta;
                if (node.walkable && node.cluster == 4)
                    Gizmos.color = new Color(0, 0, 255);
                if (node.walkable && node.cluster == 5)
                    Gizmos.color = new Color(153, 0, 76);
                if (node.walkable && node.cluster == 6)
                    Gizmos.color = Color.yellow;
                if (node.walkable && node.cluster == 7)
                    Gizmos.color = Color.blue;
                

                if (openSet != null)
                {
                    if (openSet.Contains(node))
                    {
                        Gizmos.color = Color.green;
                    }
                }
                if (closedSet != null)
                {
                    if (closedSet.Contains(node))
                    {
                        Gizmos.color = Color.red;
                    }
                }
                if (path != null)
                {
                    if (path.Contains(node))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                if (playerNode == node)
                    Gizmos.color = Color.cyan;
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - 0.05f));
            }
        }
    }
}
