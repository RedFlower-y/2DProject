using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MFarm.Inventory
{
    [RequireComponent(typeof(SlotUI))]  //��֤����һ������SlotUI���
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
        /// ���������Ӧ��Ʒ�ϣ���ʾ��Ʒ������
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (slotUI.itemDetails != null)
            {
                //Debug.Log(eventData.pointerCurrentRaycast.gameObject);
                inventoryUI.itemToolTip.gameObject.SetActive(true);
                inventoryUI.itemToolTip.SetupToolTip(slotUI.itemDetails, slotUI.slotType);

                inventoryUI.itemToolTip.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);     //  �޸Ľ����ı����ê��λ�ã�ʹ������ı�Ϊ����
                inventoryUI.itemToolTip.transform.position = transform.position + Vector3.up * 60;

                // �Ҿߵĺϳ�
                if(slotUI.itemDetails.itemType == ItemType.Furniture)
                {
                    // ��ͼֽ��������ͼ�ϳ���
                    inventoryUI.itemToolTip.resourcePanel.SetActive(true);
                    inventoryUI.itemToolTip.SetupResourcePanel(slotUI.itemDetails.itemID);
                }
                else
                {
                    // ����ͼֽ���ر���ͼ�ϳ���
                    inventoryUI.itemToolTip.resourcePanel.SetActive(false);
                }
            }
            else
            {
                inventoryUI.itemToolTip.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// ����Ƴ���Ӧ��Ʒʱ���ر���Ʒ������
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryUI.itemToolTip.gameObject.SetActive(false);
        }
    }
}

