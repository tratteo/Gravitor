using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 0.9f;
    [Range(0f, 3f)]
    public float pitch = 1f;
    public bool loop;
    [HideInInspector] public AudioSource source;
}
