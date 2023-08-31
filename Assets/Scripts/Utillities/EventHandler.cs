using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MFarm.Dialogue;

public static class EventHandler
{
    public static event Action<InventoryLocation, List<InventoryItem>> UpdateInventoryUI;
    public static void CallUpdateInventoryUI(InventoryLocation location,List<InventoryItem> list)
    {
        UpdateInventoryUI?.Invoke(location, list);
    }

    public static event Action<int, Vector3> InstantiateItemInScene;
    /// <summary>
    /// 生成物品
    /// </summary>
    /// <param name="ID">物品ID</param>
    /// <param name="pos">物品生成坐标</param>
    public static void CallInstantiateItemInScene(int ID,Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(ID, pos);
    }


    public static event Action<int, Vector3, ItemType> DropItemEvent;
    /// <summary>
    /// 扔出物品
    /// </summary>
    /// <param name="ID">物品ID</param>
    /// <param name="pos">物品扔出坐标</param>
    /// <param name="itemType">扔出物品类型</param>
    public static void CallDropItemEvent(int ID,Vector3 pos,ItemType itemType)
    {
        DropItemEvent?.Invoke(ID, pos, itemType);
    }

    
    public static event Action<ItemDetails, bool> ItemSelectedEvent;
    /// <summary>
    /// 物品选中事件 SlotUI中的OnPointerClick()事件中进行调用
    /// </summary>
    /// <param name="itemDetails">物品属性</param>
    /// <param name="isSelected">是否被选中</param>
    public static void CallItemSelectedEvent(ItemDetails itemDetails,bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails,isSelected);
    }


    public static event Action<int, int, int, Season> GameMinuteEvent;
    /// <summary>
    /// 时间事件
    /// </summary>
    public static void CallGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        GameMinuteEvent?.Invoke(minute, hour, day, season);
    }

    public static event Action<int, Season> GameDayEvent;
    /// <summary>
    /// 日子时间，刷新地图和农作物
    /// </summary>
    /// <param name="day"></param>
    /// <param name="season"></param>
    public static void CallGameDayEvent(int day,Season season)
    {
        GameDayEvent?.Invoke(day, season);
    }
    
    public static event Action<int, int, int, int, Season> GameDateEvent;
    /// <summary>
    /// 日期事件
    /// </summary>
    public static void CallGameDateEvent(int hour,int day,int month,int year,Season season)
    {
        GameDateEvent?.Invoke(hour, day, month, year, season);
    }

    
    public static event Action<string, Vector3> TransitionEvent;
    /// <summary>
    /// 传送事件
    /// </summary>
    public static void CallTransitionEvent(string sceneName,Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }

    
    public static event Action BeforeSceneUnloadEvent;
    /// <summary>
    /// 切换场景过程中，卸载场景之前的事件
    /// </summary>
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }
    
    
    public static event Action AfterSceneLoadedEvent;
    /// <summary>
    /// 切换场景过程中，加载场景之后的事件
    /// </summary>
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }

    
    public static event Action<Vector3> MoveToPosition;
    /// <summary>
    /// 切换场景过程中,加载场景之后移动Player坐标
    /// </summary>
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }

    
    public static event Action<Vector3, ItemDetails> MouseClickedEvent;
    /// <summary>
    /// 鼠标点按事件
    /// </summary>
    public static void CallMouseClickedEvent(Vector3 pos,ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(pos, itemDetails);
    }

    
    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimationEvent;
    /// <summary>
    /// 鼠标点按->动画->执行的事件
    /// </summary>
    public static void CallExecuteActionAfterAnimationEvent(Vector3 pos, ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimationEvent?.Invoke(pos, itemDetails);
    }

    public static event Action<int, TileDetails> PlantSeedEvent;
    /// <summary>
    /// 播种事件
    /// </summary>
    /// <param name="ID">种子ID</param>
    /// <param name="tile">瓦片属性</param>
    public static void CallPlantSeedEvent(int seedID,TileDetails tileDetails)
    {
        PlantSeedEvent?.Invoke(seedID, tileDetails);
    }

    public static event Action<int> HarvestAtPlayerPosition;
    /// <summary>
    /// 在玩家位置生成果实
    /// </summary>
    /// <param name="ID"></param>
    public static void CallHarvestAtPlayerPosition(int ID)
    {
        HarvestAtPlayerPosition?.Invoke(ID);
    }


    public static event Action RefreshCurrentMap;
    /// <summary>
    /// 更新可重复收割的种子瓦片
    /// </summary>
    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMap?.Invoke();
    }

    public static event Action<ParticaleEffectType, Vector3> ParticleEffectEvent;
    /// <summary>
    /// 生成粒子特效
    /// </summary>
    /// <param name="effectType">特效类型</param>
    /// <param name="pos">生成坐标</param>
    public static void CallParticleEffectEvent(ParticaleEffectType effectType,Vector3 pos)
    {
        ParticleEffectEvent?.Invoke(effectType, pos);
    }

    public static event Action GenerateCropEvent;
    /// <summary>
    /// 预先生成植物
    /// </summary>
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }

    public static event Action<DialoguePiece> ShowDialogueEvent;
    /// <summary>
    /// 显示对话
    /// </summary>
    /// <param name="dialoguePiece"></param>
    public static void CallShowDialogueEvent(DialoguePiece dialoguePiece)
    {
        ShowDialogueEvent?.Invoke(dialoguePiece);
    }

    public static event Action<SlotType, InventoryBag_SO> BaseBagOpenEvent;
    /// <summary>
    /// 打开物品目录 (商店 或者 储物箱)
    /// </summary>
    /// <param name="slotType">目录类型</param>
    /// <param name="bag_SO">对应数据SO</param>
    public static void CallBaseBagOpenEvent(SlotType slotType,InventoryBag_SO bag_SO)
    {
        BaseBagOpenEvent?.Invoke(slotType, bag_SO);
    }

    public static event Action<SlotType, InventoryBag_SO> BaseBagCloseEvent;
    /// <summary>
    /// 关闭物品目录 (商店 或者 储物箱)
    /// </summary>
    /// <param name="slotType">目录类型</param>
    /// <param name="bag_SO">对应数据SO</param>
    public static void CallBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bag_SO)
    {
        BaseBagCloseEvent?.Invoke(slotType, bag_SO);
    }


    public static event Action<GameState> UpdateGameStateEvent;
    /// <summary>
    /// 更新游戏运行状态
    /// </summary>
    /// <param name="gameState"></param>
    public static void CallUpdateGameStateEvent(GameState gameState)
    {
        UpdateGameStateEvent?.Invoke(gameState);
    }

    public static event Action<ItemDetails, bool> ShowTradeUI;
    /// <summary>
    /// 显示交易详情窗口
    /// </summary>
    /// <param name="item"></param>
    /// <param name="isSell"></param>
    public static void CallShowTradeUI(ItemDetails item,bool isSell)
    {
        ShowTradeUI?.Invoke(item, isSell);
    }
}
