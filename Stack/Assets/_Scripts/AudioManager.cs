using System.Collections.Generic;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] Sound[] sounds;

    [SerializeField] AudioClip[] cut;
    [SerializeField] AudioClip[] combo;

    List<AudioSource> cutSources = new List<AudioSource>();
    List<AudioSource> comboSources = new List<AudioSource>();

    [HideInInspector]
    public int currentCombo;

    void Awake()
    {
        instance = this;

        foreach (Sound item in sounds)
        {
            item.source = gameObject.AddComponent<AudioSource>();
            item.source.clip = item.clip;
        }
        for (int i = 0; i < cut.Length; i++)
        {
            AudioSource sorce = gameObject.AddComponent<AudioSource>();
            sorce.clip = cut[i];
            cutSources.Add(sorce);
        }
        for (int i = 0; i < combo.Length; i++)
        {
            AudioSource sorce = gameObject.AddComponent<AudioSource>();
            sorce.clip = combo[i];
            comboSources.Add(sorce);
        }
    }

    public void Cut()
    {
        cutSources[Random.Range(0, cutSources.Count)].Play();
        currentCombo = 0;
    }

    public void Combo()
    {
        if (currentCombo < comboSources.Count)
        {
            comboSources[currentCombo].Play();
            currentCombo++;
        } else
        {
            currentCombo = 0;
            comboSources[currentCombo].Play();
            currentCombo++;
        }
    }

    public void Play(string name)
    {
        Sound currentSound = Array.Find(sounds, sound => sound.name == name);
        currentSound.source.Play();
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;

    [HideInInspector]
    public AudioSource source;
}