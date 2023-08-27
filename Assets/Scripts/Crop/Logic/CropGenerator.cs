using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Map;

namespace MFarm.CropPlant
{
    public class CropGenerator : MonoBehaviour
    {
        private Grid currentGrid;
        public int seedItemID;
        public int growthDays;

        private void Awake()
        {
            currentGrid = FindObjectOfType<Grid>();
        }

        private void OnEnable()
        {
            EventHandler.GenerateCropEvent += GenerateCrop;
        }

        private void OnDisable()
        {
            EventHandler.GenerateCropEvent -= GenerateCrop;
        }

       
        /// <summary>
        /// Ԥ��������Ʒ
        /// </summary>
        private void GenerateCrop()
        {
            Vector3Int cropGridPos = currentGrid.WorldToCell(transform.position);

            if(seedItemID != 0)
            {
                var tile = GridMapManager.Instance.GetTileDetailsOnMousePosition(cropGridPos);

                if(tile == null)
                {
                    tile = new TileDetails();

                    // ��ʼ������ֹԤ��������(0,0)����
                    tile.gridX = cropGridPos.x;
                    tile.gridY = cropGridPos.y;
                }

                tile.daySinceWatered = -1;
                tile.seedItemID = seedItemID;
                tile.growthDays = growthDays;

                GridMapManager.Instance.UpdateTileDetails(tile);
            }
        }
    }
}
