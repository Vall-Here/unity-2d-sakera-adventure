using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AStarPathfinding
{
    public class Node
    {
        public Vector2Int gridPosition;
        public bool isWalkable;
        public Vector3 worldPosition;
        public int gCost; 
        public int hCost;
        public Node parent;

        public int FCost => gCost + hCost;

        public Node(Vector2Int gridPosition, bool isWalkable, Vector3 worldPosition)
        {
            this.gridPosition = gridPosition;
            this.isWalkable = isWalkable;
            this.worldPosition = worldPosition;
            this.gCost = int.MaxValue; 
            this.hCost = int.MaxValue; 
            this.parent = null;
        }
    }
}
