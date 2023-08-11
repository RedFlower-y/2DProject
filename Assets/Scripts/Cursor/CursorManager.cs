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
    private Sprite currentSprite;       // �洢��ǰ���ͼƬ
    private Image cursorImage;
    private RectTransform cursorCanvas;

    private bool cursorEnable;          // ����Ƿ����
    private bool cursorPositionValid;   // ����Ƿ�ɵ�

    private ItemDetails currentItem;

    private Transform playerTransform => FindObjectOfType<Player>().transform;  // ��Ϊplayer�ڳ�����ʼ�չ̶����ڣ����Կ��������ַ�����ȡplayer

    // ���ĵ�ͼ���
    private Camera mainCamera;          // ����Ļ����ת��Ϊ��������
    private Grid currentGrid;           // ����������ת��Ϊ��Ƭ��ͼ����������
    private Vector3 mouseWorldPos;      // ��������
    private Vector3Int mouseGridPos;    // ��������

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
            // ������UI���� �� ������
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
    /// �����
    /// </summary>
    private void CheckPlayerInput()
    {
        if(Input.GetMouseButtonDown(0) && cursorPositionValid)
        {   // ��������� �� ���λ�ú���
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);     // ִ�з���

        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;       // ж�ص�ǰ����ǰ���ر����
    }

    private void OnAfterSceneLoadedEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
    }

    #region ���������ʽ
    /// <summary>
    /// �������ͼƬ
    /// </summary>
    /// <param name="sprite">����Ŀ��ͼƬ</param>
    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// ����������
    /// </summary>
    private void SetCursorValid()
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    /// <summary>
    /// ������겻����
    /// </summary>
    private void SetCursorInvalid()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.4f);
    }
    #endregion

    /// <summary>
    /// ����ѡ����Ʒ�Ĳ�ͬ�����л�������ʾͼƬ
    /// </summary>
    /// <param name="itemDetails">ѡ����Ʒ</param>
    /// <param name="isSelected">�Ƿ�ѡ��</param>
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {   // ��Ʒû�б�ѡ��
            currentItem = null;
            cursorEnable = false;
            currentSprite = normal;
        }
        else
        {   // ��Ʒ��ѡ�У��л�ͼƬ
            currentItem = itemDetails;      // ��ѡ�е���Ʒ��Ϣ��ʱ����,����������
            // WORKFLOW:����������Ͷ�ӦͼƬ
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
    /// �������ڵ�ͼ�ϵĲ����Ƿ����ִ��
    /// </summary>
    private void CheckCursorVaild()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z)); // ��Ϊ2D��ϷmainCamera��z��Ϊ-10
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);
        //Debug.Log("WorldPos:" + mouseWorldPos + "   GridPos:" + mouseGridPos);

        // �ж���Ʒ�����÷�Χ
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

            // WORKFLOW:����������Ʒ���͵��ж�
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
                        // ��ǰũ����Ϊ��Ϊ��
                        if (currentCrop.CheckToolAvailable(currentItem.itemID))
                        {
                            // �ҹ��߿���
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
    /// �Ƿ���UI����
    /// </summary>
    /// <returns>��UI�����򷵻�true������false</returns>
    private bool InteractWithUI()
    {
        if(EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;
        return false;
    }
}
