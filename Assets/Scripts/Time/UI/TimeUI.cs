using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class TimeUI : MonoBehaviour
{
    public RectTransform    dayNightImage;
    public RectTransform    clockParent;
    public Image            seasonImage;
    public TextMeshProUGUI  dataText;
    public TextMeshProUGUI  timeText;
    public Sprite[]         seasonSprites;

    private List<GameObject> clockBlocks = new List<GameObject>();

    private void Awake()
    {
        for (int i = 0; i < clockParent.childCount; i++)
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);
            clockParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;
        EventHandler.GameDateEvent += OnGameDateEvent;
    }

    private void OnDisable()
    {
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;
        EventHandler.GameDateEvent -= OnGameDateEvent;
    }

    /// <summary>
    /// 显示分钟小时
    /// </summary>
    /// <param name="minute">分钟</param>
    /// <param name="hour">小时</param>
    private void OnGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");
    }

    /// <summary>
    /// 显示年月日，以及时间块和白天黑夜显示
    /// </summary>
    /// <param name="hour">小时</param>
    /// <param name="day">日</param>
    /// <param name="month">月</param>
    /// <param name="year">年</param>
    /// <param name="season">季节</param>
    private void OnGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        dataText.text = year + "年" + month.ToString("00") + "月" + day.ToString("00") + "日";
        seasonImage.sprite = seasonSprites[(int)season];

        SwitchHourImage(hour);
        DayNightImageRotate(hour);
    }


    /// <summary>
    /// 根据小时切换时间块显示
    /// </summary>
    /// <param name="hour">小时</param>
    private void SwitchHourImage(int hour)
    {
        int index = hour / 4;

        if(index == 0)
        {
            foreach (var item in clockBlocks)
                item.SetActive(false);
        }
        else
        {
            for(int i = 0; i < clockBlocks.Count; i++)
            {
                if (i < index + 1)  // 最大时数是23 所以要加一
                    clockBlocks[i].SetActive(true);
                else
                    clockBlocks[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// 根据小时切换白天黑夜显示
    /// </summary>
    /// <param name="hour">小时</param>
    private void DayNightImageRotate(int hour)
    {
        var target = new Vector3(0, 0, hour * 15 - 90);
        dayNightImage.DORotate(target, 1f, RotateMode.Fast);
    }
}
    
