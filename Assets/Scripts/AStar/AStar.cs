using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Map;

namespace MFarm.AStar
{
    public class AStar : Singloten<AStar>
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

        /// <summary>
        /// 构建路径更新Stack的每一步
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="startPos"></param>
        /// <param name="targetPos"></param>
        /// <param name="npcMovementStack">NPC移动路径</param>
        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int targetPos, Stack<MovementStep> npcMovementStack)
        {
            pathFound = false;

            if(GenerateGridNodes(sceneName,startPos,targetPos))
            {
                // 查找最短路径
                if(FindShortestPath())
                {
                    // 构建NPC移动路径
                    UpdatePathOnMovementStepStack(sceneName, npcMovementStack);
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


        /// <summary>
        /// 找到最短路径，并将其所有的Node添加到closedNodeList
        /// </summary>
        /// <returns></returns>
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

                // 计算周围8个Node 并补充到OpenList
                EvaluateNeighbourNodes(closeNode);
            }
            return pathFound;
        }

        /// <summary>
        /// 评估周围8个点 并生成对应消耗值
        /// </summary>
        /// <param name="currentNode"></param>
        private void EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPosition;
            Node vaildNeighbourNode;

            // 以自身为中心 包含周围3X3的正方形
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    vaildNeighbourNode = GetValidNeighbourNode(currentNodePos.x + x, currentNodePos.y + y);

                    if(vaildNeighbourNode != null)
                    {
                        if(!openNodeList.Contains(vaildNeighbourNode))
                        {
                            vaildNeighbourNode.gCost = currentNode.gCost + GetDistance(currentNode, vaildNeighbourNode);
                            vaildNeighbourNode.hCost = GetDistance(vaildNeighbourNode, targetNode);
                            vaildNeighbourNode.parentNode = currentNode;        // 链接父节点
                            openNodeList.Add(vaildNeighbourNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 找到有效的Node 非障碍 非已选择
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Node GetValidNeighbourNode(int x, int y)
        {
            if (x >= gridWidth || y >= gridHeight || x < 0 || y < 0)
                return null;

            Node neighbourNode = gridNodes.GetGridNode(x, y);

            if (neighbourNode.isObstacle || closedNodeList.Contains(neighbourNode))
                return null;
            else // 已经遍历过的点除开
                return neighbourNode;
        }

        /// <summary>
        /// 返回两点距离值
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns>14的倍数+10的倍数</returns>
        private int GetDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (xDistance > yDistance)
                return 14 * yDistance + 10 * (xDistance - yDistance);

            return 14 * xDistance + 10 * (yDistance - xDistance);
        }

        /// <summary>
        /// 更新路径每一步的坐标和场景名字
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="npcMovemnetStep"></param>
        private void UpdatePathOnMovementStepStack(string sceneName, Stack<MovementStep> npcMovemnetStep)
        {
            Node nextNode = targetNode;
            while (nextNode != null)
            {
                MovementStep newStep = new MovementStep();
                newStep.sceneName = sceneName;
                newStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);  // 场景的真实坐标

                npcMovemnetStep.Push(newStep);      // 压入堆栈
                nextNode = nextNode.parentNode;
            }
        }
    }
}
