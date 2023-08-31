using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeUI : MonoBehaviour
{
    public Image itemIcon;
    public Text itemName;
    public InputField tradeAmount;
    public Button submitButton;
    public Button cancelButton;

    private ItemDetails item;
    private bool isSellTrade;

    private void Awake()
    {
        cancelButton.onClick.AddListener(CancelTrade);
    }

    /// <summary>
    /// 打开并设置交易详情窗口
    /// </summary>
    /// <param name="item">交易物品</param>
    /// <param name="isSell">是否为卖出</param>
    public void SetUpTradeUI(ItemDetails item,bool isSell)
    {
        this.item           = item;
        itemIcon.sprite     = item.itemIcon;
        itemName.text       = item.itemName;
        isSellTrade         = isSell;
        tradeAmount.text    = string.Empty;
    }

    public void CancelTrade()
    {
        this.gameObject.SetActive(false);
    }
}
