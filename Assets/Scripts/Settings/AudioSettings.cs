using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings : BaseSettings
{
    public float MasterVolume;
    public float MusicVolume;
    public float SoundEffectsVolume;
    public bool PlaySoundNotification;

    public override bool Load()
    {
        return true;
    }

    public override bool Save()
    {
        return true;
    }
}
