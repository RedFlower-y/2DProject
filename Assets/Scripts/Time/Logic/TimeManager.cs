using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MFarm.Save;

public class TimeManager : Singloten<TimeManager>, ISaveable
{
    private int     gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    private Season  gameSeason = Season.����;
    private int     mouthInSeason = 3;
    public  bool    gameClockPause;         // ʱ����ͣ
    private float   tikTime;

    // �ƹ����
    private float   timeDifference;         // �ƹ��л�ʱ���

    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);     // ��ȡ��ǰ��Ϸʱ��

    public string GUID => GetComponent<DataGUID>().GUID;

    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent  += OnAfterSceneLoadedEvent;
        EventHandler.UpdateGameStateEvent   += OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent      += OnStartNewGameEvent;
        EventHandler.EndGameEvent           += OnEndGameEvent;
    }

    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent  -= OnAfterSceneLoadedEvent;
        EventHandler.UpdateGameStateEvent   -= OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent      -= OnStartNewGameEvent;
        EventHandler.EndGameEvent           -= OnEndGameEvent;
    }


    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();
        gameClockPause = true;

        //// Ϊʲôд��Start���� ����д��NewGameTIme()���棿
        //// ��ΪEventHandler.CallGameDateEvent()��EventHandler.CallGameMinuteEvent()�¼�ע������awake֮��ִ�У���Start�����¼�ע��֮��ִ��
        //EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        //EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);

        //// ��Ϸ��ʼ���л��ƹ�
        //EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
    }

    private void Update()
    {
        if(!gameClockPause)
        {
            tikTime += Time.deltaTime;
            if(tikTime >= Settings.secondThreshold)
            {
                tikTime -= Settings.secondThreshold;
                UpdateGameTime();
            }
        }

        if(Input.GetKey(KeyCode.T))
        {
            for (int i = 0; i < 60; i++)
                UpdateGameTime();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            gameDay++;
            EventHandler.CallGameDayEvent(gameDay, gameSeason);
            EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        gameClockPause = true;
    }

    private void OnAfterSceneLoadedEvent()
    {
        gameClockPause = false;

        // ��ȡ�浵�� �����AfterSceneLoadedEvent()
        EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
        EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        gameClockPause = gameState == GameState.GamePause;          // ��ͷTimeline������ֹͣʱ�����
    }

    private void OnStartNewGameEvent(int obj)
    {
        NewGameTime();
        gameClockPause = false;
    }

    private void OnEndGameEvent()
    {
        gameClockPause = true;
    }

    private void NewGameTime()
    {
        gameSecond  = 0;
        gameMinute  = 0;
        gameHour    = 7;
        gameDay     = 1;
        gameMonth   = 1;
        gameYear    = 2023;
        gameSeason  = Season.����;
    }

    /// <summary>
    /// ʱ�����
    /// </summary>
    public void UpdateGameTime()
    {
        gameSecond++;
        if(gameSecond > Settings.secondHold)
        {
            gameMinute++;
            gameSecond = 0;

            if(gameMinute > Settings.minuteHold)
            {
                gameHour++;
                gameMinute = 0;

                if (gameHour > Settings.hourHold)
                {
                    gameDay++;
                    gameHour = 0;

                    if (gameDay > Settings.dayHold)
                    {
                        gameDay = 1;
                        gameMonth++;

                        if (gameMonth > 12)
                        {
                            gameMonth = 1;
                        }

                        mouthInSeason--;
                        if (mouthInSeason == 0)
                        {
                            mouthInSeason = 3;

                            int seasonNumber = (int)gameSeason;
                            seasonNumber++;

                            if (seasonNumber > Settings.seasonHold)
                            {
                                seasonNumber = 0;
                                gameYear++;
                            }

                            gameSeason = (Season)seasonNumber;

                            if (gameYear > 9999)
                            {
                                gameYear = 2023;
                            }
                        }
                    }
                    // ����ˢ�µ�ͼ��ũ��������
                    EventHandler.CallGameDayEvent(gameDay, gameSeason);
                }
                EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
            }
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
            // �л��ƹ�
            EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
        }

        //Debug.Log("Second:" + gameSecond + "Minute:" + gameMinute);
    }

    /// <summary>
    /// ����lightShift ͬʱ����ʱ���
    /// </summary>
    /// <returns></returns>
    private LightShift GetCurrentLightShift()
    {
        if (GameTime >= Settings.morningTime && GameTime < Settings.nightTime)
        {
            timeDifference = (float)(GameTime - Settings.morningTime).TotalMinutes;
            return LightShift.Morning;
        }

        if (GameTime < Settings.morningTime || GameTime >= Settings.nightTime)
        {
            timeDifference = Mathf.Abs((float)(GameTime - Settings.nightTime).TotalMinutes);
            Debug.Log(timeDifference);
            return LightShift.Night;
        }

        return LightShift.Morning;
    }

    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.timeDict = new Dictionary<string, int>();
        saveData.timeDict.Add("gameYear"    , gameYear);
        saveData.timeDict.Add("gameSeason"  , (int)gameSeason);
        saveData.timeDict.Add("gameMonth"   , gameMonth);
        saveData.timeDict.Add("gameDay"     , gameDay);
        saveData.timeDict.Add("gameHour"    , gameHour);
        saveData.timeDict.Add("gameMinute"  , gameMinute);
        saveData.timeDict.Add("gameSecond"  , gameSecond);

        return saveData;
    }

    public void RestoreData(GameSaveData saveData)
    {
        gameYear    = saveData.timeDict["gameYear"];
        gameSeason  = (Season)saveData.timeDict["gameSeason"];
        gameMonth   = saveData.timeDict["gameMonth"];
        gameDay     = saveData.timeDict["gameDay"];
        gameHour    = saveData.timeDict["gameHour"];
        gameMinute  = saveData.timeDict["gameMinute"];
        gameSecond  = saveData.timeDict["gameSecond"];
    }
}
