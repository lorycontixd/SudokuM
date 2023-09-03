using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSettingsTab : MonoBehaviour
{
    #region Setting Tab Type
    public enum SettingTabType
    {
        GENERAL,
        AUDIO,
        NETWORK
    }
    #endregion

    #region Setting Change
    [Serializable]
    public struct SettingChange
    {
        public string Key;
        public object OldValue;
        public object NewValue;

        public SettingChange(string key, object oldValue, object newValue)
        {
            Key = key;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
    #endregion


    public abstract SettingTabType Type { get; }
    protected bool HasPendingChange { get => PendingChanges.Count > 0; }
    protected List<SettingChange> PendingChanges = new List<SettingChange>();


    public abstract bool Open();
    public abstract bool Close();

    public abstract bool Save();
    public abstract bool Load();

}
