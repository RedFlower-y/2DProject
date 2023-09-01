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
            cancelButton.onClick.AddListener(CancelTrade);      // �رս����������
            submitButton.onClick.AddListener(TradeItem);        // ִ�н���
        }

        /// <summary>
        /// �򿪲����ý������鴰��
        /// </summary>
        /// <param name="item">������Ʒ</param>
        /// <param name="isSell">�Ƿ�Ϊ����</param>
        public void SetUpTradeUI(ItemDetails item, bool isSell)
        {
            this.item = item;
            itemIcon.sprite = item.itemIcon;
            itemName.text = item.itemName;
            isSellTrade = isSell;
            tradeAmount.text = string.Empty;
        }

        /// <summary>
        /// ʵ�ʽ�����Ʒ
        /// </summary>
        private void TradeItem()
        {
            var amount = Convert.ToInt32(tradeAmount.text);

            InventoryManager.Instance.TradeItem(item, amount, isSellTrade);

            CancelTrade();
        }

        /// <summary>
        /// �رս��״���
        /// </summary>
        public void CancelTrade()
        {
            this.gameObject.SetActive(false);
        }
    }
}
