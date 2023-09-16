using UnityEngine;

[ExecuteAlways]         // 一直运行
public class DataGUID : MonoBehaviour
{
    public string GUID;
    private void Awake()
    {
        if (GUID == string.Empty)
        {
            GUID = System.Guid.NewGuid().ToString();
        }
    }
}
