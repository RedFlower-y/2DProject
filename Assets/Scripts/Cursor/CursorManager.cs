using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MFarm.Map;
using MFarm.CropPlant;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed, item;
    private Sprite currentSprite;       // 存储当前鼠标图片
    private Image cursorImage;
    private RectTransform cursorCanvas;

    private bool cursorEnable;          // 鼠标是否可用
    private bool cursorPositionValid;   // 鼠标是否可点

    private ItemDetails currentItem;

    private Transform playerTransform => FindObjectOfType<Player>().transform;  // 因为player在场景中始终固定存在，所以可以用这种方法获取player

    // 鼠标的地图检测
    private Camera mainCamera;          // 将屏幕坐标转换为世界坐标
    private Grid currentGrid;           // 将世界坐标转换为瓦片地图的网格坐标
    private Vector3 mouseWorldPos;      // 世界坐标
    private Vector3Int mouseGridPos;    // 网格坐标

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent      += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent  += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent      -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent  -= OnAfterSceneLoadedEvent;
    }

    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        currentSprite = normal;
        SetCursorImage(currentSprite);

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if(cursorCanvas == null)
            return;
        cursorImage.transform.position = Input.mousePosition;

        if(!InteractWithUI() && cursorEnable)
        {
            // 当不与UI互动 且 鼠标可用
            SetCursorImage(currentSprite);
            CheckCursorVaild();
            CheckPlayerInput();
        }
        else
        {
            SetCursorImage(normal);
        }
    }

    /// <summary>
    /// 鼠标点击
    /// </summary>
    private void CheckPlayerInput()
    {
        if(Input.GetMouseButtonDown(0) && cursorPositionValid)
        {   // 点击鼠标左键 且 点击位置合理
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);     // 执行方法

        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;       // 卸载当前场景前，关闭鼠标
    }

    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
    }

    #region 设置鼠标样式
    /// <summary>
    /// 设置鼠标图片
    /// </summary>
    /// <param name="sprite">鼠标的目标图片</param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// 设置鼠标可用
    /// </summary>
    private void SetCursorValid()
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// 设置鼠标不可用
    /// </summary>
    private void SetCursorInvalid()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.4f);
    }
    #endregion

    /// <summary>
    /// 根据选择物品的不同，来切换鼠标的显示图片
    /// </summary>
    /// <param name="itemDetails">选中物品</param>
    /// <param name="isSelected">是否选中</param>
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {   // 物品没有被选中
            currentItem = null;
            cursorEnable = false;
            currentSprite = normal;
        }
        else
        {   // 物品被选中，切换图片
            currentItem = itemDetails;      // 将选中的物品信息暂时储存,方便后面调用
            // WORKFLOW:添加所有类型对应图片
            currentSprite = itemDetails.itemType switch
            {
                ItemType.Seed           => seed,
                ItemType.Commodity      => item,
                ItemType.ChopTool       => tool,
                ItemType.HoeTool        => tool,
                ItemType.WaterTool      => tool,
                ItemType.BreakTool      => tool,
                ItemType.ReapTool       => tool,
                ItemType.Furniture      => tool,
                ItemType.CollectTool    => tool,
                _                       => normal,
            };

            cursorEnable = true;
        }     
    }
    /// <summary>
    /// 检测鼠标在地图上的操作是否可以执行
    /// </summary>
    private void CheckCursorVaild()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z)); // 因为2D游戏mainCamera的z轴为-10
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
        //Debug.Log("WorldPos:" + mouseWorldPos + "   GridPos:" + mouseGridPos);

        // 判断物品的适用范围
        var playerGridPos = currentGrid.WorldToCell(playerTransform.position);
        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius ||
            Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInvalid();
            return;
        }

        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);
        if(currentTile != null)
        {
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemID);

            // WORKFLOW:不充所有物品类型的判断
            switch(currentItem.itemType)
            {
                case ItemType.Seed:
                    if (currentTile.daySinceDug > -1 && currentTile.seedItemID == -1) SetCursorValid(); else SetCursorInvalid();
                    break;

                case ItemType.Commodity:
                    if (currentTile.canDropItem && currentItem.canDropped) SetCursorValid(); else SetCursorInvalid(); 
                    break;

                case ItemType.HoeTool:
                    if (currentTile.canDig) SetCursorValid(); else SetCursorInvalid();
                    break;

                case ItemType.WaterTool:
                    if (currentTile.daySinceDug > -1 && currentTile.daySinceWatered == -1) SetCursorValid(); else SetCursorInvalid();
                    break;

                case ItemType.CollectTool:
                    if(currentCrop != null)
                    {
                        // 当前农作物为不为空
                        if (currentCrop.CheckToolAvailable(currentItem.itemID))
                        {
                            // 且工具可用
                            if (currentTile.growthDays >= currentCrop.TotalGrowthDays) SetCursorValid(); else SetCursorInvalid();
                        }
                    }
                    else
                    {
                        SetCursorInvalid();
                    }
                    break;

            }
        }
        else
        {
            SetCursorInvalid();
        }
    }

    /// <summary>
    /// 是否与UI互动
    /// </summary>
    /// <returns>与UI互动则返回true，否则false</returns>
    private bool InteractWithUI()
    {
        if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;
        return false;
    }
}
