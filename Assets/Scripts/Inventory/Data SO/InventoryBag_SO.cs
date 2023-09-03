using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryBag_SO", menuName = "Inventory/InventoryBag")]
public class InventoryBag_SO : ScriptableObject
{
    public List<InventoryItem> itemList;

    /// <summary>
    /// ������ƷID������Ʒ���
    /// </summary>
    /// <param name="ID">��ƷID</param>
    /// <returns></returns>
    public InventoryItem GetInventoryItem(int ID)
    {
        return itemList.Find(i => i.itemID == ID);
    }
}
