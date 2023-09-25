using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MFarm.Save;

public class TimeManager : Singloten<TimeManager>, ISaveable
{
    private int     gameSecond, gameMinute, gameHour, gameDay, gameMonth, gameYear;
    private Season  gameSeason = Season.春天;
    private int     mouthInSeason = 3;
    public  bool    gameClockPause;         // 时间暂停
    private float   tikTime;

    // 灯光相关
    private float   timeDifference;         // 灯光切换时间差

    public TimeSpan GameTime => new TimeSpan(gameHour, gameMinute, gameSecond);     // 获取当前游戏时间

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

        //// 为什么写在Start里面 而不写在NewGameTIme()里面？
        //// 因为EventHandler.CallGameDateEvent()和EventHandler.CallGameMinuteEvent()事件注册是在awake之后执行，而Start是在事件注册之后执行
        //EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        //EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);

        //// 游戏开始就切换灯光
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

        // 读取存档后 会加载AfterSceneLoadedEvent()
        EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
        EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
        EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
    }

    private void OnUpdateGameStateEvent(GameState gameState)
    {
        gameClockPause = gameState == GameState.GamePause;          // 开头Timeline过程中停止时间计数
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
        gameSeason  = Season.春天;
    }

    /// <summary>
    /// 时间更新
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
                    // 用来刷新地图和农作物生长
                    EventHandler.CallGameDayEvent(gameDay, gameSeason);
                }
                EventHandler.CallGameDateEvent(gameHour, gameDay, gameMonth, gameYear, gameSeason);
            }
            EventHandler.CallGameMinuteEvent(gameMinute, gameHour, gameDay, gameSeason);
            // 切换灯光
            EventHandler.CallLightShiftChangeEvent(gameSeason, GetCurrentLightShift(), timeDifference);
        }

        //Debug.Log("Second:" + gameSecond + "Minute:" + gameMinute);
    }

    /// <summary>
    /// 返回lightShift 同时计算时间差
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
