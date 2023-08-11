using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;
    private int harvestActionCount;     // ��¼������ʹ�ô���

    public void ProcessToolAction(ItemDetails tool)
    {
        int requireActionCount = cropDetails.GetToolTotalRequireCount(tool.itemID);     // ����ʹ�ô���

        if (requireActionCount == -1) return;

        // �ж��Ƿ��ж��� ��ľ


        // ���������
        if (harvestActionCount < requireActionCount)
        {
            // ����ʹ�ô���������������ȡ
            harvestActionCount++;

            // ��������
            // ��������
        }

        if (harvestActionCount >= requireActionCount)
        {
            // ���ߴ����㹻������ũ����
            if (cropDetails.generateAtPlayerPosition)
            {
                // ����ũ����
                SpawnHarvestItems();
            }
        }
    }

    /// <summary>
    /// ����ũ����
    /// </summary>
    public void SpawnHarvestItems()
    {
        for (int i = 0; i < cropDetails.producedItemID.Length; i++)
        {
            int amountToProduce;

            if (cropDetails.producedMinAmount[i] == cropDetails.producedMaxAmount[i])
            {
                // ����ֻ����ָ�������Ĺ�ʵ
                amountToProduce = cropDetails.producedMinAmount[i];
            }
            else
            {
                // ������������Ĺ�ʵ
                amountToProduce = Random.Range(cropDetails.producedMinAmount[i], cropDetails.producedMaxAmount[i] + 1);
            }

            // ִ������ָ����������Ʒ
            for (int j = 0; j < amountToProduce; j++)
            {
                if (cropDetails.generateAtPlayerPosition)
                {
                    EventHandler.CallHarvestAtPlayerPosition(cropDetails.producedItemID[i]);
                }
            }
        }
    }
}
