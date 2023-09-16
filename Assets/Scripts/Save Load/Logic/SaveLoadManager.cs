using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Save
{
    public class SaveLoadManager : Singloten<SaveLoadManager>
    {
        private List<ISaveable> saveableList = new List<ISaveable>();

        public void RegisterSaveable(ISaveable saveable)
        {
            if (!saveableList.Contains(saveable))
                saveableList.Add(saveable);
        }
    }
}
