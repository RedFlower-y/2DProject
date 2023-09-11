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

/// <summary>
/// ��Ч����
/// </summary>
public enum ParticleEffectType
{
    None,
    LeaceFalling01,
    LeaceFalling02,
    Rock,
    ReapableScenery,
}

/// <summary>
/// ��Ϸ״̬
/// </summary>
public enum GameState
{
    GamePlay,
    GamePause,
}

/// <summary>
/// ��ͬʱ�����ڶ�Ӧ�ƹ�
/// </summary>
public enum LightShift
{
    Morning,
    Night,
}

/// <summary>
/// ��������
/// </summary>
public enum SoundName
{
    none,
    FootStepSoft,
    FootStepHard,

    Axe,                    // ����
    Pickaxe,                // ����
    Hoe,                    // ��ͷ
    Scythe,                 // ����
    WateringCan,            // ��ˮ��
    Basket,                 // ����

    Pickup,
    Plant,
    Pluck,
    Rustle,                 // ·���ݴ�����
    TreeFalling,
    StoneShatter,
    WoodSplinters,
    

    AmbientCountryside1,
    AmbientCountryside2,
    AmbientIndoor1,
    MusicCalm1,
    MusicCalm2,
    MusicCalm3,
    MusicCalm4,
    MusicCalm5,
    MusicCalm6,
}