using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class SoundObject
{
    [SerializeField] private string id;
    [SerializeField] private AudioSource source;

    public string Id { get => id; set => id = value; }
    public AudioSource Source { get => source; set => source = value; }
}


[System.Serializable]
public class CH_SoundManager : MonoBehaviour
{
    public static CH_SoundManager instance;

    [SerializeField] private SoundObject[] soundObjects;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(string soundId)
    {
        foreach (SoundObject soundObject in soundObjects)
        {
            if (soundObject.Id == soundId)
            {
                soundObject.Source.Play();
                return;
            }
        }

        Debug.LogWarning("Sound with ID " + soundId + " not found.");
    }
}
