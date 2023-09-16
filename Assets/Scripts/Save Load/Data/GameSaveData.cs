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
        /// string���������� SerializableVector3����������
        /// </summary>
        public Dictionary<string, SerializableVector3> characterPosDict;

        /// <summary>
        /// string���������� List<SceneItem>��������Ʒ
        /// </summary>
        public Dictionary<string, List<SceneItem>> sceneItemDict;

        /// <summary>
        /// string���������� List<SceneFurniture>�������Ҿ�
        /// </summary>
        public Dictionary<string, List<SceneFurniture>> sceneFurnitureDict;

        /// <summary>
        /// string��X����+Y����+�������� TileDetails����Ƭ��Ϣ
        /// </summary>
        public Dictionary<string, TileDetails> tileDetailsDict;

        /// <summary>
        /// string���������� bool���Ƿ��һ�μ���
        /// </summary>
        public Dictionary<string, bool> isFirstLoadDict;

        /// <summary>
        /// string��box.name+index List<InventoryItem>�������Ʒ��Ϣ
        /// </summary>
        public Dictionary<string, List<InventoryItem>> inventoryDict;

        /// <summary>
        /// string���꣬�£��գ�Сʱ���֣��� int����Ӧʱ����ֵ
        /// </summary>
        public Dictionary<string, int> timeDict;

        public int playerMoney;

        //NPC
        public string targetScene;
        public bool interactable;
        public int animationInstanceID;
    }
}
