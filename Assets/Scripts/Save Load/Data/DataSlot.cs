using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Save
{
    /// <summary>
    /// ½ø¶ÈÌõ string£ºGUID
    /// </summary>
    public class DataSlot
    {
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();
    }
}