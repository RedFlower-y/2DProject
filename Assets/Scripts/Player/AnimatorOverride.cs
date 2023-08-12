using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Inventory;

public class AnimatorOverride : MonoBehaviour
{
    private Animator[] animators;

    public SpriteRenderer holdItem;

    [Header("各部分动画列表")]
    public List<AnimatorType> animatorTypes;

    private Dictionary<string, Animator> animatorNameDict = new Dictionary<string, Animator>();

    private void Awake()
    {
        animators = GetComponentsInChildren<Animator>();
        foreach(var anim in  animators)
        {
            animatorNameDict.Add(anim.name, anim);
        }
    }

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent          += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent     += OnBeforeSceneUnloadEvent;
        EventHandler.HarvestAtPlayerPosition    += OnHarvestAtPlayerPosition;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent          -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent     -= OnBeforeSceneUnloadEvent;
        EventHandler.HarvestAtPlayerPosition    -= OnHarvestAtPlayerPosition;
    }

    private void OnHarvestAtPlayerPosition(int ID)
    {
        Sprite itemSprite = InventoryManager.Instance.GetItemDetails(ID).itemOnWorldSprite;
        if(holdItem.enabled == false)
        {
            // 避免连续获取果实时的 协程问题（还在上一个协程中就启动下一个协程）
            StartCoroutine(ShowItem(itemSprite));
        }
    }

    /// <summary>
    ///  展示获取的果实
    /// </summary>
    /// <param name="itemSprite">果实图片</param>
    /// <returns></returns>
    private IEnumerator ShowItem(Sprite itemSprite)
    {
        holdItem.sprite = itemSprite;
        holdItem.enabled = true;
        yield return new WaitForSeconds(Settings.waitTimeForHarvest);
        holdItem.enabled = false;
    }

    /// <summary>
    /// 切换场景
    /// </summary>
    private void OnBeforeSceneUnloadEvent()
    {
        // 在切换场景后恢复默认动作
        holdItem.enabled = false;
        SwitchAnimator(PartType.None);
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        // WORKFLOW: 不同工具返回不同的动画在这里补全
        PartType currentType = itemDetails.itemType switch
        {
            ItemType.Seed       => PartType.Carry,
            ItemType.Commodity  => PartType.Carry,
            ItemType.HoeTool    => PartType.Hoe,
            ItemType.ChopTool   => PartType.Chop,
            ItemType.WaterTool  => PartType.Water,
            ItemType.CollectTool=> PartType.Collect,
            _                   => PartType.None,
        };

        if(isSelected == false)
        {
            currentType = PartType.None;
            holdItem.enabled = false;
        }
        else
        {
            if(currentType == PartType.Carry)
            {
                holdItem.sprite = itemDetails.itemOnWorldSprite;
                holdItem.enabled = true;
            }
            else
            {
                holdItem.enabled = false;
            }
        }

        SwitchAnimator(currentType);
    }


    private void SwitchAnimator(PartType partType)
    {
        foreach(var item in animatorTypes)
        {
            if(item.partType == partType)
            {
                animatorNameDict[item.partName.ToString()].runtimeAnimatorController = item.overrideController;
            }
        }
    }
}
