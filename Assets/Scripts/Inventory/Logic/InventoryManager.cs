using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Save;

namespace MFarm.Inventory
{
    public class InventoryManager : Singloten<InventoryManager>, ISaveable
    {
        [Header("商品数据")]
        public ItemDataList_SO itemDataList_SO;

        [Header("建造蓝图")]
        public BluePrintList_SO bluePrintData;

        [Header("背包数据")]
        public InventoryBag_SO playerBagTemp;
        public InventoryBag_SO playerBag;
        private InventoryBag_SO currentBoxBag;

        [Header("交易")]
        public int playerMoney;

        private Dictionary<string, List<InventoryItem>> boxDataDict = new Dictionary<string, List<InventoryItem>>();

        public int BoxDataAmount => boxDataDict.Count;  // 方便储物箱的编号

        public string GUID => GetComponent<DataGUID>().GUID;

        private void OnEnable()
        {
            EventHandler.DropItemEvent              += OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition    += OnHarvestAtPlayerPosition;
            EventHandler.BuildFurnitureEvent        += OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent           += OnBaseBagOpenEvent;
            EventHandler.StartNewGameEvent          += OnStartNewGameEvent;
        }

        private void OnDisable()
        {
            EventHandler.DropItemEvent              -= OnDropItemEvent;
            EventHandler.HarvestAtPlayerPosition    -= OnHarvestAtPlayerPosition;
            EventHandler.BuildFurnitureEvent        -= OnBuildFurnitureEvent;
            EventHandler.BaseBagOpenEvent           -= OnBaseBagOpenEvent;
            EventHandler.StartNewGameEvent          -= OnStartNewGameEvent;
        }


        private void Start()
        {         
            ISaveable saveable = this;
            saveable.RegisterSaveable();

            //EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        private void OnDropItemEvent(int ID, Vector3 pos, ItemType itemType)
        {
            RemoveItem(ID, 1);
        }

        private void OnHarvestAtPlayerPosition(int ID)
        {
            // 背包是否已经有该物品
            var index = GetItemIndexInBag(ID);
            AddItemAtIndex(ID, index, 1);

            // 更新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            RemoveItem(ID, 1);          // 移除设计图
            BluePrintDetails bluePrint = bluePrintData.GetBluePrintDetails(ID);
            foreach (var item in bluePrint.resourceItem)
            {
                RemoveItem(item.itemID, item.itemAmount);   // 移除对应原材料
            }
        }

        private void OnBaseBagOpenEvent(SlotType slotType, InventoryBag_SO bag_SO)
        {
            currentBoxBag = bag_SO;
        }

        private void OnStartNewGameEvent(int index)
        {
            playerBag = Instantiate(playerBagTemp);
            playerMoney = Settings.playerStartMoney;
            boxDataDict.Clear();
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 通过ID返回物品信息
        /// </summary>
        /// <param name="ID">Item ID</param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }

        /// <summary>
        /// 添加物品到Player背包里
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestory">是否要销毁物品</param>
        public void AddItem(Item item, bool toDestory)
        {
            // 背包是否已经有该物品
            var index = GetItemIndexInBag(item.itemID);

            AddItemAtIndex(item.itemID, index, 1);

            Debug.Log(GetItemDetails(item.itemID).itemID + "Name:" + GetItemDetails(item.itemID).itemName);
            if(toDestory)
            {
                Destroy(item.gameObject);
            }

            // 更新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);

        }

        /// <summary>
        /// 检查背包是否有空位
        /// </summary>
        /// <returns></returns>
        private bool CheckBagCapacity()
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == 0)
                    return true;
            }
            return false;
        }


        /// <summary>
        /// 通过物品ID查找背包中对应物品位置
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <returns>-1表示没有这个物品，否则返回序号</returns>
        private int GetItemIndexInBag(int ID)
        {
            for (int i = 0; i < playerBag.itemList.Count; i++)
            {
                if (playerBag.itemList[i].itemID == ID)
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 在背包指定序列位置添加物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="index">序号</param>
        /// <param name="amount">数量</param>
        private void AddItemAtIndex(int ID, int index, int amount)
        {
            if(index == -1 && CheckBagCapacity())     // 背包没有这个物品 且 背包有空间
            {
                var item = new InventoryItem { itemID = ID, itemAmount = amount };
                for (int i = 0; i < playerBag.itemList.Count; i++)
                {
                    if (playerBag.itemList[i].itemID == 0)
                    {
                        playerBag.itemList[i] = item;
                        break;
                    }     
                }
            }
            else                // 背包有这个东西
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };

                playerBag.itemList[index] = item;
            }
        }

        /// <summary>
        /// Player背包范围内交换物品
        /// </summary>
        /// <param name="fromIndex">交换物品</param>
        /// <param name="toIndex">被交换物品</param>
        public void SwapItem(int fromIndex, int toIndex)
        {
            InventoryItem currentItem = playerBag.itemList[fromIndex];
            InventoryItem targetItem = playerBag.itemList[toIndex];

            if(targetItem.itemID != 0)
            {
                // 如果要交换的物品栏不为空
                playerBag.itemList[toIndex] = currentItem;
                playerBag.itemList[fromIndex] = targetItem;
            }
            else
            {
                // 如果要交换的物品栏为空
                playerBag.itemList[toIndex] = currentItem;
                playerBag.itemList[fromIndex] = new InventoryItem();
            }

            // 更新UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 跨背包交换物品
        /// </summary>
        /// <param name="fromLocation"></param>
        /// <param name="fromIndex"></param>
        /// <param name="targetLoaction"></param>
        /// <param name="targetIndex"></param>
        public void SwapItem(InventoryLocation fromLocation, int fromIndex, InventoryLocation targetLoaction, int targetIndex)
        {
            var currentList = GetItemList(fromLocation);
            var targetList  = GetItemList(targetLoaction);

            InventoryItem currentItem = currentList[fromIndex];

            if (targetIndex < targetList.Count)
            {
                InventoryItem targetItem = targetList[targetIndex];

                if (targetItem.itemID != 0 && currentItem.itemID != targetItem.itemID)
                {
                    // 不相同的两个物品
                    currentList[fromIndex]  = targetItem;
                    targetList[targetIndex] = currentItem;
                }
                else if (currentItem.itemID == targetItem.itemID)
                {
                    // 相同的两个物品
                    targetItem.itemAmount   += currentItem.itemAmount;
                    targetList[targetIndex] = targetItem;
                    currentList[fromIndex]  = new InventoryItem();
                }
                else
                {
                    // 将物品移到空格子中
                    targetList[targetIndex] = currentItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                EventHandler.CallUpdateInventoryUI(fromLocation, currentList);
                EventHandler.CallUpdateInventoryUI(targetLoaction, targetList);
            }
        }

        /// <summary>
        /// 根据物品所处位置返回背包数据列表
        /// </summary>
        /// <param name="location">物品所处位置</param>
        /// <returns></returns>
        private List<InventoryItem> GetItemList(InventoryLocation location)
        {
            return location switch
            {
                InventoryLocation.Player    => playerBag.itemList,
                InventoryLocation.Box       => currentBoxBag.itemList,
                _                           => null,
            };
        }

        /// <summary>
        /// 移除指定数量的背包物品
        /// </summary>
        /// <param name="ID">物品ID</param>
        /// <param name="removeAmount">移除数量</param>
        private void RemoveItem(int ID, int removeAmount)
        {
            var index = GetItemIndexInBag(ID);

            if (playerBag.itemList[index].itemAmount > removeAmount)
            {
                var newAmount = playerBag.itemList[index].itemAmount - removeAmount;
                var newItem = new InventoryItem { itemID = ID, itemAmount = newAmount };
                playerBag.itemList[index] = newItem;
            }
            else if(playerBag.itemList[index].itemAmount == removeAmount)
            {
                var newItem = new InventoryItem();
                playerBag.itemList[index] = newItem;
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 交易物品
        /// </summary>
        /// <param name="itemDetails">物品信息</param>
        /// <param name="amount">交易数量</param>
        /// <param name="isSellTrade">是否卖东西</param>
        public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
        {
            int cost = itemDetails.itemPrice * amount;
            int index = GetItemIndexInBag(itemDetails.itemID);  // 获取物品背包位置

            if (isSellTrade)
            {
                // 卖
                if (playerBag.itemList[index].itemAmount >= amount)
                {
                    RemoveItem(itemDetails.itemID, amount);
                    cost = (int)(cost * itemDetails.sellPercentage);
                    playerMoney += cost;
                }
            }
            else if (playerMoney - cost >= 0)
            {
                // 买
                if (CheckBagCapacity())
                {
                    AddItemAtIndex(itemDetails.itemID, index, amount);
                }
                playerMoney -= cost;
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// 检查蓝图中物品是否能够建造(原材料对应库存是否满足)
        /// </summary>
        /// <param name="ID">图纸ID</param>
        /// <returns></returns>
        public bool CheckStock(int ID)
        {
            var bluePrintDetails = bluePrintData.GetBluePrintDetails(ID);

            foreach (var resourceItem in bluePrintDetails.resourceItem)
            {
                var itemStock = playerBag.GetInventoryItem(resourceItem.itemID);
                if (itemStock.itemAmount >= resourceItem.itemAmount)
                {
                    continue;       // 继续判断蓝图中的下一个物品
                }
                else
                {
                    return false;   // 任意一个物品不够的话，则返回false(无法建造)
                }
            }
            return true;            // 所有物品的数量都满足
        }

        /// <summary>
        /// 根据key查找箱子数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<InventoryItem> GetBoxDataList(string key)
        {
            if(boxDataDict.ContainsKey(key))
                return boxDataDict[key];
            return null;
        }

        /// <summary>
        /// 添加箱子数据到boxDataDict
        /// </summary>
        /// <param name="box"></param>
        public void AddBoxDataDict(Box box)
        {
            var key = box.name + box.index;
            if (!boxDataDict.ContainsKey(key))
                boxDataDict.Add(key, box.boxBagData.itemList);
        }

        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.playerMoney = this.playerMoney;

            saveData.inventoryDict = new Dictionary<string, List<InventoryItem>>();
            saveData.inventoryDict.Add(playerBag.name, playerBag.itemList);

            foreach (var item in boxDataDict)
            {
                saveData.inventoryDict.Add(item.Key, item.Value);
            }

            return saveData;
        }

        public void RestoreData(GameSaveData saveData)
        {
            this.playerMoney = saveData.playerMoney;

            playerBag = Instantiate(playerBagTemp);             // 只在OnStartNewGameEvent中初始化过，读取存档无法初始化，所以在读取存档时先创建空对象
            playerBag.itemList = saveData.inventoryDict[playerBag.name];

            foreach (var item in saveData.inventoryDict)
            {
                if (boxDataDict.ContainsKey(item.Key))
                {
                    boxDataDict[item.Key] = item.Value;
                }
            }

            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }
    }
}

