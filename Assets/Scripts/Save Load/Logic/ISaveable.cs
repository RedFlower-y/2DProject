namespace MFarm.Save
{
    public interface ISaveable
    {
        string GUID { get; }            // 保证每一个需要储存的文件 都有GUID
        void RegisterSaveable()
        {
            SaveLoadManager.Instance.RegisterSaveable(this);
        }
        GameSaveData GenerateSaveData();
        void RestoreData(GameSaveData saveData);
    }
}
