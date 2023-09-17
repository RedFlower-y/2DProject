using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace MFarm.Save
{
    public class SaveLoadManager : Singloten<SaveLoadManager>
    {
        private List<ISaveable> saveableList = new List<ISaveable>();

        public List<DataSlot> dataSlots = new List<DataSlot>(new DataSlot[3]);

        private string jsonFolder;

        private int currentDataIndex;

        protected override void Awake()
        {
            base.Awake();
            jsonFolder = Application.persistentDataPath + "/SAVE DATA/";        // �ڴ˴��������json�ļ����ļ���
            ReadSaveData();
        }

        private void OnEnable()
        {
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
        }

        private void OnDisable()
        {
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
                Save(currentDataIndex);
            if (Input.GetKeyDown(KeyCode.O))
                Load(currentDataIndex);
        }

        private void OnStartNewGameEvent(int index)
        {
            currentDataIndex = index;
        }

        public void RegisterSaveable(ISaveable saveable)
        {
            if (!saveableList.Contains(saveable))
                saveableList.Add(saveable);
        }

        /// <summary>
        /// ������Ϸʱ����ѯ�浵
        /// </summary>
        private void ReadSaveData()
        {
            if (Directory.Exists(jsonFolder))
            {
                for (int i = 0; i < dataSlots.Count; i++)
                {
                    var resultPath = jsonFolder + "data" + i + ".sav";

                    if (File.Exists(resultPath))
                    {
                        var stringData = File.ReadAllText(resultPath);

                        var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);

                        dataSlots[i] = jsonData;
                    }
                }
            }
        }

        /// <summary>
        /// �浵
        /// </summary>
        /// <param name="index">�浵��λ���</param>
        private void Save(int index)
        {
            DataSlot data = new DataSlot();

            foreach (var saveable in saveableList)
            {
                data.dataDict.Add(saveable.GUID, saveable.GenerateSaveData());
            }

            dataSlots[index] = data;

            var resultPath = jsonFolder + "data" + index + ".sav";

            var jsonData = JsonConvert.SerializeObject(dataSlots[index], Formatting.Indented);  // ���������л� Formatting.Indented�����ݻ��У������Ķ�

            if (!File.Exists(resultPath))
            {
                Directory.CreateDirectory(jsonFolder);
            }

            Debug.Log("DATA" + index + "SAVED!");

            File.WriteAllText(resultPath, jsonData);        // �����л�����ļ��浽.sav���ļ���
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="index">������λ���</param>
        public void Load(int index)
        {
            currentDataIndex = index;

            var resultPath = jsonFolder + "data" + index + ".sav";

            var stringData = File.ReadAllText(resultPath);

            var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);

            foreach (var saveable in saveableList)
            {
                saveable.RestoreData(jsonData.dataDict[saveable.GUID]);
            }
        }
    }
}
