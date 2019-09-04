using System;
using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public const string MENU_SONG = "Whitesand";
    public const string LEVEL_SONG = "AnsiaOrchestra";

    [SerializeField] private Sound[] sounds = null;

    private static AudioManager instance = null;
    public static AudioManager GetInstance() { return instance; }

    private bool audioActive = true;

    public Sound currentMusic = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        int length = sounds.Length;
        for (int i = 0; i < length; i++)
        {
            sounds[i].source = gameObject.AddComponent<AudioSource>();
            sounds[i].source.clip = sounds[i].clip;
            sounds[i].source.volume = sounds[i].volume;
            sounds[i].source.pitch = sounds[i].pitch;
            sounds[i].source.loop = sounds[i].loop;
        }
    }

    private void StopAllSound()
    {
        int length = sounds.Length;
        for (int i = 0; i < length; i++)
        {
            if (sounds[i].source.isPlaying)
            {
                sounds[i].source.Stop();
}
        }
    }

    public void SmoothOutSound(Sound s, float stride, float duration)
    {
        StartCoroutine(SmoothOutSound_C(s, stride, duration));
    }
    private IEnumerator SmoothOutSound_C(Sound s, float stride, float duration)
    {
        if (s != null)
        {
            float currentVolume = s.volume;
            while (currentVolume - stride > 0f)
            {
                currentVolume -= stride;
                s.source.volume = currentVolume;
                yield return new WaitForSecondsRealtime(duration * stride);
            }
            s.source.Stop();
            s.source.volume = s.volume;
            s = null;
        }
    }

    public Sound SmoothInSound(string name, float stride, float duration)
    {
        if (!audioActive)
        {
            return null;
        }

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (!s.source.isPlaying)
        {
            StartCoroutine(SmoothInSound_C(s, stride, duration));
        }
        return s;
    }
    private IEnumerator SmoothInSound_C(Sound s, float stride, float duration)
    {
        if (s != null)
        {
            float currentVolume = s.source.volume = 0f;
            s.source.Play();
            while (currentVolume + stride < s.volume)
            {
                currentVolume += stride;
                s.source.volume = currentVolume;
                yield return new WaitForSecondsRealtime(duration * stride);
            }
            s.source.volume = s.volume;
        }
    }

    public Sound PlaySound(string name)
    {
        if (!audioActive)
        {
            return null;
        }

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (!s.source.isPlaying)
        {
            s.source.Play();
        }

        return s;
    }

    public void StopSound(Sound s)
    {
        s.source.Stop();
    }

    public void NotifyAudioSettings(SettingsData settingsData)
    {
        if (!settingsData.audioActive)
        {
            StopAllSound();
        }

        audioActive = settingsData.audioActive;
    }
}
