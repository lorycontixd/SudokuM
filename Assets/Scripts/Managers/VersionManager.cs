using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersionManager : MonoBehaviour
{
    #region Singleton
    private static VersionManager _instance;
    public static VersionManager Instance { get { return _instance; } }

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

    public int MajorVersion;
    public int MinorVersion;
    public int PatchVersion;
    private Version version { get { return new Version(MajorVersion, MinorVersion, PatchVersion); } }
    public bool ShareVersionWithPhoton = true;

    private void Start()
    {
        if (ShareVersionWithPhoton)
        {
            PhotonNetwork.GameVersion = version.ToString();
        }
    }

    public string GetVersionString()
    {
        return $"{MajorVersion}.{MinorVersion}.{PatchVersion}";
    }
}
