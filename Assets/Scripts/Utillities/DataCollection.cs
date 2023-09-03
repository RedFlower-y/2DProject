using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   // ���л�
public class ItemDetails
{
    public int      itemID;
    public string   itemName;
    public ItemType itemType;
    public Sprite   itemIcon;
    public Sprite   itemOnWorldSprite;
    public string   itemDescription;
    public int      itemUseRadius;          // ʹ�÷�Χ
    public bool     canPickedUp;
    public bool     canDropped;
    public bool     canCarried;
    public int      itemPrice;
    [Range(0, 1)]
    public float    sellPercentage;         // ��������۸����
}

[System.Serializable]   // ���л�
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
/// �����޷�ֱ�Ӵ��棬������Ҫ���л�
/// </summary>
[System.Serializable]
public class SerializableVector3
{
    public float x, y, z;

    // ���������л�
    public SerializableVector3(Vector3 pos)
    {
        this.x = pos.x;
        this.y = pos.y;
        this.z = pos.z;
    }

    // �����л����xyzת������
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }

    // 2D��Ϸ��ֱ�ӷ��ض�ά����
    public Vector2Int ToVector2Int()
    {
        return new Vector2Int((int)x, (int)y);
    }
}

/// <summary>
/// �����������Ʒ
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
    // TODO:������Ϣ ���索����
    public int itemID;
    public SerializableVector3 position;
}


/// <summary>
/// ��Ƭ��ͼ����
/// </summary>
[System.Serializable]
public class TileProperty
{
    public Vector2Int tileCoordinate;       // ��Ƭ����
    public GridType gridType;
    public bool boolTypeValue;
}

/// <summary>
/// ��Ƭ��ͼ�ֵ�
/// </summary>
[System.Serializable]
public class TileDetails
{
    public int  gridX, gridY;
    public bool canDig;
    public bool canDropItem;
    public bool canPlaceFurniture;
    public bool isNPCObstacle;
    public int  daySinceDug = -1;            // ��¼����ʱ��
    public int  daySinceWatered = -1;        // ��¼��ˮʱ��
    public int  seedItemID = -1;             // ��¼��Ƭ��ͼ������ӱ��
    public int  growthDays = -1;             // ��¼��������ʱ��
    public int  daySinceLastHarvest = -1;    // ��¼������һ���ո���˶೤ʱ��
}

[System.Serializable]
public class NPCPosition
{
    public Transform npc;
    public string startScene;
    public Vector3 position;
}

/// <summary>
/// ����·���Ľڵ�
/// </summary>
[System.Serializable]
public class ScenePath
{
    public string sceneName;
    public Vector2Int fromGridCell;
    public Vector2Int goToGridCell;
}

/// <summary>
/// ����·��
/// </summary>
[System.Serializable]
public class SceneRoute
{
    public string fromSceneName;
    public string goToSceneName;
    public List<ScenePath> scenePathList;
}
