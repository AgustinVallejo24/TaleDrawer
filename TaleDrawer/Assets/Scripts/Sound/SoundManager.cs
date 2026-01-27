using UnityEngine;
using System.Linq;
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioEvent[] audioEvents;
    public static AudioEvent[] staticAudioEvents;
    private void Awake()
    {
        staticAudioEvents = audioEvents;
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void Play(SoundsType soundType, Vector3 position)
    {
        if (soundType == SoundsType.Null) return;

        AudioEvent audioEvent = staticAudioEvents.Where(x => x.type == soundType).First();

        GameObject go = new GameObject("Audio_" + soundType);
        go.transform.position = position;

        AudioSource source = go.AddComponent<AudioSource>();
        source.clip = audioEvent.GetClip();
        source.volume = audioEvent.volume;
        source.pitch = audioEvent.GetPitch();
        source.outputAudioMixerGroup = audioEvent.mixerGroup;

        source.Play();
        Destroy(go, source.clip.length / source.pitch);
    }
}
public enum SoundsType
{
    Null,
    StoneSteps,
    PaperSteps,
    Jump,
    Death,

}