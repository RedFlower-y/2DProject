using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BluePrintList_SO", menuName = "Inventory/BluePrintList")]
public class BluePrintList_SO : ScriptableObject
{
    public List<BluePrintDetails> bluePrintDataList;

    /// <summary>
    /// ������ƷID��ȡ��Ӧ��ͼ����
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
    public int ID;                                                  // ��ƷID
    public InventoryItem[] resourceItem = new InventoryItem[3];     // ���ϳɲ����� 
    public GameObject buildPrefab;                                  // �ϳɺ�Prefab
}