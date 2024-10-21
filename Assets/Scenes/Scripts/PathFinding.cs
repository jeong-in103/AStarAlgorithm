using System;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private CustomGrid grid;

    [SerializeField] private Transform beginPos;
    [SerializeField] private Transform targetPos;
    
    private void Start()
    {
        Setup();
    }

    private void Update()
    {
        FindPath();
    }
    
    private void Setup()
    {
        grid = GetComponent<CustomGrid>();

        if (grid.grid == null)
            grid.SetUp();
    }

    public void StartFindPath()
    {
        Setup();
        FindPath();
    }

    private void FindPath()
    {
        // ��ǥ�� �׸��� �ε����� ��ȯ
        Node beginNode = grid.WorldPosToGridIndex(beginPos.position);
        Node targetNode = grid.WorldPosToGridIndex(targetPos.position);

        if (beginNode == null || targetNode == null)
        {
            return;
        }

        List<Node> openList = new List<Node>();
        List<Node> closeList = new List<Node>();

        // ��� ��带 ���� ��Ͽ� �߰�
        openList.Add(beginNode);

        while (openList.Count > 0)
        {
            Node curNode = openList[0];

            // ���� ��Ͽ��� �ڽ�Ʈ�� ���� ���� ��� ����
            for (int i = 1; i < openList.Count; ++i)
            {
                if (openList[i].Cost < curNode.Cost)
                {
                    curNode = openList[i];
                }
            }

            // ���� ��Ͽ��� ���� ��� ���� �� ���� ��Ͽ� �߰�
            openList.Remove(curNode);
            closeList.Add(curNode);

            // ���� ��尡 Ÿ�� ����� Ž�� ����
            if (curNode == targetNode)
            {
                MakePath(beginNode, targetNode);
                return;
            }

            // ������ �� �ִ� ��带 ���� ��Ͽ� �߰�
            List<Node> neighborNode = grid.FindNeighborNodes(curNode);
            foreach (Node node in neighborNode)
            {
                if (!node.IsPassable)
                    continue;

                if (closeList.Contains(node))
                    continue;

                int neighborCostG = curNode.CostG + GetCost(curNode, node);
                if(!openList.Contains(node) || neighborCostG < node.CostG )
                {
                    // G cost : ���� ��忡�� ���� ������
                    node.CostG = neighborCostG;
                    // H cost : ���� ��忡�� ��ǥ ������
                    node.CostH = GetCost(node, targetNode);
                    // �θ� ��� ����
                    node.parentNode = curNode;

                    openList.Add(node);
                }
            }
        }
    }

    // Ž�� ���� �Ŀ� ���� ����� �θ� ��带 �����Ͽ� ����Ʈ�� �߰���
    private void MakePath(Node beginNode, Node targetNode)
    {
        List <Node> pathList = new List<Node>();
        Node curNode = targetNode;

        while (curNode != beginNode) 
        { 
            pathList.Add(curNode);
            curNode = curNode.parentNode;
        }

        pathList.Reverse();
        grid.pathList = pathList;
    }

    // �� ��尣�� �Ÿ��� cost ���
    private int GetCost(Node node1, Node node2)
    {
        int distX = Mathf.Abs(node1.IndexX - node2.IndexX);
        int distY = Mathf.Abs(node1.IndexY - node2.IndexY);

        if (distX > distY)
            return (14 * distY) + 10 * (distX - distY);

        return (14 * distX) + 10 * (distY - distX);
    }
}
