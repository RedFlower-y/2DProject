using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.AStar
{
    public class GridNodes
    {
        private int width;
        private int height;
        private Node[,] gridNodes;

        /// <summary>
        /// 构造函数初始化节点范围数组
        /// </summary>
        /// <param name="width">地图宽度</param>
        /// <param name="height">地图高度</param>
        public GridNodes(int width, int height)
        {
            this.width = width;
            this.height = height;

            gridNodes = new Node[width, height];

            for (int x = 0; x < width; x++)
            {
                for(int y = 0; y < height; y++)
                {
                    gridNodes[x, y] = new Node(new Vector2Int(x, y));
                }
            }
        }


        public Node GetGridNode(int xPos,int yPos)
        {
            if (xPos < width && yPos < height)
                return gridNodes[xPos, yPos];
            Debug.Log("超出网格范围");
            return null;
        }
    }
}
