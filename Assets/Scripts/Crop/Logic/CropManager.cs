using UnityEngine;

namespace MFarm.Crop
{
    public class CropManager : MonoBehaviour
    {
        public CropDataList_SO cropData;
        private Transform cropParent;
        private Grid currentGrid;
        private Season currentSeason;

        private void OnEnable()
        {
            EventHandler.PlantSeedEvent         += OnPlantSeedEvent;
            EventHandler.AfterSceneLoadedEvent  += OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent           += OnGameDayEvent;
        }

        private void OnDisable()
        {
            EventHandler.PlantSeedEvent         -= OnPlantSeedEvent;
            EventHandler.AfterSceneLoadedEvent  -= OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent           -= OnGameDayEvent;
        }

        private void OnPlantSeedEvent(int itemID, TileDetails tileDetails)
        {
            CropDetails currentCrop = GetCropDetails(itemID);
            if (currentCrop != null && SeasonAvailable(currentCrop) && tileDetails.seedItemID == -1)
            {
                // 第一次种植
                tileDetails.seedItemID = itemID;
                tileDetails.growthDays = 0;
                // 显示农作物
                DisplayCropPlant(tileDetails, currentCrop);
            }
            else if(tileDetails.seedItemID != -1)
            {
                // 刷新地图
                DisplayCropPlant(tileDetails, currentCrop);
            }
        }

        private void OnAfterSceneLoadedEvent()
        {
            currentGrid = FindObjectOfType<Grid>();
            cropParent = GameObject.FindWithTag("CropParent").transform;
        }

        private void OnGameDayEvent(int gameDay, Season gameSeason)
        {
            currentSeason = gameSeason;
        }

        /// <summary>
        /// 显示农作物
        /// </summary>
        /// <param name="tileDetails">瓦片地图信息</param>
        /// <param name="cropDetails">种子信息</param>
        private void DisplayCropPlant(TileDetails tileDetails,CropDetails cropDetails)
        {
            //成长阶段
            int growthStages = cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = cropDetails.TotalGrowthDays;

            // 倒序计算当前的成长阶段
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (tileDetails.growthDays >= dayCounter)
                {
                    // 农作物已经成熟了
                    currentStage = i;
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];
            }

            // 获取农作物当前阶段的Prefab和Sprite
            GameObject cropPrefab = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthSprites[currentStage];

            // 获取农作物的位置
            Vector3 pos = new Vector3(tileDetails.gridX + 0.5f, tileDetails.gridY + 0.5f, 0);

            GameObject cropInstance = Instantiate(cropPrefab, pos, Quaternion.identity, cropParent);        // 实例化
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;                      // 显示图片
        }

        /// <summary>
        /// 通过物品ID查找种子信息
        /// </summary>
        /// <param name="itemID">物品ID</param>
        /// <returns>种子信息</returns>
        private CropDetails GetCropDetails(int itemID)
        {
            return cropData.cropDetailsList.Find(c => c.seedItemID == itemID);
        }

        /// <summary>
        /// 判断当前季节是否可以种植
        /// </summary>
        /// <param name="crop">种子信息</param>
        /// <returns>true 可以种植 false 不可以种植</returns>
        private bool SeasonAvailable(CropDetails crop)
        {
            for(int i = 0; i<crop.seasons.Length; i++)
            {
                if (crop.seasons[i] == currentSeason)
                    return true;
            }
            return false;
        }
    }
}