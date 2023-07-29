using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]     // �ڱ༭ģʽ������
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
                EditorUtility.SetDirty(mapData);        // ʵʱ����
#endif
        }
    }

    private void UpdateTileProperties()
    {
        currentTileamp.CompressBounds();        // ����ͼ��Χ��СΪ�����˵�ͼԪ�صķ�Χ

        if(!Application.IsPlaying(this))
        {
            if(mapData != null)
            {
                Vector3Int startPos = currentTileamp.cellBounds.min;        // �ѻ��Ƶ�ͼ��Χ�����½�����
                Vector3Int endPos = currentTileamp.cellBounds.max;          // �ѻ��Ƶ�ͼ��Χ�����½�����

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
