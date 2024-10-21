using System.Collections.Generic;
using UnityEngine;

public class CustomGrid : MonoBehaviour
{
    [SerializeField] private LayerMask impassableMask;

    [SerializeField] private Vector2  gridWorldSize;
    [SerializeField] [Range(0, 1f)] private float nodeRadius;
    private float nodeDiameter;

    int gridSizeX;
    int gridSizeY;

    public Node[,] grid { get; private set; }

    public List<Node> pathList { get; set; }

    public void SetUp()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        // 그리드의 왼쪽 아래 좌표 => 첫번째 노드
        Vector3 worldBottomLeft = 
            transform.position - (Vector3.right * gridWorldSize.x / 2)
                               - (Vector3.forward * gridWorldSize.y / 2);

        // 노드들를 순서대로 정렬
        Vector3 worldPoint = Vector3.zero;
        for(int x = 0; x < gridSizeX; ++x)
        {
            for (int y = 0; y < gridSizeY; ++y)
            {
                worldPoint =
                    worldBottomLeft + (Vector3.right * (x * nodeDiameter + nodeRadius))
                                    + (Vector3.forward * (y * nodeDiameter + nodeRadius));

                bool passable = !(Physics.CheckSphere(worldPoint, nodeRadius, impassableMask));

                grid[x, y] = new Node(passable, worldPoint, x, y);
            }
        }
    }

    public List<Node> FindNeighborNodes(Node node)
    {
        List<Node> neighbor = new List<Node>();

        for(int x = -1; x <= 1; ++x)
        {
            for(int y = -1; y <= 1; ++y)
            {
                if (x == 0 && y == 0)
                    continue;

                int neighborX = node.IndexX + x;
                int neighborY = node.IndexY + y;

                if ((neighborX >= 0 && neighborX < gridSizeX) && (neighborY >= 0 && neighborY < gridSizeY))
                    neighbor.Add(grid[neighborX, neighborY]);
            }
        }

        return neighbor;
    }

    public Node WorldPosToGridIndex(Vector3 targetPos)
    {
        // 타겟 포지션이 그리드 월드 사이즈에 비례하여 몇 퍼센트 쯤에 위치해있는지 계산
        float percentX = (targetPos.x  + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (targetPos.z + gridWorldSize.y / 2) / gridWorldSize.y;

        //그리드 월드 사이즈 밖에 위치하면 null 반환
        if (percentX < 0 || percentX > 1 || percentY < 0 || percentY > 1)
            return null;

        // 그리드 인덱스 계산
        int indexX = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int indexY = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[indexX, indexY];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if(grid != null)
        {
            foreach(Node n in grid)
            {
                Gizmos.color = (n.IsPassable) ? Color.white : Color.red;

                if(pathList != null)
                {
                    if(pathList.Contains(n))
                    {
                        Gizmos.color = Color.blue;
                    }
                }

                Gizmos.DrawCube(n.WorldPos, Vector3.one * (nodeDiameter - 0.1f));
            }
        }

    }
}
