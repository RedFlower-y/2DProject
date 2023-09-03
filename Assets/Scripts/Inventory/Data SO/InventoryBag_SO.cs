using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryBag_SO", menuName = "Inventory/InventoryBag")]
public class InventoryBag_SO : ScriptableObject
{
    public List<InventoryItem> itemList;

    /// <summary>
    /// 根据物品ID返回物品库存
    /// </summary>
    /// <param name="ID">物品ID</param>
    /// <returns></returns>
    public InventoryItem GetInventoryItem(int ID)
    {
        return itemList.Find(i => i.itemID == ID);
    }
}
