using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MFarm.Inventory
{
    public class ItemManager : MonoBehaviour
    {
        public Item itemPrefab;
        public Item bounceItemPrefab;
        private Transform itemParent;

        private Transform playerTransform => FindObjectOfType<Player>().transform;

        // 记录场景Item
        private Dictionary<string, List<SceneItem>> sceneItemDict = new Dictionary<string, List<SceneItem>>();

        // 记录场景家具
        private Dictionary<string, List<SceneFurniture>> sceneFurnitureDict = new Dictionary<string, List<SceneFurniture>>();

        private void OnEnable()
        {
            EventHandler.InstantiateItemInScene += OnInstantiateItemInScene;
            EventHandler.DropItemEvent          += OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent  += OnAfterSceneLoadedEvent;
            EventHandler.BuildFurnitureEvent    += OnBuildFurnitureEvent;       // 建造
        }

        private void OnDisable()
        {
            EventHandler.InstantiateItemInScene -= OnInstantiateItemInScene;
            EventHandler.DropItemEvent          -= OnDropItemEvent;
            EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
            EventHandler.AfterSceneLoadedEvent  -= OnAfterSceneLoadedEvent;
            EventHandler.BuildFurnitureEvent    -= OnBuildFurnitureEvent;       // 建造
        }

        

        /// <summary>
        /// 物品实例化，加载到场景
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="pos">物品加载位置</param>
        private void OnInstantiateItemInScene(int ID, Vector3 pos)
        {
            var item = Instantiate(bounceItemPrefab, pos, Quaternion.identity, itemParent);
            item.itemID = ID;
            item.GetComponent<ItemBounce>().InitBounceItem(pos, Vector3.up);    // 砍树掉落物品时，下落动画
        }

        /// <summary>
        /// 扔出物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="mousePos">物品扔出坐标</param>
        /// <param name="itemType">扔出物品类型</param>
        private void OnDropItemEvent(int ID, Vector3 mousePos, ItemType itemType)
        {
            // 如果扔出来的是种子，则不执行后面的生成物体了
            if (itemType == ItemType.Seed) return;

            var item = Instantiate(bounceItemPrefab, playerTransform.position, Quaternion.identity, itemParent);    // 扔出物品时则替换ItemPrefab为BounceItemPrefab
            item.itemID = ID;
            var dir = (mousePos - playerTransform.position).normalized;
            item.GetComponent<ItemBounce>().InitBounceItem(mousePos, dir);
        }

        /// <summary>
        /// 旧场景卸载前，保存当前场景所有物品信息
        /// </summary>
        private void OnBeforeSceneUnloadEvent()
        {
            GetAllSceneItem();
            GetAllSceneFurniture();
        }

        /// <summary>
        /// 新场景加载后，加载物品信息
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
            var buildItem = Instantiate(bluePrint.buildPrefab, mousePos, Quaternion.identity, itemParent);      // 实际生成
        }

        /// <summary>
        /// 获取当前场景的全部物品信息
        /// </summary>
        private void GetAllSceneItem()
        {
            // 将当前场景的所有物品加入到currentSceneItem这个字典中
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


            // 将currentSceneItem更新到sceneItemDict中
            if (sceneItemDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                // 找到对应场景名的数据就更新item数据列表
                sceneItemDict[SceneManager.GetActiveScene().name] = currentSceneItems;
            }
            else
            {
                // 找不到对应场景名的数据就创建新的数据
                sceneItemDict.Add(SceneManager.GetActiveScene().name, currentSceneItems);
            }
        }


        /// <summary>
        /// 刷新重建当前场景物品
        /// </summary>
        private void RecreateAllItem()
        {
            List<SceneItem> currentSceneItems = new List<SceneItem>();

            // 存在首次加载的场景在sceneItemDict中没有对应数据
            if (sceneItemDict.TryGetValue(SceneManager.GetActiveScene().name, out currentSceneItems))
            {
                // 使用TryGetValue，先查找有没有对应的数据，有则返回数据，没有的话则返回false
                if (currentSceneItems != null)
                {
                    // 先删除场景中所有物品
                    foreach (var item in FindObjectsOfType<Item>())
                    {
                        Destroy(item.gameObject);
                    }

                    // 再按照sceneItemDict中的数据重新生成
                    foreach (var item in currentSceneItems)
                    {
                        Item newItem = Instantiate(itemPrefab, item.position.ToVector3(), Quaternion.identity, itemParent);
                        newItem.Init(item.itemID);
                    }
                }
            }
        }

        /// <summary>
        /// 获取当前场景的全部家具信息
        /// </summary>
        private void GetAllSceneFurniture()
        {
            // 将当前场景的所有家具加入到currentSceneFurniture这个字典中
            List<SceneFurniture> currentSceneFurniture = new List<SceneFurniture>();
            foreach (var item in FindObjectsOfType<Furniture>())
            {
                SceneFurniture sceneFurniture = new SceneFurniture
                {
                    itemID = item.itemID,
                    position = new SerializableVector3(item.transform.position),
                };
                currentSceneFurniture.Add(sceneFurniture);
            }


            // 将currentSceneFurniture更新到sceneFurnitureDict中
            if (sceneFurnitureDict.ContainsKey(SceneManager.GetActiveScene().name))
            {
                // 找到对应场景名的数据就更新furniture数据列表
                sceneFurnitureDict[SceneManager.GetActiveScene().name] = currentSceneFurniture;
            }
            else
            {
                // 找不到对应场景名的数据就创建新的数据
                sceneFurnitureDict.Add(SceneManager.GetActiveScene().name, currentSceneFurniture);
            }
        }

        /// <summary>
        /// 刷新重建当前场景家具
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
                        OnBuildFurnitureEvent(sceneFurniture.itemID, sceneFurniture.position.ToVector3());
                    }
                }
            }
        }
    }
}

