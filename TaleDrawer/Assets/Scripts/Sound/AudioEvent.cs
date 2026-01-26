using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu]
public class AudioEvent : ScriptableObject
{
    public AudioClip[] clips;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.1f, 3f)]
    public float pitch = 1f;

    public bool randomPitch = false;
    public Vector2 pitchRange = new Vector2(0.9f, 1.1f);

    public AudioMixerGroup mixerGroup;

    public AudioClip GetClip()
    {
        return clips[Random.Range(0, clips.Length)];
    }

    public float GetPitch()
    {
        if (randomPitch)
            return Random.Range(pitchRange.x, pitchRange.y);

        return pitch;
    }
}
