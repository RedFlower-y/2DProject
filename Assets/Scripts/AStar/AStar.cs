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
        private List<Node>      openNodeList;           // ��ǰѡ��Node��Χ��8����
        private HashSet<Node>   closedNodeList;         // ���б�ѡ�еĵ�
        private bool            pathFound;

        /// <summary>
        /// ����·������Stack��ÿһ��
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="startPos"></param>
        /// <param name="targetPos"></param>
        /// <param name="npcMovementStack">NPC�ƶ�·��</param>
        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int targetPos, Stack<MovementStep> npcMovementStack)
        {
            pathFound = false;

            if(GenerateGridNodes(sceneName,startPos,targetPos))
            {
                // �������·��
                if(FindShortestPath())
                {
                    // ����NPC�ƶ�·��
                    UpdatePathOnMovementStepStack(sceneName, npcMovementStack);
                }
            }
        }

        /// <summary>
        /// ��������ڵ���Ϣ����ʼ�������б�
        /// </summary>
        /// <param name="sceneName">��������</param>
        /// <param name="startPos">���</param>
        /// <param name="targetPos">�յ�</param>
        /// <returns></returns>
        private bool GenerateGridNodes(string sceneName, Vector2Int startPos, Vector2Int targetPos)
        {
            if(GridMapManager.Instance.GetGridDimensions(sceneName,out Vector2Int gridDimensions,out Vector2Int gridOrigin))
            {
                // ������Ƭ��ͼ��Χ���������ƶ��ڵ㷶Χ����
                gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                gridWidth = gridDimensions.x;
                gridHeight = gridDimensions.y;
                originX = gridOrigin.x;
                originY = gridOrigin.y;

                // ��ʼ��
                openNodeList = new List<Node>();
                closedNodeList = new HashSet<Node>();
            }
            else
            {
                return false;
            }

            // gridNodes�ķ�Χ�Ǵ�0,0��ʼ��������Ҫ��ȥԭ������õ�ʵ������
            startNode   = gridNodes.GetGridNode(startPos.x  - originX, startPos.y  - originY);
            targetNode  = gridNodes.GetGridNode(targetPos.x - originX, targetPos.y - originY);

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x + originX, y + originY, 0);
                    TileDetails tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(tilePos);      // ͵��

                    if(tile != null)
                    {
                        Node node = gridNodes.GetGridNode(x, y);

                        if (tile.isNPCObstacle)
                            node.isObstacle = true;                 // ��ͼ��Ϣ���뵽�ڵ���Ϣ��
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// �ҵ����·�������������е�Node��ӵ�closedNodeList
        /// </summary>
        /// <returns></returns>
        private bool FindShortestPath()
        {
            // ������
            openNodeList.Add(startNode);

            while (openNodeList.Count > 0)
            {
                // �ڵ�����Node�ں��ȽϺ���
                openNodeList.Sort();

                // �ҵ�����ڵ㣬openNodeList��ɾ��������ӵ�closedNodeList��
                Node closeNode = openNodeList[0];
                openNodeList.RemoveAt(0);
                closedNodeList.Add(closeNode);

                if (closeNode == targetNode)
                {
                    // ��������·���Ĳ���
                    pathFound = true;
                    break;
                }

                // ������Χ8��Node �����䵽OpenList
                EvaluateNeighbourNodes(closeNode);
            }
            return pathFound;
        }

        /// <summary>
        /// ������Χ8���� �����ɶ�Ӧ����ֵ
        /// </summary>
        /// <param name="currentNode"></param>
        private void EvaluateNeighbourNodes(Node currentNode)
        {
            Vector2Int currentNodePos = currentNode.gridPosition;
            Node vaildNeighbourNode;

            // ������Ϊ���� ������Χ3X3��������
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
                            vaildNeighbourNode.parentNode = currentNode;        // ���Ӹ��ڵ�
                            openNodeList.Add(vaildNeighbourNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// �ҵ���Ч��Node ���ϰ� ����ѡ��
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
            else // �Ѿ��������ĵ����
                return neighbourNode;
        }

        /// <summary>
        /// �����������ֵ
        /// </summary>
        /// <param name="nodeA"></param>
        /// <param name="nodeB"></param>
        /// <returns>14�ı���+10�ı���</returns>
        private int GetDistance(Node nodeA, Node nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.gridPosition.x - nodeB.gridPosition.x);
            int yDistance = Mathf.Abs(nodeA.gridPosition.y - nodeB.gridPosition.y);

            if (xDistance > yDistance)
                return 14 * yDistance + 10 * (xDistance - yDistance);

            return 14 * xDistance + 10 * (yDistance - xDistance);
        }

        /// <summary>
        /// ����·��ÿһ��������ͳ�������
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
                newStep.gridCoordinate = new Vector2Int(nextNode.gridPosition.x + originX, nextNode.gridPosition.y + originY);  // ��������ʵ����

                npcMovemnetStep.Push(newStep);      // ѹ���ջ
                nextNode = nextNode.parentNode;
            }
        }
    }
}
