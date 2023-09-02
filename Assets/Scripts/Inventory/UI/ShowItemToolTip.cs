using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MFarm.Inventory
{
    [RequireComponent(typeof(SlotUI))]  //保证身上一定挂载SlotUI组件
    public class ShowItemToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private SlotUI slotUI;
        private InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();    

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
            //inventoryUI = GetComponent<InventoryUI>();
        }

        /// <summary>
        /// 鼠标移至对应物品上，显示物品详情栏
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotUI.itemDetails != null)
            {
                //Debug.Log(eventData.pointerCurrentRaycast.gameObject);
                inventoryUI.itemToolTip.gameObject.SetActive(true);
                inventoryUI.itemToolTip.SetupToolTip(slotUI.itemDetails, slotUI.slotType);

                inventoryUI.itemToolTip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);     //  修改介绍文本框的锚点位置，使其从中心变为中下
                inventoryUI.itemToolTip.transform.position = transform.position + Vector3.up * 60;

                // 家具的合成
                if(slotUI.itemDetails.itemType == ItemType.Furniture)
                {
                    // 是图纸，开启蓝图合成栏
                    inventoryUI.itemToolTip.resourcePanel.SetActive(true);
                    inventoryUI.itemToolTip.SetupResourcePanel(slotUI.itemDetails.itemID);
                }
                else
                {
                    // 不是图纸，关闭蓝图合成栏
                    inventoryUI.itemToolTip.resourcePanel.SetActive(false);
                }
            }
            else
            {
                inventoryUI.itemToolTip.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 鼠标移出对应物品时，关闭物品详情栏
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.itemToolTip.gameObject.SetActive(false);
        }
    }
}

