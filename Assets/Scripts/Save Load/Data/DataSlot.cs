using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Transition;

namespace MFarm.Save
{
    public class DataSlot
    {
        /// <summary>
        /// 进度条 string：GUID
        /// </summary>
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();

        #region 存档栏UI显示进度详情
        public string DataTime
        {
            get
            {
                var key = TimeManager.Instance.GUID;

                if (dataDict.ContainsKey(key))
                {
                    var timeData = dataDict[key];
                    return  timeData.timeDict["gameYear"]   + "年/" +
                    (Season)timeData.timeDict["gameSeason"] + "/" +
                            timeData.timeDict["gameMonth"]  + "月/" +
                            timeData.timeDict["gameDay"]    + "日/" +
                            timeData.timeDict["gameHour"]   + ":" +
                            timeData.timeDict["gameMinute"];
                }
                else
                    return string.Empty;
            }
        }

        public string DataScene
        {
            get
            {
                var key = TransitionManager.Instance.GUID;

                if (dataDict.ContainsKey(key))
                {
                    var transitionData = dataDict[key];
                    return transitionData.dataSceneName switch
                    {
                        "00.Start"  => "海边",
                        "01.Field"  => "农场",
                        "02.Home"   => "小木屋",
                        "03.Stall"  => "市场",
                        _           => string.Empty,
                    };
                }
                else
                    return string.Empty;
            }
        }
        #endregion
    }
}