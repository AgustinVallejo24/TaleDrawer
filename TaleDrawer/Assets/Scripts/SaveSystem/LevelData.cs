using UnityEngine;

[System.Serializable]
public class LevelData
{
    public SaveState saveState = SaveState.NotFound;
    public int currentCheckPoint;
    public bool[] puzzlesCompleted;
    public bool completed;
    public bool[] killedEnemies;
}
public enum SaveState
{
    Saved,
    NotFound,
}