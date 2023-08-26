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
    /// ��ʾ����Сʱ
    /// </summary>
    /// <param name="minute">����</param>
    /// <param name="hour">Сʱ</param>
    private void OnGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        timeText.text = hour.ToString("00") + ":" + minute.ToString("00");
    }

    /// <summary>
    /// ��ʾ�����գ��Լ�ʱ���Ͱ����ҹ��ʾ
    /// </summary>
    /// <param name="hour">Сʱ</param>
    /// <param name="day">��</param>
    /// <param name="month">��</param>
    /// <param name="year">��</param>
    /// <param name="season">����</param>
    private void OnGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        dataText.text = year + "��" + month.ToString("00") + "��" + day.ToString("00") + "��";
        seasonImage.sprite = seasonSprites[(int)season];

        SwitchHourImage(hour);
        DayNightImageRotate(hour);
    }


    /// <summary>
    /// ����Сʱ�л�ʱ�����ʾ
    /// </summary>
    /// <param name="hour">Сʱ</param>
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
                if (i < index + 1)  // ���ʱ����23 ����Ҫ��һ
                    clockBlocks[i].SetActive(true);
                else
                    clockBlocks[i].SetActive(false);
            }
        }
    }

    /// <summary>
    /// ����Сʱ�л������ҹ��ʾ
    /// </summary>
    /// <param name="hour">Сʱ</param>
    private void DayNightImageRotate(int hour)
    {
        var target = new Vector3(0, 0, hour * 15 - 90);
        dayNightImage.DORotate(target, 1f, RotateMode.Fast);
    }
}
    
