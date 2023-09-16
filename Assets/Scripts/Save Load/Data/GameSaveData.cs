using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Save
{
    [System.Serializable]
    public class GameSaveData
    {
        public string dataSceneName;

        /// <summary>
        /// string：人物名称 SerializableVector3：人物坐标
        /// </summary>
        public Dictionary<string, SerializableVector3> characterPosDict;

        /// <summary>
        /// string：场景名称 List<SceneItem>：场景物品
        /// </summary>
        public Dictionary<string, List<SceneItem>> sceneItemDict;

        /// <summary>
        /// string：场景名称 List<SceneFurniture>：场景家具
        /// </summary>
        public Dictionary<string, List<SceneFurniture>> sceneFurnitureDict;

        /// <summary>
        /// string：X坐标+Y坐标+场景名称 TileDetails：瓦片信息
        /// </summary>
        public Dictionary<string, TileDetails> tileDetailsDict;

        /// <summary>
        /// string：场景名称 bool：是否第一次加载
        /// </summary>
        public Dictionary<string, bool> isFirstLoadDict;

        /// <summary>
        /// string：box.name+index List<InventoryItem>：库存物品信息
        /// </summary>
        public Dictionary<string, List<InventoryItem>> inventoryDict;

        /// <summary>
        /// string：年，月，日，小时，分，秒 int：对应时间数值
        /// </summary>
        public Dictionary<string, int> timeDict;

        public int playerMoney;

        //NPC
        public string targetScene;
        public bool interactable;
        public int animationInstanceID;
    }
}
