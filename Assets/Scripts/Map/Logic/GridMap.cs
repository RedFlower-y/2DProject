using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]     // 在编辑模式下运行
public class GridMap : MonoBehaviour
{
    public MapData_SO mapData;
    public GridType gridType;
    private Tilemap currentTileamp;

    private void OnEnable()
    {
        if(!Application.IsPlaying(this))
        {
            currentTileamp = GetComponent<Tilemap>();

            if(mapData != null)
                mapData.tileProperties.Clear();

        }
    }

    private void OnDisable()
    {
        if(!Application.IsPlaying(this))
        {
            currentTileamp = GetComponent<Tilemap>();

            UpdateTileProperties();

#if UNITY_EDITOR
            if(mapData != null)
                EditorUtility.SetDirty(mapData);        // 实时保存
#endif
        }
    }

    private void UpdateTileProperties()
    {
        currentTileamp.CompressBounds();        // 将地图范围缩小为绘制了地图元素的范围

        if(!Application.IsPlaying(this))
        {
            if(mapData != null)
            {
                Vector3Int startPos = currentTileamp.cellBounds.min;        // 已绘制地图范围的左下角坐标
                Vector3Int endPos = currentTileamp.cellBounds.max;          // 已绘制地图范围的右下角坐标

                for (int x = startPos.x; x < endPos.x; x++)
                {
                    for (int y = startPos.y; y < endPos.y; y++)
                    {
                        TileBase tile = currentTileamp.GetTile(new Vector3Int(x, y, 0));

                        if(tile != null)
                        {
                            TileProperty newTile = new TileProperty
                            {
                                tileCoordinate = new Vector2Int(x, y),
                                gridType = this.gridType,
                                boolTypeValue = true,
                            };

                            mapData.tileProperties.Add(newTile);
                        }
                    }
                }
            }
        }
    }
}
