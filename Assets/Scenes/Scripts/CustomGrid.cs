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

        // �׸����� ���� �Ʒ� ��ǥ => ù��° ���
        Vector3 worldBottomLeft = 
            transform.position - (Vector3.right * gridWorldSize.x / 2)
                               - (Vector3.forward * gridWorldSize.y / 2);

        // ���鸦 ������� ����
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
        // Ÿ�� �������� �׸��� ���� ����� ����Ͽ� �� �ۼ�Ʈ �뿡 ��ġ���ִ��� ���
        float percentX = (targetPos.x  + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (targetPos.z + gridWorldSize.y / 2) / gridWorldSize.y;

        //�׸��� ���� ������ �ۿ� ��ġ�ϸ� null ��ȯ
        if (percentX < 0 || percentX > 1 || percentY < 0 || percentY > 1)
            return null;

        // �׸��� �ε��� ���
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
