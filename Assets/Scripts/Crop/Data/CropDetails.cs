using UnityEngine;

[System.Serializable]
public class CropDetails
{
    public int seedItemID;
    [Header("��ͬ�׶���Ҫ������")]
    public int[] growthDays;

    public int TotalGrowthDays
    {
        get
        {
            int amount = 0;
            foreach(var days in growthDays)
            {
                amount += days;
            }
            return amount;
        }
    }

    [Header("��ͬ�����׶���ƷPrefab")]
    public GameObject[] growthPrefabs;

    [Header("��ͬ�׶ε�ͼƬ")]
    public Sprite[] growthSprites;

    [Header("����ֲ�ļ���")]
    public Season[] seasons;

    [Space]
    [Header("�ո��")]
    public int[] harvestToolItemID;

    [Header("ÿ�ֹ���ʹ�ô���")]
    public int[] requireActionCount;

    [Header("ת������ƷID")]
    public int transferItemID;

    [Space]
    [Header("�ո��ʵ��Ϣ")]
    public int[] producedItemID;
    public int[] producedMinAmount;
    public int[] producedMaxAmount;
    public Vector2 spawnRadius;             // ��ʵ���ɷ�Χ

    [Header("�ٴ�����ʱ��")]
    public int daysToRegrow;
    public int timesOfRegrow;               // �ظ���������

    [Header("Options")]
    public bool generateAtPlayerPosition;
    public bool hasAnimation;
    public bool hasParticalEffect;
    // TODO:��Ч ��Ч ��

    /// <summary>
    /// ��鵱ǰ�����Ƿ����
    /// </summary>
    /// <param name="toolID">����ID</param>
    /// <returns></returns>
    public bool CheckToolAvailable(int toolID)
    {
        foreach (var tool in harvestToolItemID)
        {
            if (tool == toolID)
                return true;          
        }
        return false;
    }

    /// <summary>
    /// ������Ҫʹ�ù��ߵĴ���
    /// </summary>
    /// <param name="toolID">����ID</param>
    /// <returns></returns>
    public int GetToolTotalRequireCount(int toolID)
    {
        for (int i = 0; i < harvestToolItemID.Length; i++)
        {
            if(harvestToolItemID[i] == toolID)
                return requireActionCount[i];
        }
        return -1;
    }
}
