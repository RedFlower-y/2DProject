using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BluePrintList_SO", menuName = "Inventory/BluePrintList")]
public class BluePrintList_SO : ScriptableObject
{
    public List<BluePrintDetails> bluePrintDataList;

    /// <summary>
    /// 根据物品ID获取对应蓝图数据
    /// </summary>
    /// <param name="itemID"></param>
    /// <returns></returns>
    public BluePrintDetails GetBluePrintDetails(int itemID)
    {
        return bluePrintDataList.Find(b => b.ID == itemID);
    }
}

[System.Serializable]
public class BluePrintDetails
{
    public int ID;                                                  // 物品ID
    public InventoryItem[] resourceItem = new InventoryItem[3];     // 最大合成材料数 
    public GameObject buildPrefab;                                  // 合成后Prefab
}