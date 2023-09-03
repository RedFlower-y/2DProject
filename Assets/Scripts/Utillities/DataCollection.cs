using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   // 序列化
public class ItemDetails
{
    public int      itemID;
    public string   itemName;
    public ItemType itemType;
    public Sprite   itemIcon;
    public Sprite   itemOnWorldSprite;
    public string   itemDescription;
    public int      itemUseRadius;          // 使用范围
    public bool     canPickedUp;
    public bool     canDropped;
    public bool     canCarried;
    public int      itemPrice;
    [Range(0, 1)]
    public float    sellPercentage;         // 买卖折算价格比例
}

[System.Serializable]   // 序列化
public struct InventoryItem
{
    public int itemID;
    public int itemAmount;
}

[System.Serializable]
public class AnimatorType
{
    public PartType partType;
    public PartName partName;
    public AnimatorOverrideController overrideController;
}

/// <summary>
/// 坐标无法直接储存，所以需要序列化
/// </summary>
[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;

    // 将坐标序列化
    public SerializableVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }

    // 将序列化后的xyz转回坐标
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    // 2D游戏，直接返回二维坐标
    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x, (int)y);
    }
}

/// <summary>
/// 场景储存的物品
/// </summary>
[System.Serializable]
public class SceneItem
{
    public int itemID;
    public SerializableVector3 position;
}

[System.Serializable]
public class SceneFurniture
{
    // TODO:更多信息 例如储物箱
    public int itemID;
    public SerializableVector3 position;
}


/// <summary>
/// 瓦片地图属性
/// </summary>
[System.Serializable]
public class TileProperty
{
    public Vector2Int tileCoordinate;       // 瓦片坐标
    public GridType gridType;
    public bool boolTypeValue;
}

/// <summary>
/// 瓦片地图字典
/// </summary>
[System.Serializable]
public class TileDetails
{
    public int  gridX, gridY;
    public bool canDig;
    public bool canDropItem;
    public bool canPlaceFurniture;
    public bool isNPCObstacle;
    public int  daySinceDug = -1;            // 记录挖土时间
    public int  daySinceWatered = -1;        // 记录浇水时间
    public int  seedItemID = -1;             // 记录瓦片地图里的种子编号
    public int  growthDays = -1;             // 记录种子生长时间
    public int  daySinceLastHarvest = -1;    // 记录距离上一次收割过了多长时间
}

[System.Serializable]
public class NPCPosition
{
    public Transform npc;
    public string startScene;
    public Vector3 position;
}

/// <summary>
/// 场景路径的节点
/// </summary>
[System.Serializable]
public class ScenePath
{
    public string sceneName;
    public Vector2Int fromGridCell;
    public Vector2Int goToGridCell;
}

/// <summary>
/// 场景路径
/// </summary>
[System.Serializable]
public class SceneRoute
{
    public string fromSceneName;
    public string goToSceneName;
    public List<ScenePath> scenePathList;
}
