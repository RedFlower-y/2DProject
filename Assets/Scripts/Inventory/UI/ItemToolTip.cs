using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI    nameText;
    [SerializeField] private TextMeshProUGUI    typeText;
    [SerializeField] private TextMeshProUGUI    descriptionText;
    [SerializeField] private Text               valueText;
    [SerializeField] private GameObject         bottomPart;

    /// <summary>
    /// 物品详情栏内部显示
    /// </summary>
    /// <param name="itemDetails">物品信息</param>
    /// <param name="slotType">物品所处位置（玩家背包？商店？）</param>
    public void SetupToolTip(ItemDetails itemDetails, SlotType slotType)
    {
        nameText.text = itemDetails.itemName;
        typeText.text = GetItemType(itemDetails.itemType);
        descriptionText.text = itemDetails.itemDescription;
        if(itemDetails.itemType == ItemType.Seed || itemDetails.itemType == ItemType.Commodity || itemDetails.itemType == ItemType.Furniture)
        {
            // 当物品为种子，商品，家具，才能买卖（显示价格）
            bottomPart.SetActive(true);
            var price = itemDetails.itemPrice;
            if (slotType == SlotType.Bag)
                price = (int)(price * itemDetails.sellPercentage);
            valueText.text = price.ToString();
        }
        else
        {
            bottomPart.SetActive(false);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());  // 强制刷新介绍栏
    }

    /// <summary>
    /// 物品详情页面里，物品类别的中文转化
    /// </summary>
    /// <param name="itemType">物品类别</param>
    /// <returns></returns>
    private string GetItemType(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Seed => "种子",
            ItemType.Commodity => "商品",
            ItemType.Furniture => "家具",
            ItemType.BreakTool => "工具",
            ItemType.ChopTool => "工具",
            ItemType.CollectTool => "工具",
            ItemType.HoeTool => "工具",
            ItemType.ReapTool => "工具",
            ItemType.WaterTool => "工具",
            _ => "无"
        };
    }

 }
