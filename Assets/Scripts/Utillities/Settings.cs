using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    public const float itemFadeDuration = 0.35f;    // �ڵ����͸���ٶ�

    public const float targetAlpha = 0.45f;     // �ڵ����͸���̶�

    // ʱ�����
    public const float secondThreshold = 0.01f;    // ��ֵԽС��ʱ��Խ��
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    public const int dayHold = 30;
    public const int seasonHold = 3;

    // Transition
    public const float fadeDuration = 1.5f;

    // �ո�ũ�������
    public const float waitTimeForHarvest = 1.0f;

    // Player���
    public const float playSize = 1.96f;

    // �����������
    public const int reapAmount = 2;

    // NPC�����ƶ�
    public const float gridCellSize = 1;               // ���񳤶�
    public const float gridCellDiagonaSize = 1.41f;    // ����б���򳤶�
    public const float pixelSize = 0.05f;              // ���سߴ� 20*20 ռ 1unit
}
