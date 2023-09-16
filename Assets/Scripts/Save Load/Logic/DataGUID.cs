using UnityEngine;

[ExecuteAlways]         // һֱ����
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
