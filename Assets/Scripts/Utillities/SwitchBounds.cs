using Cinemachine;
using UnityEngine;

public class SwitchBounds : MonoBehaviour
{
    //// TODO:�л���������ĵ���
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

        confiner.InvalidatePathCache();         // �����µĵ�ͼ�߽�ʱ��������� Call this if the bounding shape's point change at runtime.
    }
}
