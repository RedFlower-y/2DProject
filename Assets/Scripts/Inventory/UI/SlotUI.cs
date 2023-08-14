using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace MFarm.Inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("组件获取")]
        // 此处使用[SerializeField]原因：Inspector窗口中获取会比在Awake中获取要快
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;
        public Image slotHighlight;
        [SerializeField] private Button button;

        [Header("格子类型")]
        public SlotType slotType;
        public bool isSelected;
        public int slotIndex;

        // 物品信息
        public ItemDetails itemDetails;
        public int itemAmount;

        public InventoryUI inventoryUI => GetComponentInParent<InventoryUI>();

        private void Start()
        {
            isSelected = false;
            if (itemDetails == null)
            {
                UpdateEmptySlot();
            }
        }

        /// <summary>
        /// 更新格子UI和信息
        /// </summary>
        /// <param name="item">ItemDetails</param>
        /// <param name="amount">持有数量</param>
        public void UpdateSlot(ItemDetails item, int amount)
        {
            itemDetails = item;
            slotImage.sprite = item.itemIcon;
            itemAmount = amount;
            amountText.text = amount.ToString();
            slotImage.enabled = true;
            button.interactable = true;
        }

        /// <summary>
        /// 将Slot更新为空
        /// </summary>
        public void UpdateEmptySlot()
        {
            if (isSelected)
            {
                isSelected = false;
                inventoryUI.UpdateSlotHighlight(-1);
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }

            itemDetails = null;
            slotImage.enabled = false;
            amountText.text = string.Empty;
            button.interactable = false;
        }

        /// <summary>
        /// 点击高亮显示
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails == null)
                return;
            isSelected = !isSelected;
            //slotHighlight.gameObject.SetActive(isSelected);

            inventoryUI.UpdateSlotHighlight(slotIndex);

            // 如果选中物品，通知Player的Arm动画改变，变成托举物品的状态
            if(slotType == SlotType.Bag)
            {
                // 保证选中的物品是背包物品，如果是商店则不改变Player的动画
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
        }      

       /// <summary>
       /// 拖拽物品开始
       /// </summary>
       /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(itemAmount != 0)
            {
                inventoryUI.dragItem.enabled = true;
                inventoryUI.dragItem.sprite = slotImage.sprite;
                inventoryUI.dragItem.SetNativeSize();           // 防止拖拽图片大于默认尺寸时的失真

                // 拖拽物品进行高亮
                isSelected = true;
                inventoryUI.UpdateSlotHighlight(slotIndex);
            }
        }

        /// <summary>
        /// 拖拽物品过程中
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.transform.position = Input.mousePosition;      // 跟随鼠标位置移动
        }

        /// <summary>
        /// 拖拽物品结束
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.enabled = false;
            //Debug.Log(eventData.pointerCurrentRaycast.gameObject);   // 查看拖拽物品结束后的位置

            if(eventData.pointerCurrentRaycast.gameObject != null)
            {
                // 确认拖拽结束的位置是UI部分
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                    return;
                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targetIndex = targetSlot.slotIndex;

                // 在Player自身背包范围内交换
                if(slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                }

                // 清空所有高亮显示
                inventoryUI.UpdateSlotHighlight(-1);
            }
            //else
            //{
            //    // 测试扔在地上
            //    if(itemDetails.canDropped)
            //    {
            //        // 鼠标对应的世界地图坐标
            //        var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

            //        EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
            //    }
                
            //}
        }
    }
}

