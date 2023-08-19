using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Map;

namespace MFarm.AStar
{
    public class AStar : MonoBehaviour
    {
        private GridNodes       gridNodes;
        private Node            startNode;
        private Node            targetNode;
        private int             gridWidth;
        private int             gridHeight;
        private int             originX;
        private int             originY;
        private List<Node>      openNodeList;           // 当前选中Node周围的8个点
        private HashSet<Node>   closedNodeList;         // 所有被选中的点
        private bool            pathFound;

        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int targetPos)
        {
            pathFound = false;

            if(GenerateGridNodes(sceneName,startPos,targetPos))
            {
                // 查找最短路径
                if(FindShortestPath())
                {
                    // 构建NPC移动路径

                }
            }
        }

        /// <summary>
        /// 构建网格节点信息，初始化两个列表
        /// </summary>
        /// <param name="sceneName">场景名字</param>
        /// <param name="startPos">起点</param>
        /// <param name="targetPos">终点</param>
        /// <returns></returns>
        private bool GenerateGridNodes(string sceneName, Vector2Int startPos, Vector2Int targetPos)
        {
            if(GridMapManager.Instance.GetGridDimensions(sceneName,out Vector2Int gridDimensions,out Vector2Int gridOrigin))
            {
                // 根据瓦片地图范围构建网格移动节点范围数据
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;

                // 初始化
                openNodeList = new List<Node>();
                closedNodeList = new HashSet<Node>();
            }
            else
            {
                return false;
            }

            // gridNodes的范围是从0,0开始，所以需要减去原点坐标得到实际坐标
            startNode   = gridNodes.GetGridNode(startPos.x  - originX, startPos.y  - originY);
            targetNode  = gridNodes.GetGridNode(targetPos.x - originX, targetPos.y - originY);

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x + originX, y + originY, 0);
                    TileDetails tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(tilePos);      // 偷懒

                    if(tile != null)
                    {
                        Node node = gridNodes.GetGridNode(x, y);

                        if (tile.isNPCObstacle)
                            node.isObstacle = true;                 // 地图信息导入到节点信息中
                    }
                }
            }

            return true;
        }

        private bool FindShortestPath()
        {
            // 添加起点
            openNodeList.Add(startNode);

            while (openNodeList.Count > 0)
            {
                // 节点排序，Node内含比较函数
                openNodeList.Sort();

                // 找到最近节点，openNodeList中删除，并添加到closedNodeList中
                Node closeNode = openNodeList[0];
                openNodeList.RemoveAt(0);
                closedNodeList.Add(closeNode);

                if (closeNode == targetNode)
                {
                    // 已完成最短路径的查找
                    pathFound = true;
                    break;
                }

                // 计算周围8个Node补充到OpenList
            }
            return pathFound;
        }
    }
}
