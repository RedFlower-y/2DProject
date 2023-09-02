using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MFarm.Inventory;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI    nameText;
    [SerializeField] private TextMeshProUGUI    typeText;
    [SerializeField] private TextMeshProUGUI    descriptionText;
    [SerializeField] private Text               valueText;
    [SerializeField] private GameObject         bottomPart;
    [SerializeField] private Image[]            resourceItem;
    public GameObject resourcePanel;

    /// <summary>
    /// ��Ʒ�������ڲ���ʾ
    /// </summary>
    /// <param name="itemDetails">��Ʒ��Ϣ</param>
    /// <param name="slotType">��Ʒ����λ�ã���ұ������̵ꣿ��</param>
    public void SetupToolTip(ItemDetails itemDetails, SlotType slotType)
    {
        nameText.text = itemDetails.itemName;
        typeText.text = GetItemType(itemDetails.itemType);
        descriptionText.text = itemDetails.itemDescription;
        if(itemDetails.itemType == ItemType.Seed || itemDetails.itemType == ItemType.Commodity || itemDetails.itemType == ItemType.Furniture)
        {
            // ����ƷΪ���ӣ���Ʒ���Ҿߣ�������������ʾ�۸�
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
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());  // ǿ��ˢ�½�����
    }

    /// <summary>
    /// ��Ʒ����ҳ�����Ʒ��������ת��
    /// </summary>
    /// <param name="itemType">��Ʒ���</param>
    /// <returns></returns>
    private string GetItemType(ItemType itemType)
    {
        return itemType switch
        {
            ItemType.Seed           => "����",
            ItemType.Commodity      => "��Ʒ",
            ItemType.Furniture      => "�Ҿ�",
            ItemType.BreakTool      => "����",
            ItemType.ChopTool       => "����",
            ItemType.CollectTool    => "����",
            ItemType.HoeTool        => "����",
            ItemType.ReapTool       => "����",
            ItemType.WaterTool      => "����",
            _                       => "��"
        };
    }

    /// <summary>
    /// ��ͼUI �� ʵ��
    /// </summary>
    /// <param name="ID"></param>
    public void SetupResourcePanel(int ID)
    {
        var bluePrintDetails = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(ID);

        for (int i = 0; i < resourceItem.Length; i++)
        {
            // ����UI����ͼUI�ĺϳ��������������3�������о�
            if (i < bluePrintDetails.resourceItem.Length)
            {       
                var item = bluePrintDetails.resourceItem[i];
                resourceItem[i].gameObject.SetActive(true);
                resourceItem[i].sprite = InventoryManager.Instance.GetItemDetails(item.itemID).itemIcon;
                resourceItem[i].transform.GetChild(0).GetComponent<Text>().text = item.itemAmount.ToString();
            }
            else
            {
                // ʵ��ԭ��������3��������� �رն������ͼUI�ĺϳ���
                resourceItem[i].gameObject.SetActive(false);
            }
        }
    }
 }
