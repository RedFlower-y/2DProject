using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Transition;

namespace MFarm.Save
{
    public class DataSlot
    {
        /// <summary>
        /// ������ string��GUID
        /// </summary>
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();

        #region �浵��UI��ʾ��������
        public string DataTime
        {
            get
            {
                var key = TimeManager.Instance.GUID;

                if (dataDict.ContainsKey(key))
                {
                    var timeData = dataDict[key];
                    return  timeData.timeDict["gameYear"]   + "��/" +
                    (Season)timeData.timeDict["gameSeason"] + "/" +
                            timeData.timeDict["gameMonth"]  + "��/" +
                            timeData.timeDict["gameDay"]    + "��/" +
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
                        "00.Start"  => "����",
                        "01.Field"  => "ũ��",
                        "02.Home"   => "Сľ��",
                        "03.Stall"  => "�г�",
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