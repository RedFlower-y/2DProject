using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    [RequireComponent(typeof(SlotUI))]
    public class ActionBarButton : MonoBehaviour
    {
        public KeyCode key;         // 设置快捷键

        private SlotUI slotUI;

        private bool canUseKeyboard;    // 在物品交易窗口打开时，禁止使用键盘快捷键控制物品栏

        private void Awake()
        {
            slotUI = GetComponent<SlotUI>();
        }

        private void OnEnable()
        {
            EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;
        }

        private void OnDisable()
        {
            EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        }

        private void Update()
        {
            if (Input.GetKeyDown(key) && canUseKeyboard)
            {
                slotUI.isSelected = !slotUI.isSelected;
                if (slotUI.isSelected)
                    slotUI.inventoryUI.UpdateSlotHighlight(slotUI.slotIndex);
                else
                    slotUI.inventoryUI.UpdateSlotHighlight(-1);

                EventHandler.CallItemSelectedEvent(slotUI.itemDetails, slotUI.isSelected);
            }
        }

        private void OnUpdateGameStateEvent(GameState gameState)
        {
            canUseKeyboard = gameState == GameState.GamePlay;
        }
    }
}