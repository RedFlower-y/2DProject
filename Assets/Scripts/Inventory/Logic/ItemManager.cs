using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MFarm.Save;

namespace MFarm.Inventory
{
    public class ItemManager : MonoBehaviour, ISaveable
    {
        public Item itemPrefab;
        public Item bounceItemPrefab;
        private Transform itemParent;

        private Transform playerTransform => FindObjectOfType<Player>().transform;

        public string GUID => GetComponent<DataGUID>().GUID;

        // ��¼����Item
        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();

        // ��¼�����Ҿ�
        private Dictionary<string, List<SceneFurniture>> sceneFurnitureDict = new Dictionary<string, List<SceneFurniture>>();

        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.DropItemEvent          += OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent  += OnAfterSceneLoadedEvent;
            EventHandler.BuildFurnitureEvent    += OnBuildFurnitureEvent;       // ����
            EventHandler.StartNewGameEvent      += OnStartNewGameEvent;
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.DropItemEvent          -= OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent  -= OnAfterSceneLoadedEvent;
            EventHandler.BuildFurnitureEvent    -= OnBuildFurnitureEvent;       // ����
            EventHandler.StartNewGameEvent      -= OnStartNewGameEvent;
        }

        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();
        }

        /// <summary>
        /// ��Ʒʵ���������ص�����
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <param name="pos">��Ʒ����λ��</param>
        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
            var item = Instantiate(bounceItemPrefab, pos, Quaternion.identity, itemParent);
            item.itemID = ID;
            item.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);    // ����������Ʒʱ�����䶯��
        }

        /// <summary>
        /// �ӳ���Ʒ
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <param name="mousePos">��Ʒ�ӳ�����</param>
        /// <param name="itemType">�ӳ���Ʒ����</param>
        private void OnDropItemEvent(int ID, Vector3 mousePos, ItemType itemType)
        {
            // ����ӳ����������ӣ���ִ�к��������������
            if (itemType == ItemType.Seed) return;

            var item = Instantiate(bounceItemPrefab, playerTransform.position, Quaternion.identity, itemParent);    // �ӳ���Ʒʱ���滻ItemPrefabΪBounceItemPrefab
            item.itemID = ID;
            var dir = (mousePos - playerTransform.position).normalized;
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos, dir);
        }

        /// <summary>
        /// �ɳ���ж��ǰ�����浱ǰ����������Ʒ��Ϣ
        /// </summary>
        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItem();
            GetAllSceneFurniture();
        }

        /// <summary>
        /// �³������غ󣬼�����Ʒ��Ϣ
        /// </summary>
        private void OnAfterSceneLoadedEvent()
        {
            itemParent = GameObject.FindWithTag("ItemParent").transform;
            RecreateAllItem();
            RecreateFurniture();
        }

        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(ID);
            var buildItem = Instantiate(bluePrint.buildPrefab, mousePos, Quaternion.identity, itemParent);      // ʵ������

            // �ٵ�����������ʱ�򣬾͸�ֵ�����ӱ��
            if (buildItem.GetComponent<Box>())
            {
                buildItem.GetComponent<Box>().index = InventoryManager.Instance.BoxDataAmount;
                buildItem.GetComponent<Box>().InitBox(buildItem.GetComponent<Box>().index);
            }
        }

        private void OnStartNewGameEvent(int index)
        {
            sceneItemDict.Clear();
            sceneFurnitureDict.Clear();
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
                if (currentSceneItems != null)
                {
                    // ��ɾ��������������Ʒ
                    foreach (var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }

                    // �ٰ���sceneItemDict�е�������������
                    foreach (var item in currentSceneItems)
                    {
                        Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                        newItem.Init(item.itemID);
                    }
                }
            }
        }

        /// <summary>
        /// ��ȡ��ǰ������ȫ���Ҿ���Ϣ
        /// </summary>
        private void GetAllSceneFurniture()
        {
            // ����ǰ���������мҾ߼��뵽currentSceneFurniture����ֵ���
            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();
            foreach (var item in FindObjectsOfType<Furniture>())
            {
                SceneFurniture sceneFurniture = new SceneFurniture
                {
                    itemID = item.itemID,
                    position = new SerializableVector3(item.transform.position),
                };

                if (item.GetComponent<Box>())
                    sceneFurniture.boxIndex = item.GetComponent<Box>().index;

                currentSceneFurniture.Add(sceneFurniture);
            }


            // ��currentSceneFurniture���µ�sceneFurnitureDict��
            if (sceneFurnitureDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                // �ҵ���Ӧ�����������ݾ͸���furniture�����б�
                sceneFurnitureDict[SceneManager.GetActiveScene().name] = currentSceneFurniture;
            }
            else
            {
                // �Ҳ�����Ӧ�����������ݾʹ����µ�����
                sceneFurnitureDict.Add(SceneManager.GetActiveScene().name, currentSceneFurniture);
            }
        }

        /// <summary>
        /// ˢ���ؽ���ǰ�����Ҿ�
        /// </summary>
        private void RecreateFurniture()
        {
            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();

            if (sceneFurnitureDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneFurniture))
            {
                if (currentSceneFurniture != null)
                {
                    foreach (SceneFurniture sceneFurniture in currentSceneFurniture)
                    {
                        BluePrintDetails bluePrint = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(sceneFurniture.itemID);
                        var buildItem = Instantiate(bluePrint.buildPrefab, sceneFurniture.position.ToVector3(), Quaternion.identity, itemParent);      // ʵ������

                        if (buildItem.GetComponent<Box>())
                        {
                            // ����index��ŵ����ӣ�ֱ�ӻ�ȡԭ��index���
                            buildItem.GetComponent<Box>().InitBox(sceneFurniture.boxIndex);
                        }
                    }
                }
            }
        }

        public GameSaveData GenerateSaveData()
        {
            // ֻ���ڻ�����ʱ���Ż��ȡȫ�����ݣ�������Ҫ�Ȼ�ȡȫ�����ݣ���֤����δ������ʱ��������
            GetAllSceneItem();
            GetAllSceneFurniture();

            GameSaveData saveData = new GameSaveData();
            saveData.sceneItemDict = this.sceneItemDict;
            saveData.sceneFurnitureDict = this.sceneFurnitureDict;

            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            this.sceneItemDict = saveData.sceneItemDict;
            this.sceneFurnitureDict = saveData.sceneFurnitureDict;

            RecreateAllItem();
            RecreateFurniture();
        }
    }
}

