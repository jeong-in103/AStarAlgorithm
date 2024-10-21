using UnityEngine;

public class Node
{
    public Vector3 WorldPos;
    public int IndexX;
    public int IndexY;

    public Node parentNode;

    public int CostG;
    public int CostH;
    public int Cost => CostG + CostH;
    
    public bool IsPassable;

    public Node()
    {
        IsPassable = true;
        WorldPos = new Vector3(0, 0, 0);
    }

    public Node(bool isPassable, Vector3 worldPos, int indexX, int indexY)
    {
        IsPassable = isPassable;
        WorldPos = worldPos;
        IndexX = indexX;
        IndexY = indexY;
    }
}
