using UnityEngine;

[System.Serializable]
public class SaveData
{
    public HubData hubData = new HubData();
    public LevelData[] levelsData = new LevelData[100];
    public bool testBool;
}
