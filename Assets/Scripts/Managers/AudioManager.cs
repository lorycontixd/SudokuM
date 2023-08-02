using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    #region Sound
    [Serializable]
    public struct Sound
    {
        public string name;
        public AudioClip clip;
    }
    #endregion

    #region Singleton
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    public List<Sound> sounds;
    public string HalfBoardSoundKey;

    private AudioSource m_AudioSource;
    private bool hasAudioSource;


    private void Start()
    {
        hasAudioSource = TryGetComponent<AudioSource>(out m_AudioSource);
        m_AudioSource.playOnAwake = false;
        m_AudioSource.volume = 1f;
        m_AudioSource.loop = false;
    }


    public Sound GetSound(string name)
    {
        return sounds.FirstOrDefault(s => s.name == name);
    }

    public bool PlayHalfBoardNotification()
    {
        try
        {
            Sound sound = GetSound(HalfBoardSoundKey);
            AudioClip clip = sound.clip;
            if (clip != null)
            {
                m_AudioSource.PlayOneShot(clip);
                return true;
            }
            else
            {
                return false;
            }
        }
        catch(Exception e)
        {
            Debug.LogWarning($"PlayHalfBoard failed ==> {e}");
            return false;
        }
    }
}
