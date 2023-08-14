using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

namespace MFarm.Inventory
{
    public class SlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("�����ȡ")]
        // �˴�ʹ��[SerializeField]ԭ��Inspector�����л�ȡ�����Awake�л�ȡҪ��
        [SerializeField] private Image slotImage;
        [SerializeField] private TextMeshProUGUI amountText;
        public Image slotHighlight;
        [SerializeField] private Button button;

        [Header("��������")]
        public SlotType slotType;
        public bool isSelected;
        public int slotIndex;

        // ��Ʒ��Ϣ
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
        /// ���¸���UI����Ϣ
        /// </summary>
        /// <param name="item">ItemDetails</param>
        /// <param name="amount">��������</param>
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
        /// ��Slot����Ϊ��
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
        /// ���������ʾ
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            if (itemDetails == null)
                return;
            isSelected = !isSelected;
            //slotHighlight.gameObject.SetActive(isSelected);

            inventoryUI.UpdateSlotHighlight(slotIndex);

            // ���ѡ����Ʒ��֪ͨPlayer��Arm�����ı䣬����о���Ʒ��״̬
            if(slotType == SlotType.Bag)
            {
                // ��֤ѡ�е���Ʒ�Ǳ�����Ʒ��������̵��򲻸ı�Player�Ķ���
                EventHandler.CallItemSelectedEvent(itemDetails, isSelected);
            }
        }      

       /// <summary>
       /// ��ק��Ʒ��ʼ
       /// </summary>
       /// <param name="eventData"></param>
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(itemAmount != 0)
            {
                inventoryUI.dragItem.enabled = true;
                inventoryUI.dragItem.sprite = slotImage.sprite;
                inventoryUI.dragItem.SetNativeSize();           // ��ֹ��קͼƬ����Ĭ�ϳߴ�ʱ��ʧ��

                // ��ק��Ʒ���и���
                isSelected = true;
                inventoryUI.UpdateSlotHighlight(slotIndex);
            }
        }

        /// <summary>
        /// ��ק��Ʒ������
        /// </summary>
        /// <param name="eventData"></param>
        public void OnDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.transform.position = Input.mousePosition;      // �������λ���ƶ�
        }

        /// <summary>
        /// ��ק��Ʒ����
        /// </summary>
        /// <param name="eventData"></param>
        public void OnEndDrag(PointerEventData eventData)
        {
            inventoryUI.dragItem.enabled = false;
            //Debug.Log(eventData.pointerCurrentRaycast.gameObject);   // �鿴��ק��Ʒ�������λ��

            if(eventData.pointerCurrentRaycast.gameObject != null)
            {
                // ȷ����ק������λ����UI����
                if (eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>() == null)
                    return;
                var targetSlot = eventData.pointerCurrentRaycast.gameObject.GetComponent<SlotUI>();
                int targetIndex = targetSlot.slotIndex;

                // ��Player��������Χ�ڽ���
                if(slotType == SlotType.Bag && targetSlot.slotType == SlotType.Bag)
                {
                    InventoryManager.Instance.SwapItem(slotIndex, targetIndex);
                }

                // ������и�����ʾ
                inventoryUI.UpdateSlotHighlight(-1);
            }
            //else
            //{
            //    // �������ڵ���
            //    if(itemDetails.canDropped)
            //    {
            //        // ����Ӧ�������ͼ����
            //        var pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

            //        EventHandler.CallInstantiateItemInScene(itemDetails.itemID, pos);
            //    }
                
            //}
        }
    }
}

