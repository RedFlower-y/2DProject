public enum ItemType
{
    Seed,           // 种子
    Commodity,      // 商品
    Furniture,      // 家具
    
    HoeTool,        // 锄头
    ChopTool,       // 斧子（砍树）
    BreakTool,      // 锤子（砸石头）
    ReapTool,       // 镰刀（割草）
    WaterTool,      // 浇水器
    CollectTool,    // 收菜

    ReapableScenery,// 可以被割的杂草
}

/// <summary>
/// 收纳容器类型
/// </summary>
public enum SlotType    
{
    Bag,
    Box,
    Shop,
}

/// <summary>
/// 物品所处位置
/// </summary>
public enum InventoryLocation
{
    Player,
    Box,
}

/// <summary>
/// 所持物品类别（根据所持物品判断对应的Player动画）
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
/// 角色身体部分
/// </summary>
public enum PartName
{
    Body,
    Hair,
    Arm,
    Tool,
}

/// <summary>
/// 季节
/// </summary>
public enum Season
{
    春天,
    夏天,
    秋天,
    冬天,
}


/// <summary>
/// 瓦片地图类型
/// </summary>
public enum GridType
{
    Diggable,
    DropItem,
    PlaceFurniture,
    NPCObstacle,
}

/// <summary>
/// 特效类型
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
/// 游戏状态
/// </summary>
public enum GameState
{
    GamePlay,
    GamePause,
}

/// <summary>
/// 不同时间周期对应灯光
/// </summary>
public enum LightShift
{
    Morning,
    Night,
}

/// <summary>
/// 声音名字
/// </summary>
public enum SoundName
{
    none,
    FootStepSoft,
    FootStepHard,

    Axe,                    // 斧子
    Pickaxe,                // 稿子
    Hoe,                    // 锄头
    Scythe,                 // 镰刀
    WateringCan,            // 浇水器
    Basket,                 // 篮子

    Pickup,
    Plant,
    Pluck,
    Rustle,                 // 路过草丛声音
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