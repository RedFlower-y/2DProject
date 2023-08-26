using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    public const float itemFadeDuration = 0.35f;    // 遮挡物半透明速度

    public const float targetAlpha = 0.45f;     // 遮挡物半透明程度

    // 时间相关
    public const float secondThreshold = 0.01f;    // 数值越小，时间越快
    public const int secondHold = 59;
    public const int minuteHold = 59;
    public const int hourHold = 23;
    public const int dayHold = 30;
    public const int seasonHold = 3;

    // Transition
    public const float fadeDuration = 1.5f;

    // 收割农作物相关
    public const float waitTimeForHarvest = 1.0f;

    // Player相关
    public const float playSize = 1.96f;

    // 割草数量限制
    public const int reapAmount = 2;

    // NPC网格移动
    public const float gridCellSize = 1;                // 网格长度
    public const float gridCellDiagonaSize = 1.41f;     // 网格斜方向长度
    public const float pixelSize = 0.05f;               // 像素尺寸 20*20 占 1unit
    public const float animationBreakTime = 5f;         // 动画间隔时间
}
