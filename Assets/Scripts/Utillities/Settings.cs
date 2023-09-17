using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public const float  gridCellSize = 1;                // ���񳤶�
    public const float  gridCellDiagonaSize = 1.41f;     // ����б���򳤶�
    public const float  pixelSize = 0.05f;               // ���سߴ� 20*20 ռ 1unit
    public const float  animationBreakTime = 5f;         // �������ʱ��
    public const int    maxGridSize = 9999;              // ����ͼ�ߴ磬NPC�糡���ƶ����

    // �ƹ�
    public const float lightChangeDuration = 25f;       // �ƹ��л�ʱ��
    public static TimeSpan morningTime  = new TimeSpan(5, 0, 0);
    public static TimeSpan nightTime    = new TimeSpan(19, 0, 0);

    // ����Ϸ���
    public static Vector3 playerStartPos = new Vector3(0, -10f, 0);
    public const int playerStartMoney = 100;
}
