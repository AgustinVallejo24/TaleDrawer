using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ReferenceSymbol
{

    public Texture2D templateTexture;
    [HideInInspector]
    public string symbolID;
    [HideInInspector]
    public float[] distribution;
    [HideInInspector]
    public List<double> momentMagnitudes;
    [HideInInspector]
    public int strokes = 1;
    [HideInInspector]
    public string symbolName;
    public ReferenceSymbol(string name, float[] rotDistribution, List<double> magnitudes, int strokesQ, string sID)
    {
        symbolName = name;
        distribution = rotDistribution;
        momentMagnitudes = magnitudes;
        strokes = strokesQ;
        symbolID = sID;
    }

   
}
