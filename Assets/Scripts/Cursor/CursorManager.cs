using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using MFarm.Map;
using MFarm.CropPlant;
using MFarm.Inventory;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed, item;
    private Sprite currentSprite;       // 存储当前鼠标图片
    private Image cursorImage;
    private RectTransform cursorCanvas;

    // 建造图标跟随
    private Image buildImage;           // 跟随鼠标的建造物(蓝图生成)图片

    // 鼠标检测
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

        // 拿到建造物的图标
        buildImage = cursorCanvas.GetChild(1).GetComponent<Image>();
        buildImage.gameObject.SetActive(false);

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
            buildImage.gameObject.SetActive(false);
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

        buildImage.color = new Color(1, 1, 1, 0.5f);    // 可以建造时，将建造物图片设置为半透明
    }

    /// <summary>
    /// 设置鼠标不可用
    /// </summary>
    private void SetCursorInvalid()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.4f);

        buildImage.color = new Color(1, 0, 0, 0.5f);    // 不可以建造时，将建造物图片设置为半透明红色
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

            buildImage.gameObject.SetActive(false);      // 没被选择，关闭建造物图片
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

            // 显示建造物图片
            if (itemDetails.itemType == ItemType.Furniture)
            {
                buildImage.gameObject.SetActive(true);
                buildImage.sprite = itemDetails.itemOnWorldSprite;
                buildImage.SetNativeSize();
            }
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

        // 建造物图片跟随移动
        buildImage.rectTransform.position = Input.mousePosition;

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
            Crop crop = GridMapManager.Instance.GetCropObject(mouseWorldPos);

            // WORKFLOW:补充所有物品类型的判断
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

                case ItemType.BreakTool:
                case ItemType.ChopTool:
                    if (crop != null)
                    {
                        if (crop.CanHarvest && crop.cropDetails.CheckToolAvailable(currentItem.itemID)) SetCursorValid(); else SetCursorInvalid();
                    }
                    else 
                    {
                        SetCursorInvalid();
                    }
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

                case ItemType.ReapTool:
                    if (GridMapManager.Instance.HaveReapableItemsInRadius(mouseWorldPos, currentItem)) SetCursorValid(); else SetCursorInvalid();
                    break;

                case ItemType.Furniture:
                    buildImage.gameObject.SetActive(true);
                    var bluePrintDetails = InventoryManager.Instance.bluePrintData.GetBluePrintDetails(currentItem.itemID);

                    if (currentTile.canPlaceFurniture && InventoryManager.Instance.CheckStock(currentItem.itemID) && !HaveFurnitureInRadius(bluePrintDetails))
                        SetCursorValid();
                    else
                        SetCursorInvalid();
                    break;
            }
        }
        else
        {
            SetCursorInvalid();
        }
    }

    /// <summary>
    /// 判断建造家具的地方有没有其他家具
    /// </summary>
    /// <param name="bluePrintDetails">家具蓝图</param>
    /// <returns></returns>
    private bool HaveFurnitureInRadius(BluePrintDetails bluePrintDetails)
    {
        var buildItem = bluePrintDetails.buildPrefab;
        Vector2 point = mouseWorldPos;
        var size = buildItem.GetComponent<BoxCollider2D>().size;

        var otherColl = Physics2D.OverlapBox(point, size, 0);
        if (otherColl != null)
            return otherColl.GetComponent<Furniture>();
        return false;
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
