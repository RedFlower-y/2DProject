using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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


    public static event Action<int, Vector3> DropItemEvent;
    /// <summary>
    /// 扔出物品
    /// </summary>
    /// <param name="ID">物品ID</param>
    /// <param name="pos">物品扔出坐标</param>
    public static void CallDropItemEvent(int ID,Vector3 pos)
    {
        DropItemEvent?.Invoke(ID, pos);
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

    
    public static event Action<int, int> GameMinuteEvent;
    /// <summary>
    /// 时间事件
    /// </summary>
    public static void CallGameMinuteEvent(int minute,int hour)
    {
        GameMinuteEvent?.Invoke(minute, hour);
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
}
