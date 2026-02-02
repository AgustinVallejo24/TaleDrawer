using UnityEngine;
using TMPro;


[CreateAssetMenu(menuName = "Dialogue Block")]
public class Dialogue : ScriptableObject
{
    public TextAsset inkJSON;
    public TMP_FontAsset baseFont;

    [Header("Voces")]
    public SpeakerVoiceData[] voices;
}
