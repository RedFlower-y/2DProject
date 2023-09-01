using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MFarm.Inventory
{
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
            cancelButton.onClick.AddListener(CancelTrade);      // 关闭交易详情界面
            submitButton.onClick.AddListener(TradeItem);        // 执行交易
        }

        /// <summary>
        /// 打开并设置交易详情窗口
        /// </summary>
        /// <param name="item">交易物品</param>
        /// <param name="isSell">是否为卖出</param>
        public void SetUpTradeUI(ItemDetails item, bool isSell)
        {
            this.item = item;
            itemIcon.sprite = item.itemIcon;
            itemName.text = item.itemName;
            isSellTrade = isSell;
            tradeAmount.text = string.Empty;
        }

        /// <summary>
        /// 实际交易物品
        /// </summary>
        private void TradeItem()
        {
            var amount = Convert.ToInt32(tradeAmount.text);

            InventoryManager.Instance.TradeItem(item, amount, isSellTrade);

            CancelTrade();
        }

        /// <summary>
        /// 关闭交易窗口
        /// </summary>
        public void CancelTrade()
        {
            this.gameObject.SetActive(false);
        }
    }
}
