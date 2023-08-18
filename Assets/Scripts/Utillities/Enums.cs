public enum ItemType
{
    Seed,           // ����
    Commodity,      // ��Ʒ
    Furniture,      // �Ҿ�
    
    HoeTool,        // ��ͷ
    ChopTool,       // ���ӣ�������
    BreakTool,      // ���ӣ���ʯͷ��
    ReapTool,       // ��������ݣ�
    WaterTool,      // ��ˮ��
    CollectTool,    // �ղ�

    ReapableScenery,// ���Ա�����Ӳ�
}

/// <summary>
/// ������������
/// </summary>
public enum SlotType    
{
    Bag,
    Box,
    Shop,
}

/// <summary>
/// ��Ʒ����λ��
/// </summary>
public enum InventoryLocation
{
    Player,
    Box,
}

/// <summary>
/// ������Ʒ��𣨸���������Ʒ�ж϶�Ӧ��Player������
/// </summary>
public enum PartType
{
    None,
    Carry,
    Hoe,
    Chop,
    Break,
    Water,
    Collect,
    Reap,
}

/// <summary>
/// ��ɫ���岿��
/// </summary>
public enum PartName
{
    Body,
    Hair,
    Arm,
    Tool,
}

/// <summary>
/// ����
/// </summary>
public enum Season
{
    ����,
    ����,
    ����,
    ����,
}


/// <summary>
/// ��Ƭ��ͼ����
/// </summary>
public enum GridType
{
    Diggable,
    DropItem,
    PlaceFurniture,
    NPCObstacle,
}


public enum ParticaleEffectType
{
    None,
    LeaceFalling01,
    LeaceFalling02,
    Rock,
    ReapableScenery,
}