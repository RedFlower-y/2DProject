using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    public CropDetails cropDetails;
    public TileDetails tileDetails;
    private int harvestActionCount;     // ��¼������ʹ�ô���
    private Animator anim;
    private Transform PlayerTransform => FindObjectOfType<Player>().transform;
    public bool CanHarvest => tileDetails.growthDays >= cropDetails.TotalGrowthDays;    // �ж�ũ�����Ƿ����

    public void ProcessToolAction(ItemDetails tool, TileDetails tile)
    {
        tileDetails = tile;

        int requireActionCount = cropDetails.GetToolTotalRequireCount(tool.itemID);     // ����ʹ�ô���

        if (requireActionCount == -1) return;

        anim = GetComponentInChildren<Animator>();

        // ���������
        if (harvestActionCount < requireActionCount)
        {
            // ����ʹ�ô���������������ȡ
            harvestActionCount++;

            // �ж��Ƿ��ж��� ��ľ
            if (anim != null && cropDetails.hasAnimation)
            {
                if (PlayerTransform.position.x < transform.position.x)
                    anim.SetTrigger("RotateRight");
                else
                    anim.SetTrigger("RotateLeft");
            }
            // ��������
            // ��������
        }

        if (harvestActionCount >= requireActionCount)
        {
            // ���ߴ����㹻������ũ����
            if (cropDetails.generateAtPlayerPosition || !cropDetails.hasAnimation)  // ׷�ӷ��ж�������������Ϊ��׮û�ж��� Ҳ�����ڽ�ɫλ������ũ����
            {
                // ����ũ����
                SpawnHarvestItems();
            }
            else if (cropDetails.hasAnimation)
            {
                // ������
                if (PlayerTransform.position.x < transform.position.x)
                    anim.SetTrigger("FallingRight");
                else
                    anim.SetTrigger("FallingLeft");

                StartCoroutine(HarvestAfterAnimation());
            }
        }
    }

    /// <summary>
    /// ֱ�������굹�¶�����Animator�е�END�׶Σ��������ɹ�ʵ
    /// </summary>
    /// <returns></returns>
    private IEnumerator HarvestAfterAnimation()
    {
        while(!anim.GetCurrentAnimatorStateInfo(0).IsName("END"))
        {
            yield return null;
        }

        // ����ũ����
        SpawnHarvestItems();

        // ת�������� ��ת��Ϊ����
        if (cropDetails.transferItemID > 0)
        {
            CreateTransferCrop();
        }
    }

    /// <summary>
    /// ����ת����������壨�� ��ľ����������ľ׮��
    /// </summary>
    private void CreateTransferCrop()
    {
        tileDetails.seedItemID = cropDetails.transferItemID;
        tileDetails.daySinceLastHarvest = -1;
        tileDetails.growthDays = 0;

        EventHandler.CallRefreshCurrentMap();
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
                else
                {
                    // �ж�Ӧ�����ɵ���Ʒ����
                    var dirX = transform.position.x > PlayerTransform.position.x ? 1 : -1;
                    // һ����Χ�ڵ����
                    var spawnPos = new Vector3( transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                                                transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y), 
                                                0);
                    // �����ͼ��������Ʒ
                    EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);
                }
            }
        }

        if(tileDetails != null)
        {
            tileDetails.daySinceLastHarvest++;

            if (cropDetails.daysToRegrow > 0 && tileDetails.daySinceLastHarvest < cropDetails.timesOfRegrow)
            {
                // �����ظ�����
                tileDetails.growthDays = cropDetails.TotalGrowthDays - cropDetails.daysToRegrow;
              
                EventHandler.CallRefreshCurrentMap();           // ˢ������
            }
            else
            {
                // �������ظ�����
                tileDetails.daySinceLastHarvest = -1;
                tileDetails.seedItemID = -1;

                // ��Ϊ�����ظ��������������ո����ʵ�����ػָ�Ϊδ��״̬
                //tileDetails.daySinceDug = -1;
            }

            Destroy(gameObject);
        }
    }
}
