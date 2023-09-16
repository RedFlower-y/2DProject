namespace MFarm.Save
{
    public interface ISaveable
    {
        string GUID { get; }            // ��֤ÿһ����Ҫ������ļ� ����GUID
        void RegisterSaveable()
        {
            SaveLoadManager.Instance.RegisterSaveable(this);
        }
        GameSaveData GenerateSaveData();
        void RestoreData(GameSaveData saveData);
    }
}
