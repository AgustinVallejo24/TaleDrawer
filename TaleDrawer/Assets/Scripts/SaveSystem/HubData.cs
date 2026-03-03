using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public class HubData
{
    public int storyProgress;
    public bool[] collectiblesFound;
    public List<HintEntry> hints = new List<HintEntry>();
    public bool[] symbolsFound;
    public int highestLevelUnlocked;

}
[System.Serializable]
public class HintEntry
{
    public Hints hint;
    public bool unlocked;
}
