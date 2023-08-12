using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;
    private TileDetails tileDetails;
    private int harvestActionCount;     // 记录工具已使用次数

    public void ProcessToolAction(ItemDetails tool, TileDetails tile)
    {
        tileDetails = tile;

        int requireActionCount = cropDetails.GetToolTotalRequireCount(tool.itemID);     // 工具使用次数

        if (requireActionCount == -1) return;

        // 判断是否有动画 树木


        // 点击计数器
        if (harvestActionCount < requireActionCount)
        {
            // 工具使用次数不够，继续挖取
            harvestActionCount++;

            // 播放粒子
            // 播放声音
        }

        if (harvestActionCount >= requireActionCount)
        {
            // 工具次数足够，生成农作物
            if (cropDetails.generateAtPlayerPosition)
            {
                // 生成农作物
                SpawnHarvestItems();
            }
        }
    }

    /// <summary>
    /// 生成农作物
    /// </summary>
    public void SpawnHarvestItems()
    {
        for (int i = 0; i < cropDetails.producedItemID.Length; i++)
        {
            int amountToProduce;

            if (cropDetails.producedMinAmount[i] == cropDetails.producedMaxAmount[i])
            {
                // 代表只生成指定数量的果实
                amountToProduce = cropDetails.producedMinAmount[i];
            }
            else
            {
                // 生成随机数量的果实
                amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i] + 1);
            }

            // 执行生成指定数量的物品
            for (int j = 0; j < amountToProduce; j++)
            {
                if (cropDetails.generateAtPlayerPosition)
                {
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                }
                else
                {
                    // 世界地图上生成物品

                }
            }
        }

        if(tileDetails != null)
        {
            tileDetails.daySinceLastHarvest++;

            if (cropDetails.daysToRegrow > 0 && tileDetails.daySinceLastHarvest < cropDetails.timesOfRegrow)
            {
                // 可以重复生长
                tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;
              
                EventHandler.CallRefreshCurrentMap();           // 刷新种子
            }
            else
            {
                // 不可以重复生长
                tileDetails.daySinceLastHarvest = -1;
                tileDetails.seedItemID = -1;

                // 因为不可重复生长，所以在收割完果实后，土地恢复为未挖状态
                //tileDetails.daySinceDug = -1;
            }

            Destroy(gameObject);
        }
    }
}
