using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using MFarm.CropPlant;

namespace MFarm.Map
{
    public class GridMapManager : Singloten<GridMapManager>
    {
        [Header("种地瓦片切换信息")]
        public RuleTile digTile;
        public RuleTile waterTile;
        private Tilemap digTilemap;
        private Tilemap waterTilemap;

        [Header("地图信息")]
        public List<MapData_SO> mapDataList;

        private Dictionary<string, TileDetails> tileDetailsDict = new Dictionary<string, TileDetails>();    // 场景名字+坐标和对应的瓦片信息
        private Dictionary<string, bool> isFirstLoadDict = new Dictionary<string, bool>();                  // 判断场景是否是第一次加载 场景预加载物体相关
        private Grid currentGrid;
        private Season currentSeason;
        private List<ReapItem> itemsInRadius;        // 杂草列表

        private void OnEnable()
        {
            EventHandler.ExecuteActionAfterAnimationEvent   += OnExecuteActionAfterAnimationEvent;
            EventHandler.AfterSceneLoadedEvent              += OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent                       += OnGameDayEvent;
            EventHandler.RefreshCurrentMap                  += RefreshMap;
        }


        private void OnDisable()
        {
            EventHandler.ExecuteActionAfterAnimationEvent   -= OnExecuteActionAfterAnimationEvent;
            EventHandler.AfterSceneLoadedEvent              -= OnAfterSceneLoadedEvent;
            EventHandler.GameDayEvent                       -= OnGameDayEvent;
            EventHandler.RefreshCurrentMap                  -= RefreshMap;
        }


        private void Start()
        {
            foreach(var mapData in mapDataList)
            {
                isFirstLoadDict.Add(mapData.sceneName, true);
                InitTileDetailsDict(mapData);
            }
        }

        private void OnAfterSceneLoadedEvent()
        {
            currentGrid = FindObjectOfType<Grid>();

            digTilemap      = GameObject.FindWithTag("Dig").GetComponent<Tilemap>();
            waterTilemap    = GameObject.FindWithTag("Water").GetComponent<Tilemap>();

            if (isFirstLoadDict[SceneManager.GetActiveScene().name])
            {
                // 预先生成农作物
                EventHandler.CallGenerateCropEvent();
                isFirstLoadDict[SceneManager.GetActiveScene().name] = false; // 第一次加载完成后。改为false
            }
            
            RefreshMap();
        }

        private void OnGameDayEvent(int gameDay, Season gameSeason)
        {
            currentSeason = gameSeason;

            foreach(var tile in tileDetailsDict)
            {
                if (tile.Value.daySinceWatered > -1)
                    tile.Value.daySinceWatered = -1;
                if (tile.Value.daySinceDug > -1)
                    tile.Value.daySinceDug++;
                if (tile.Value.daySinceDug > 5 && tile.Value.seedItemID == -1)  
                {
                    // 超期消除挖坑
                    tile.Value.daySinceDug = -1;
                    tile.Value.canDig = true;
                    tile.Value.growthDays = -1;
                }
                if(tile.Value.seedItemID != -1)
                {
                    // 种植了种子
                    tile.Value.growthDays++;
                }
            }
            RefreshMap();
        }

        /// <summary>
        /// 根据地图信息生成字典
        /// </summary>
        /// <param name="mapData"></param>
        private void InitTileDetailsDict(MapData_SO mapData)
        {
            foreach (TileProperty tileProperty in mapData.tileProperties)
            {
                TileDetails tileDetails = new TileDetails
                {
                    gridX = tileProperty.tileCoordinate.x,
                    gridY = tileProperty.tileCoordinate.y,
                };

                string key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + mapData.sceneName;     //字典的key

                if(GetTileDetails(key) != null)
                {
                    tileDetails = GetTileDetails(key);
                }

                switch (tileProperty.gridType)
                {
                    case GridType.Diggable:
                        tileDetails.canDig = tileProperty.boolTypeValue;
                        break;
                    case GridType.DropItem:
                        tileDetails.canDropItem = tileProperty.boolTypeValue;
                        break;
                    case GridType.PlaceFurniture:
                        tileDetails.canPlaceFurniture = tileProperty.boolTypeValue;
                        break;
                    case GridType.NPCObstacle:
                        tileDetails.isNPCObstacle = tileProperty.boolTypeValue;
                        break;
                }

                // 如果已经存在TileDetails则更新，否则新建
                if (GetTileDetails(key) != null)
                    tileDetailsDict[key] = tileDetails;
                else
                    tileDetailsDict.Add(key, tileDetails);
            }
        }

        /// <summary>
        /// 根据key返回瓦片地图信息
        /// </summary>
        /// <param name="key">key值</param>
        /// <returns></returns>
        private TileDetails GetTileDetails(string key)
        {
            if(tileDetailsDict.ContainsKey(key))
                return tileDetailsDict[key];
            else
                return null;
        }

        /// <summary>
        /// 获取鼠标网格坐标所指瓦片地图信息
        /// </summary>
        /// <param name="mouseGridPos">鼠标网格坐标</param>
        /// <returns></returns>
        public TileDetails GetTileDetailsOnMousePosition(Vector3Int mouseGridPos)
        {
            string key = mouseGridPos.x + "x" + mouseGridPos.y + "y" + SceneManager.GetActiveScene().name;     //字典的key
            return GetTileDetails(key);
        }

        /// <summary>
        /// 执行实际工具或物品功能
        /// </summary>
        /// <param name="mouseWorldPos">鼠标坐标</param>
        /// <param name="itemDetails">物品信息</param>
        private void OnExecuteActionAfterAnimationEvent(Vector3 mouseWorldPos, ItemDetails itemDetails)
        {
            var mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
            var currentTile = GetTileDetailsOnMousePosition(mouseGridPos);

            if(currentTile != null)
            {
                Crop currentCrop = GetCropObject(mouseWorldPos);

                //WORKFLOW: 物品使用实际功能
                switch (itemDetails.itemType)
                {
                    case ItemType.Seed:
                        EventHandler.CallPlantSeedEvent(itemDetails.itemID, currentTile);
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos, itemDetails.itemType);
                        break;

                    case ItemType.Commodity:
                        EventHandler.CallDropItemEvent(itemDetails.itemID, mouseWorldPos, itemDetails.itemType);
                        break;

                    case ItemType.HoeTool:
                        SetDigGround(currentTile);
                        currentTile.daySinceDug = 0;
                        currentTile.canDig = false;
                        currentTile.canDropItem = false;
                        // 音效
                        break;

                    case ItemType.WaterTool:
                        SetWaterGround(currentTile);
                        currentTile.daySinceWatered = 0;
                        // 音效
                        break;

                    case ItemType.BreakTool:
                    case ItemType.ChopTool:
                        currentCrop?.ProcessToolAction(itemDetails, currentCrop.tileDetails);
                        break;

                    case ItemType.CollectTool:
                        // 执行收割方法
                        currentCrop?.ProcessToolAction(itemDetails, currentTile);
                        break;

                    case ItemType.ReapTool:
                        var reapCount = 0;
                        for (int i = 0; i < itemsInRadius.Count; i++)
                        {
                            EventHandler.CallParticleEffectEvent(ParticaleEffectType.ReapableScenery, itemsInRadius[i].transform.position + Vector3.up);
                            itemsInRadius[i].SpawnHarvestItems();
                            Destroy(itemsInRadius[i].gameObject);

                            reapCount++;
                            if (reapCount >= Settings.reapAmount)
                                break;
                        }
                        break;
                }
                UpdateTileDetails(currentTile);         // 更新字典
            }
        }

        /// <summary>
        /// 通过物理方法判断鼠标点击位置的农作物
        /// </summary>
        /// <param name="mouseWorldPos">鼠标坐标</param>
        /// <returns>农作物信息</returns>
        public Crop GetCropObject(Vector3 mouseWorldPos)
        {
            Collider2D[] colliders = Physics2D.OverlapPointAll(mouseWorldPos);
            Crop currentCrop = null;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Crop>())
                    currentCrop = colliders[i].GetComponent<Crop>();
            }
            return currentCrop;
        }

        /// <summary>
        /// 查找工具范围内符合要求的农作物 查找杂草
        /// </summary>
        /// <param name="tool">当前使用工具</param>
        /// <returns></returns>
        public bool HaveReapableItemsInRadius(Vector3 mouseWorldPos, ItemDetails tool)
        {
            itemsInRadius = new List<ReapItem>();

            Collider2D[] colliders = new Collider2D[20];

            Physics2D.OverlapCircleNonAlloc(mouseWorldPos, tool.itemUseRadius, colliders);

            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i] != null)
                    {
                        if (colliders[i].GetComponent<ReapItem>())
                        {
                            var item = colliders[i].GetComponent<ReapItem>();
                            itemsInRadius.Add(item);
                        }
                    }
                }
            }
            return itemsInRadius.Count > 0;
        }


        /// <summary>
        /// 显示挖坑瓦片
        /// </summary>
        /// <param name="tile"></param>
        private void SetDigGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);
            if (digTilemap != null)
                digTilemap.SetTile(pos, digTile);
        }

        /// <summary>
        /// 显示浇水瓦片
        /// </summary>
        /// <param name="tile"></param>
        private void SetWaterGround(TileDetails tile)
        {
            Vector3Int pos = new Vector3Int(tile.gridX, tile.gridY, 0);
            if (waterTilemap != null)
                waterTilemap.SetTile(pos, waterTile);
        }


        /// <summary>
        /// 执行实际工具或物品功能后，更新瓦片字典
        /// </summary>
        /// <param name="tileDetails"></param>
        public void UpdateTileDetails(TileDetails tileDetails)
        {
            string key = tileDetails.gridX + "x" + tileDetails.gridY + "y" + SceneManager.GetActiveScene().name;
            if(tileDetailsDict.ContainsKey(key))
            {
                tileDetailsDict[key] = tileDetails;
            }
            else
            {
                tileDetailsDict.Add(key, tileDetails);
            }
        }

        /// <summary>
        /// 刷新当前地图，日期相关
        /// </summary>
        private void RefreshMap()
        {
            if (digTilemap != null)
                digTilemap.ClearAllTiles();
            if (waterTilemap != null)
                waterTilemap.ClearAllTiles();

            foreach(var crop in FindObjectsOfType<Crop>())
            {
                Destroy(crop.gameObject);
            }

            DisplayMap(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// 显示地图瓦片
        /// </summary>
        /// <param name="sceneName">场景名称</param>
        private void DisplayMap(string sceneName)
        {
            foreach(var tile in tileDetailsDict)
            {
                var key = tile.Key;
                var tileDetails = tile.Value;

                if(key.Contains(sceneName))
                {
                    if(tileDetails.daySinceDug > -1)
                        SetDigGround(tileDetails);
                    if(tileDetails.daySinceWatered > -1)
                        SetWaterGround(tileDetails);
                    if (tileDetails.seedItemID > -1)
                        EventHandler.CallPlantSeedEvent(tileDetails.seedItemID, tileDetails);
                }
            }
        }
    }
}

