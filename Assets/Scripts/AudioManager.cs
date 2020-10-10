using UnityEngine.Audio;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance;

    public Slider sfx;
    public Slider music;
    public float sfxVol;
    public float musicVol;
    bool sceneBool = true;
    private void Start()
    {
        Play("Menu");
    }

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

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.spatialBlend = s.spatialBlend;
        }
    }

    private void Update()
    {
        if (sfx != null && music != null)
        {
            sfxVol = sfx.value;
            musicVol = music.value;

            ChangeVolume("Explosion", sfx.value / 4);
            ChangeVolume("Blaster", sfx.value / 4);

            
            if (sceneBool)
            {
                ChangeVolume("Menu", music.value / 2);
                ChangeVolume("Theme", 0);
            }
            else
            {
                ChangeVolume("Menu", 0);
                ChangeVolume("Theme", music.value / 4);
            }
        }
    }

    public void ChangeVolume(string name, float vol)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " is not found!");
            return;
        }
        s.source.volume = vol;
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Sound: " + name + " is not found!");
            return;
        }

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        s.source.Play();
    }

    public void RefreshAudioManager()
    {
        if (GameObject.Find("SFXSlider") && GameObject.Find("MusicSlider"))
        {
            sfx = GameObject.Find("SFXSlider").GetComponent<Slider>();
            music = GameObject.Find("MusicSlider").GetComponent<Slider>();
            sfx.value = sfxVol;
            music.value = musicVol;
        }
    }
    
    public void ToggleSceneBool()
    {
        sceneBool = !sceneBool;
    }

}
