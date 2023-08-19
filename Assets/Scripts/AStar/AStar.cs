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
        private List<Node>      openNodeList;           // ��ǰѡ��Node��Χ��8����
        private HashSet<Node>   closedNodeList;         // ���б�ѡ�еĵ�
        private bool            pathFound;

        public void BuildPath(string sceneName, Vector2Int startPos, Vector2Int targetPos)
        {
            pathFound = false;

            if(GenerateGridNodes(sceneName,startPos,targetPos))
            {
                // �������·��
                if(FindShortestPath())
                {
                    // ����NPC�ƶ�·��

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

                // ������Χ8��Node���䵽OpenList
            }
            return pathFound;
        }
    }
}
