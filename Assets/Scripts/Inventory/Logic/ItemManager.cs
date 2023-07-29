using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFarm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrefab;
        private Transform itemParent;

        // ��¼����Item
        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();

        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.DropItemEvent          += OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent  += OnAfterSceneLoadedEvent;
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.DropItemEvent          -= OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent  -= OnAfterSceneLoadedEvent;
        }

        /// <summary>
        /// ��Ʒʵ���������ص�����
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <param name="pos">��Ʒ����λ��</param>
        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
            var item = Instantiate(itemPrefab, pos, Quaternion.identity, itemParent);
            item.itemID = ID;
        }

        /// <summary>
        /// �ӳ���Ʒ
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <param name="pos">��Ʒ�ӳ�����</param>
        private void OnDropItemEvent(int ID, Vector3 pos)
        {
            var item = Instantiate(itemPrefab, pos, Quaternion.identity, itemParent);
            item.itemID = ID;
        }

        /// <summary>
        /// �ɳ���ж��ǰ�����浱ǰ����������Ʒ��Ϣ
        /// </summary>
        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItem();
        }

        /// <summary>
        /// �³������غ󣬼�����Ʒ��Ϣ
        /// </summary>
        private void OnAfterSceneLoadedEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            RecreateAllItem();
        }  

        /// <summary>
        /// ��ȡ��ǰ������ȫ����Ʒ��Ϣ
        /// </summary>
        private void GetAllSceneItem()
        {
            // ����ǰ������������Ʒ���뵽currentSceneItem����ֵ���
            List<SceneItem> currentSceneItems = new List<SceneItem>();
            foreach (var item in FindObjectsOfType<Item>())
            {
                SceneItem sceneItem = new SceneItem
                {
                    itemID = item.itemID,
                    position = new SerializableVector3(item.transform.position),
                };
                currentSceneItems.Add(sceneItem);
            }


            // ��currentSceneItem���µ�sceneItemDict��
            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                // �ҵ���Ӧ�����������ݾ͸���item�����б�
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItems;
            }
            else
            {
                // �Ҳ�����Ӧ�����������ݾʹ����µ�����
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItems);
            }
        }


        /// <summary>
        /// ˢ���ؽ���ǰ������Ʒ
        /// </summary>
        private void RecreateAllItem()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();

            // �����״μ��صĳ�����sceneItemDict��û�ж�Ӧ����
            if (sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItems))
            {
                // ʹ��TryGetValue���Ȳ�����û�ж�Ӧ�����ݣ����򷵻����ݣ�û�еĻ��򷵻�false
                if(currentSceneItems != null)
                {
                    // ��ɾ��������������Ʒ
                    foreach(var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }

                    // �ٰ���sceneItemDict�е�������������
                    foreach(var item in currentSceneItems)
                    {
                        Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                        newItem.Init(item.itemID);
                    }
                }
            }
        }
    }
}

