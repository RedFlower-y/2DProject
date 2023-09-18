using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Save;

namespace MFarm.Inventory
{
    public class InventoryManager : Singloten<InventoryManager>, ISaveable
    {
        [Header("��Ʒ����")]
        public ItemDataList_SO itemDataList_SO;

        [Header("������ͼ")]
        public BluePrintList_SO bluePrintData;

        [Header("��������")]
        public InventoryBag_SO playerBagTemp;
        public InventoryBag_SO playerBag;
        private InventoryBag_SO currentBoxBag;

        [Header("����")]
        public int playerMoney;

        private Dictionary<string, List<InventoryItem>> boxDataDict = new Dictionary<string, List<InventoryItem>>();

        public int BoxDataAmount => boxDataDict.Count;  // ���㴢����ı��

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
            // �����Ƿ��Ѿ��и���Ʒ
            var index = GetItemIndexInBag(ID);
            AddItemAtIndex(ID, index, 1);

            // ����UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        private void OnBuildFurnitureEvent(int ID, Vector3 mousePos)
        {
            RemoveItem(ID, 1);          // �Ƴ����ͼ
            BluePrintDetails bluePrint = bluePrintData.GetBluePrintDetails(ID);
            foreach (var item in bluePrint.resourceItem)
            {
                RemoveItem(item.itemID, item.itemAmount);   // �Ƴ���Ӧԭ����
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
        /// ͨ��ID������Ʒ��Ϣ
        /// </summary>
        /// <param name="ID">Item ID</param>
        /// <returns></returns>
        public ItemDetails GetItemDetails(int ID)
        {
            return itemDataList_SO.itemDetailsList.Find(i => i.itemID == ID);
        }

        /// <summary>
        /// �����Ʒ��Player������
        /// </summary>
        /// <param name="item"></param>
        /// <param name="toDestory">�Ƿ�Ҫ������Ʒ</param>
        public void AddItem(Item item, bool toDestory)
        {
            // �����Ƿ��Ѿ��и���Ʒ
            var index = GetItemIndexInBag(item.itemID);

            AddItemAtIndex(item.itemID, index, 1);

            Debug.Log(GetItemDetails(item.itemID).itemID + "Name:" + GetItemDetails(item.itemID).itemName);
            if(toDestory)
            {
                Destroy(item.gameObject);
            }

            // ����UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);

        }

        /// <summary>
        /// ��鱳���Ƿ��п�λ
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
        /// ͨ����ƷID���ұ����ж�Ӧ��Ʒλ��
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <returns>-1��ʾû�������Ʒ�����򷵻����</returns>
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
        /// �ڱ���ָ������λ�������Ʒ
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <param name="index">���</param>
        /// <param name="amount">����</param>
        private void AddItemAtIndex(int ID, int index, int amount)
        {
            if(index == -1 && CheckBagCapacity())     // ����û�������Ʒ �� �����пռ�
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
            else                // �������������
            {
                int currentAmount = playerBag.itemList[index].itemAmount + amount;
                var item = new InventoryItem { itemID = ID, itemAmount = currentAmount };

                playerBag.itemList[index] = item;
            }
        }

        /// <summary>
        /// Player������Χ�ڽ�����Ʒ
        /// </summary>
        /// <param name="fromIndex">������Ʒ</param>
        /// <param name="toIndex">��������Ʒ</param>
        public void SwapItem(int fromIndex, int toIndex)
        {
            InventoryItem currentItem = playerBag.itemList[fromIndex];
            InventoryItem targetItem = playerBag.itemList[toIndex];

            if(targetItem.itemID != 0)
            {
                // ���Ҫ��������Ʒ����Ϊ��
                playerBag.itemList[toIndex] = currentItem;
                playerBag.itemList[fromIndex] = targetItem;
            }
            else
            {
                // ���Ҫ��������Ʒ��Ϊ��
                playerBag.itemList[toIndex] = currentItem;
                playerBag.itemList[fromIndex] = new InventoryItem();
            }

            // ����UI
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// �米��������Ʒ
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
                    // ����ͬ��������Ʒ
                    currentList[fromIndex]  = targetItem;
                    targetList[targetIndex] = currentItem;
                }
                else if (currentItem.itemID == targetItem.itemID)
                {
                    // ��ͬ��������Ʒ
                    targetItem.itemAmount   += currentItem.itemAmount;
                    targetList[targetIndex] = targetItem;
                    currentList[fromIndex]  = new InventoryItem();
                }
                else
                {
                    // ����Ʒ�Ƶ��ո�����
                    targetList[targetIndex] = currentItem;
                    currentList[fromIndex] = new InventoryItem();
                }
                EventHandler.CallUpdateInventoryUI(fromLocation, currentList);
                EventHandler.CallUpdateInventoryUI(targetLoaction, targetList);
            }
        }

        /// <summary>
        /// ������Ʒ����λ�÷��ر��������б�
        /// </summary>
        /// <param name="location">��Ʒ����λ��</param>
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
        /// �Ƴ�ָ�������ı�����Ʒ
        /// </summary>
        /// <param name="ID">��ƷID</param>
        /// <param name="removeAmount">�Ƴ�����</param>
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
        /// ������Ʒ
        /// </summary>
        /// <param name="itemDetails">��Ʒ��Ϣ</param>
        /// <param name="amount">��������</param>
        /// <param name="isSellTrade">�Ƿ�������</param>
        public void TradeItem(ItemDetails itemDetails, int amount, bool isSellTrade)
        {
            int cost = itemDetails.itemPrice * amount;
            int index = GetItemIndexInBag(itemDetails.itemID);  // ��ȡ��Ʒ����λ��

            if (isSellTrade)
            {
                // ��
                if (playerBag.itemList[index].itemAmount >= amount)
                {
                    RemoveItem(itemDetails.itemID, amount);
                    cost = (int)(cost * itemDetails.sellPercentage);
                    playerMoney += cost;
                }
            }
            else if (playerMoney - cost >= 0)
            {
                // ��
                if (CheckBagCapacity())
                {
                    AddItemAtIndex(itemDetails.itemID, index, amount);
                }
                playerMoney -= cost;
            }
            EventHandler.CallUpdateInventoryUI(InventoryLocation.Player, playerBag.itemList);
        }

        /// <summary>
        /// �����ͼ����Ʒ�Ƿ��ܹ�����(ԭ���϶�Ӧ����Ƿ�����)
        /// </summary>
        /// <param name="ID">ͼֽID</param>
        /// <returns></returns>
        public bool CheckStock(int ID)
        {
            var bluePrintDetails = bluePrintData.GetBluePrintDetails(ID);

            foreach (var resourceItem in bluePrintDetails.resourceItem)
            {
                var itemStock = playerBag.GetInventoryItem(resourceItem.itemID);
                if (itemStock.itemAmount >= resourceItem.itemAmount)
                {
                    continue;       // �����ж���ͼ�е���һ����Ʒ
                }
                else
                {
                    return false;   // ����һ����Ʒ�����Ļ����򷵻�false(�޷�����)
                }
            }
            return true;            // ������Ʒ������������
        }

        /// <summary>
        /// ����key������������
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
        /// ����������ݵ�boxDataDict
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

            playerBag = Instantiate(playerBagTemp);             // ֻ��OnStartNewGameEvent�г�ʼ��������ȡ�浵�޷���ʼ���������ڶ�ȡ�浵ʱ�ȴ����ն���
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

