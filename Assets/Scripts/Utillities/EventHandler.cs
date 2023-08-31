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
    /// ������Ʒ
    /// </summary>
    /// <param name="ID">��ƷID</param>
    /// <param name="pos">��Ʒ��������</param>
    public static void CallInstantiateItemInScene(int ID,Vector3 pos)
    {
        InstantiateItemInScene?.Invoke(ID, pos);
    }


    public static event Action<int, Vector3, ItemType> DropItemEvent;
    /// <summary>
    /// �ӳ���Ʒ
    /// </summary>
    /// <param name="ID">��ƷID</param>
    /// <param name="pos">��Ʒ�ӳ�����</param>
    /// <param name="itemType">�ӳ���Ʒ����</param>
    public static void CallDropItemEvent(int ID,Vector3 pos,ItemType itemType)
    {
        DropItemEvent?.Invoke(ID, pos, itemType);
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


    public static event Action<int, int, int, Season> GameMinuteEvent;
    /// <summary>
    /// ʱ���¼�
    /// </summary>
    public static void CallGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        GameMinuteEvent?.Invoke(minute, hour, day, season);
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

    public static event Action<int, TileDetails> PlantSeedEvent;
    /// <summary>
    /// �����¼�
    /// </summary>
    /// <param name="ID">����ID</param>
    /// <param name="tile">��Ƭ����</param>
    public static void CallPlantSeedEvent(int seedID,TileDetails tileDetails)
    {
        PlantSeedEvent?.Invoke(seedID, tileDetails);
    }

    public static event Action<int> HarvestAtPlayerPosition;
    /// <summary>
    /// �����λ�����ɹ�ʵ
    /// </summary>
    /// <param name="ID"></param>
    public static void CallHarvestAtPlayerPosition(int ID)
    {
        HarvestAtPlayerPosition?.Invoke(ID);
    }


    public static event Action RefreshCurrentMap;
    /// <summary>
    /// ���¿��ظ��ո��������Ƭ
    /// </summary>
    public static void CallRefreshCurrentMap()
    {
        RefreshCurrentMap?.Invoke();
    }

    public static event Action<ParticaleEffectType, Vector3> ParticleEffectEvent;
    /// <summary>
    /// ����������Ч
    /// </summary>
    /// <param name="effectType">��Ч����</param>
    /// <param name="pos">��������</param>
    public static void CallParticleEffectEvent(ParticaleEffectType effectType,Vector3 pos)
    {
        ParticleEffectEvent?.Invoke(effectType, pos);
    }

    public static event Action GenerateCropEvent;
    /// <summary>
    /// Ԥ������ֲ��
    /// </summary>
    public static void CallGenerateCropEvent()
    {
        GenerateCropEvent?.Invoke();
    }

    public static event Action<DialoguePiece> ShowDialogueEvent;
    /// <summary>
    /// ��ʾ�Ի�
    /// </summary>
    /// <param name="dialoguePiece"></param>
    public static void CallShowDialogueEvent(DialoguePiece dialoguePiece)
    {
        ShowDialogueEvent?.Invoke(dialoguePiece);
    }

    public static event Action<SlotType, InventoryBag_SO> BaseBagOpenEvent;
    /// <summary>
    /// ����ƷĿ¼ (�̵� ���� ������)
    /// </summary>
    /// <param name="slotType">Ŀ¼����</param>
    /// <param name="bag_SO">��Ӧ����SO</param>
    public static void CallBaseBagOpenEvent(SlotType slotType,InventoryBag_SO bag_SO)
    {
        BaseBagOpenEvent?.Invoke(slotType, bag_SO);
    }

    public static event Action<SlotType, InventoryBag_SO> BaseBagCloseEvent;
    /// <summary>
    /// �ر���ƷĿ¼ (�̵� ���� ������)
    /// </summary>
    /// <param name="slotType">Ŀ¼����</param>
    /// <param name="bag_SO">��Ӧ����SO</param>
    public static void CallBaseBagCloseEvent(SlotType slotType, InventoryBag_SO bag_SO)
    {
        BaseBagCloseEvent?.Invoke(slotType, bag_SO);
    }


    public static event Action<GameState> UpdateGameStateEvent;
    /// <summary>
    /// ������Ϸ����״̬
    /// </summary>
    /// <param name="gameState"></param>
    public static void CallUpdateGameStateEvent(GameState gameState)
    {
        UpdateGameStateEvent?.Invoke(gameState);
    }

    public static event Action<ItemDetails, bool> ShowTradeUI;
    /// <summary>
    /// ��ʾ�������鴰��
    /// </summary>
    /// <param name="item"></param>
    /// <param name="isSell"></param>
    public static void CallShowTradeUI(ItemDetails item,bool isSell)
    {
        ShowTradeUI?.Invoke(item, isSell);
    }
}
