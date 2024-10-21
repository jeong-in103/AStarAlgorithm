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
        // 좌표를 그리드 인덱스로 변환
        Node beginNode = grid.WorldPosToGridIndex(beginPos.position);
        Node targetNode = grid.WorldPosToGridIndex(targetPos.position);

        if (beginNode == null || targetNode == null)
        {
            return;
        }

        List<Node> openList = new List<Node>();
        List<Node> closeList = new List<Node>();

        // 출발 노드를 열린 목록에 추가
        openList.Add(beginNode);

        while (openList.Count > 0)
        {
            Node curNode = openList[0];

            // 열린 목록에서 코스트가 가장 작은 노드 선택
            for (int i = 1; i < openList.Count; ++i)
            {
                if (openList[i].Cost < curNode.Cost)
                {
                    curNode = openList[i];
                }
            }

            // 열린 목록에서 현재 노드 삭제 및 닫힌 목록에 추가
            openList.Remove(curNode);
            closeList.Add(curNode);

            // 현재 노드가 타겟 노드라면 탐색 종료
            if (curNode == targetNode)
            {
                MakePath(beginNode, targetNode);
                return;
            }

            // 지나갈 수 있는 노드를 열린 목록에 추가
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
                    // G cost : 시작 노드에서 현재 노드까지
                    node.CostG = neighborCostG;
                    // H cost : 현재 노드에서 목표 노드까지
                    node.CostH = GetCost(node, targetNode);
                    // 부모 노드 설정
                    node.parentNode = curNode;

                    openList.Add(node);
                }
            }
        }
    }

    // 탐색 종료 후에 최종 노드의 부모 노드를 추적하여 리스트에 추가함
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

    // 두 노드간의 거리로 cost 계산
    private int GetCost(Node node1, Node node2)
    {
        int distX = Mathf.Abs(node1.IndexX - node2.IndexX);
        int distY = Mathf.Abs(node1.IndexY - node2.IndexY);

        if (distX > distY)
            return (14 * distY) + 10 * (distX - distY);

        return (14 * distX) + 10 * (distY - distX);
    }
}
