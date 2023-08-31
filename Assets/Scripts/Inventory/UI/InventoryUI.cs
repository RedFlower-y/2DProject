using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MFarm.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        public ItemToolTip itemToolTip;

        [Header("��קͼƬ")]
        public Image dragItem;

        [Header("��ұ���UI")]
        [SerializeField] private GameObject bagUI;
        private bool bagOpened;

        [Header("ͨ�ñ���UI")]
        [SerializeField] private GameObject baseBag;
        public GameObject shopSlotPrefab;

        [Header("����UI")]
        public TradeUI tradeUI;

        [SerializeField] private SlotUI[] playerSlots;
        [SerializeField] private List<SlotUI> baseBagSlots;


        private void OnEnable()
        {
            EventHandler.UpdateInventoryUI      += OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent       += OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent      += OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI            += OnShowTradeUI;
        }

        private void OnDisable()
        {
            EventHandler.UpdateInventoryUI      -= OnUpdateInventoryUI;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.BaseBagOpenEvent       -= OnBaseBagOpenEvent;
            EventHandler.BaseBagCloseEvent      -= OnBaseBagCloseEvent;
            EventHandler.ShowTradeUI            -= OnShowTradeUI;
        }


        private void Start()
        {
            // ��ÿһ���������
            for(int i = 0; i < playerSlots.Length; i++)
            {
                playerSlots[i].slotIndex = i;
            }

            bagOpened = bagUI.activeInHierarchy;
        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.B))
                OpenBagUI();
        }

        private void OnShowTradeUI(ItemDetails item, bool isSell)
        {
            tradeUI.gameObject.SetActive(true);
            tradeUI.SetUpTradeUI(item, isSell);
        }

        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bag_SO)
        {
            // TODO: �������prefab
            GameObject prefab = slotType switch
            {
                SlotType.Shop => shopSlotPrefab,
                _ => null,
            };

            // ���ɱ���UI
            baseBag.SetActive(true);

            baseBagSlots = new List<SlotUI>();

            for (int i = 0; i < bag_SO.itemList.Count; i++)
            {
                var slot = Instantiate(prefab, baseBag.transform.GetChild(0)).GetComponent<SlotUI>();
                slot.slotIndex = i;
                baseBagSlots.Add(slot);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(baseBag.GetComponent<RectTransform>()); // ǿ��ˢ��UI

            // ͬʱ����ұ���
            if (slotType == SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(-1, 0.5f);
                bagUI.SetActive(true);
                bagOpened = true;
            }

            // ����UI��ʾ
            OnUpdateInventoryUI(InventoryLocation.Box, bag_SO.itemList);
        }

        private void OnBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bag_SO)
        {
            baseBag.SetActive(false);
            itemToolTip.gameObject.SetActive(false);
            UpdateSlotHighlight(-1);

            foreach(var slot in baseBagSlots)
            {
                Destroy(slot.gameObject);
            }
            baseBagSlots.Clear();

            // ͬʱ�ر���ұ���
            if (slotType == SlotType.Shop)
            {
                bagUI.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                bagUI.SetActive(false);
                bagOpened = false;
            }
        }

        /// <summary>
        /// �л�����
        /// </summary>
        private void OnBeforeSceneUnloadEvent()
        {
            // ȡ������ѡ��
            UpdateSlotHighlight(-1);
        }

        /// <summary>
        /// ����ָ��λ�õ�Slot�¼�
        /// </summary>
        /// <param name="location">�������</param>
        /// <param name="list">�����б�</param>
        private void OnUpdateInventoryUI(InventoryLocation location, List<InventoryItem> list)
        {
            switch(location)
            {
                case InventoryLocation.Player:
                    for (int i = 0; i < playerSlots.Length; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            playerSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            playerSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
                case InventoryLocation.Box:
                    for (int i = 0; i < baseBagSlots.Count; i++)
                    {
                        if (list[i].itemAmount > 0)
                        {
                            var item = InventoryManager.Instance.GetItemDetails(list[i].itemID);
                            baseBagSlots[i].UpdateSlot(item, list[i].itemAmount);
                        }
                        else
                        {
                            baseBagSlots[i].UpdateEmptySlot();
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// �򿪹رձ���UI��Button�����¼�
        /// </summary>
        public void OpenBagUI()
        {
            bagOpened = !bagOpened;
            bagUI.SetActive(bagOpened);
        }

        /// <summary>
        /// ���¸�����ʾ
        /// </summary>
        /// <param name="index">���</param>
        public void UpdateSlotHighlight(int index)
        {
            foreach (var slot in playerSlots)
            {
                if(slot.isSelected && slot.slotIndex == index)
                {
                    slot.slotHighlight.gameObject.SetActive(true);
                }
                else
                {
                    slot.isSelected = false;
                    slot.slotHighlight.gameObject.SetActive(false);
                }
            }
        }
    }
}

