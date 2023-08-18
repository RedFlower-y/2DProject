using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.CropPlant
{
    public class ReapItem : MonoBehaviour
    {
        private CropDetails cropDetails;
        private Transform PlayerTransform => FindObjectOfType<Player>().transform;

        public void InitCropData(int ID)
        {
            cropDetails = CropManager.Instance.GetCropDetails(ID);
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
                        var spawnPos = new Vector3(transform.position.x + Random.Range(dirX, cropDetails.spawnRadius.x * dirX),
                                                    transform.position.y + Random.Range(-cropDetails.spawnRadius.y, cropDetails.spawnRadius.y),
                                                    0);
                        // �����ͼ��������Ʒ
                        EventHandler.CallInstantiateItemInScene(cropDetails.producedItemID[i], spawnPos);
                    }
                }
            }
        }
    }
}
