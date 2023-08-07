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
    /// ������Ʒ
    /// </summary>
    /// <param name="ID">��ƷID</param>
    /// <param name="pos">��Ʒ��������</param>
    public static void CallInstantiateItemInScene(int ID,Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(ID, pos);
    }


    public static event Action<int, Vector3> DropItemEvent;
    /// <summary>
    /// �ӳ���Ʒ
    /// </summary>
    /// <param name="ID">��ƷID</param>
    /// <param name="pos">��Ʒ�ӳ�����</param>
    public static void CallDropItemEvent(int ID,Vector3 pos)
    {
        DropItemEvent?.Invoke(ID, pos);
    }

    
    public static event Action<ItemDetails, bool> ItemSelectedEvent;
    /// <summary>
    /// ��Ʒѡ���¼� SlotUI�е�OnPointerClick()�¼��н��е���
    /// </summary>
    /// <param name="itemDetails">��Ʒ����</param>
    /// <param name="isSelected">�Ƿ�ѡ��</param>
    public static void CallItemSelectedEvent(ItemDetails itemDetails,bool isSelected)
    {
        ItemSelectedEvent?.Invoke(itemDetails,isSelected);
    }

    
    public static event Action<int, int> GameMinuteEvent;
    /// <summary>
    /// ʱ���¼�
    /// </summary>
    public static void CallGameMinuteEvent(int minute,int hour)
    {
        GameMinuteEvent?.Invoke(minute, hour);
    }

    public static event Action<int, Season> GameDayEvent;
    /// <summary>
    /// ����ʱ�䣬ˢ�µ�ͼ��ũ����
    /// </summary>
    /// <param name="day"></param>
    /// <param name="season"></param>
    public static void CallGameDayEvent(int day,Season season)
    {
        GameDayEvent?.Invoke(day, season);
    }
    
    public static event Action<int, int, int, int, Season> GameDateEvent;
    /// <summary>
    /// �����¼�
    /// </summary>
    public static void CallGameDateEvent(int hour,int day,int month,int year,Season season)
    {
        GameDateEvent?.Invoke(hour, day, month, year, season);
    }

    
    public static event Action<string, Vector3> TransitionEvent;
    /// <summary>
    /// �����¼�
    /// </summary>
    public static void CallTransitionEvent(string sceneName,Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
    }

    
    public static event Action BeforeSceneUnloadEvent;
    /// <summary>
    /// �л����������У�ж�س���֮ǰ���¼�
    /// </summary>
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
    }
    
    
    public static event Action AfterSceneLoadedEvent;
    /// <summary>
    /// �л����������У����س���֮����¼�
    /// </summary>
    public static void CallAfterSceneLoadedEvent()
    {
        AfterSceneLoadedEvent?.Invoke();
    }

    
    public static event Action<Vector3> MoveToPosition;
    /// <summary>
    /// �л�����������,���س���֮���ƶ�Player����
    /// </summary>
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }

    
    public static event Action<Vector3, ItemDetails> MouseClickedEvent;
    /// <summary>
    /// ���㰴�¼�
    /// </summary>
    public static void CallMouseClickedEvent(Vector3 pos,ItemDetails itemDetails)
    {
        MouseClickedEvent?.Invoke(pos, itemDetails);
    }

    
    public static event Action<Vector3, ItemDetails> ExecuteActionAfterAnimationEvent;
    /// <summary>
    /// ���㰴->����->ִ�е��¼�
    /// </summary>
    public static void CallExecuteActionAfterAnimationEvent(Vector3 pos, ItemDetails itemDetails)
    {
        ExecuteActionAfterAnimationEvent?.Invoke(pos, itemDetails);
    }
}
