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
                // ��һ����ֲ
                tileDetails.seedItemID = itemID;
                tileDetails.growthDays = 0;
                // ��ʾũ����
                DisplayCropPlant(tileDetails, currentCrop);
            }
            else if(tileDetails.seedItemID != -1)
            {
                // ˢ�µ�ͼ
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
        /// ��ʾũ����
        /// </summary>
        /// <param name="tileDetails">��Ƭ��ͼ��Ϣ</param>
        /// <param name="cropDetails">������Ϣ</param>
        private void DisplayCropPlant(TileDetails tileDetails,CropDetails cropDetails)
        {
            //�ɳ��׶�
            int growthStages = cropDetails.growthDays.Length;
            int currentStage = 0;
            int dayCounter = cropDetails.TotalGrowthDays;

            // ������㵱ǰ�ĳɳ��׶�
            for (int i = growthStages - 1; i >= 0; i--)
            {
                if (tileDetails.growthDays >= dayCounter)
                {
                    // ũ�����Ѿ�������
                    currentStage = i;
                    break;
                }
                dayCounter -= cropDetails.growthDays[i];
            }

            // ��ȡũ���ﵱǰ�׶ε�Prefab��Sprite
            GameObject cropPrefab = cropDetails.growthPrefabs[currentStage];
            Sprite cropSprite = cropDetails.growthSprites[currentStage];

            // ��ȡũ�����λ��
            Vector3 pos = new Vector3(tileDetails.gridX + 0.5f, tileDetails.gridY + 0.5f, 0);

            GameObject cropInstance = Instantiate(cropPrefab, pos, Quaternion.identity, cropParent);        // ʵ����
            cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = cropSprite;                      // ��ʾͼƬ
        }

        /// <summary>
        /// ͨ����ƷID����������Ϣ
        /// </summary>
        /// <param name="itemID">��ƷID</param>
        /// <returns>������Ϣ</returns>
        private CropDetails GetCropDetails(int itemID)
        {
            return cropData.cropDetailsList.Find(c => c.seedItemID == itemID);
        }

        /// <summary>
        /// �жϵ�ǰ�����Ƿ������ֲ
        /// </summary>
        /// <param name="crop">������Ϣ</param>
        /// <returns>true ������ֲ false ��������ֲ</returns>
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