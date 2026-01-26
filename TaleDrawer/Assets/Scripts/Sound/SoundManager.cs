using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;



    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void Play(AudioEvent audioEvent, Vector3 position)
    {
        if (audioEvent == null) return;

        GameObject go = new GameObject("Audio_" + audioEvent.name);
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
