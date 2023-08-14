using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;
    public TileDetails tileDetails;
    private int harvestActionCount;     // 记录工具已使用次数
    private Animator anim;
    private Transform PlayerTransform => FindObjectOfType<Player>().transform;
    public bool CanHarvest => tileDetails.growthDays >= cropDetails.TotalGrowthDays;    // 判断农作物是否成熟

    public void ProcessToolAction(ItemDetails tool, TileDetails tile)
    {
        tileDetails = tile;

        int requireActionCount = cropDetails.GetToolTotalRequireCount(tool.itemID);     // 工具使用次数

        if (requireActionCount == -1) return;

        anim = GetComponentInChildren<Animator>();

        // 点击计数器
        if (harvestActionCount < requireActionCount)
        {
            // 工具使用次数不够，继续挖取
            harvestActionCount++;

            // 判断是否有动画 树木
            if (anim != null && cropDetails.hasAnimation)
            {
                if (PlayerTransform.position.x < transform.position.x)
                    anim.SetTrigger("RotateRight");
                else
                    anim.SetTrigger("RotateLeft");
            }
            // 播放粒子
            // 播放声音
        }

        if (harvestActionCount >= requireActionCount)
        {
            // 工具次数足够，生成农作物
            if (cropDetails.generateAtPlayerPosition || !cropDetails.hasAnimation)  // 追加非有动画条件，是因为树桩没有动画 也不会在角色位置生成农作物
            {
                // 生成农作物
                SpawnHarvestItems();
            }
            else if (cropDetails.hasAnimation)
            {
                // 树倒下
                if (PlayerTransform.position.x < transform.position.x)
                    anim.SetTrigger("FallingRight");
                else
                    anim.SetTrigger("FallingLeft");

                StartCoroutine(HarvestAfterAnimation());
            }
        }
    }

    /// <summary>
    /// 直到播放完倒下动画（Animator中的END阶段），才生成果实
    /// </summary>
    /// <returns></returns>
    private IEnumerator HarvestAfterAnimation()
    {
        while(!anim.GetCurrentAnimatorStateInfo(0).IsName("END"))
        {
            yield return null;
        }

        // 生成农作物
        SpawnHarvestItems();

        // 转换新物体 树转换为树根
        if (cropDetails.transferItemID > 0)
        {
            CreateTransferCrop();
        }
    }

    /// <summary>
    /// 生成转换后的新物体（例 树木砍倒后留下木桩）
    /// </summary>
    private void CreateTransferCrop()
    {
        tileDetails.seedItemID = cropDetails.transferItemID;
        tileDetails.daySinceLastHarvest = -1;
        tileDetails.growthDays = 0;

        EventHandler.CallRefreshCurrentMap();
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
                    // 判断应该生成的物品方向
                    var dirX = transform.position.x > PlayerTransform.position.x ? 1 : -1;
                    // 一定范围内的随机
                    var spawnPos = new Vector3( transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                                                transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 
                                                0);
                    // 世界地图上生成物品
                    EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);
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
