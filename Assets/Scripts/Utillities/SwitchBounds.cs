using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
    //// TODO:切换场景后更改调用
    //private void Start()
    //{
    //    SwitchConfinerShape();
    //}

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadedEvent += SwitchConfinerShape;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadedEvent -= SwitchConfinerShape;
    }

    private void SwitchConfinerShape()
    {
        PolygonCollider2D confinerShape = GameObject.FindGameObjectWithTag("BoundsConfiner").GetComponent<PolygonCollider2D>();

        CinemachineConfiner confiner = GetComponent<CinemachineConfiner>();

        confiner.m_BoundingShape2D = confinerShape;

        confiner.InvalidatePathCache();         // 调用新的地图边界时，清除缓存 Call this if the bounding shape's point change at runtime.
    }
}
