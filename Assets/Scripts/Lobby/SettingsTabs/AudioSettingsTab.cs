using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettingsTab : BaseSettingsTab
{
    public override SettingTabType Type => SettingTabType.AUDIO;

    public AudioSettings audioSettings;

    private void Start()
    {
        audioSettings = new AudioSettings();
    }

    public override bool Close()
    {
        return true;
    }

    public override bool Load()
    {
        return true;
    }

    public override bool Open()
    {
        return true;
    }

    public override bool Save()
    {
        return true;
    }

    #region UI 
    public void OnMasterVolumeChange(float value)
    {
    }
    #endregion

    public float SliderToVolume(float value)
    {
        float newValue = ((value - 0) * (20f + 80f) / (100f - 0f)) - 80f;
        return newValue;
    }
}
