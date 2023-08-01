using ExitGames.Client.Photon.StructWrapping;
using RESTClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class DataManager : MonoBehaviour
{
    #region Singleton
    private static DataManager _instance;
    public static DataManager Instance { get { return _instance; } }

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

    [Header("Data")]
    public GoogleData GoogleData;

    //private VersionData VersionData;

    [Header("Cloud files")]
    [SerializeField] private string googleDataCloudFile = "admin/googledata.json";
    [SerializeField] private string versionsCloudFile = "admin/versions.json";

    // Privates
    public bool IsReady { get; private set; }
    private bool ReadGoogleData = false;
    private bool ReadVersionData = false;


    private IEnumerator Start()
    {
        if (StorageManager.Instance != null)
        {
            yield return new WaitUntil(() =>  StorageManager.Instance.IsReady); 
            StorageManager.Instance.onStorageOperationComplete += OnStorageOperation;

            StorageManager.Instance.GetData(googleDataCloudFile);
        }
        StartCoroutine(CheckReady());
    }

    private IEnumerator CheckReady()
    {
        yield return new WaitUntil(() => ReadGoogleData);
        Debug.Log($"[DataManager] IsReady");
        IsReady = true;
    }


    private void OnStorageOperation(string fileUrl, StorageManager.Operation operation, bool isSuccess, RestResponse response, string content)
    {
        if (fileUrl == StorageManager.Instance.BuildUrl(googleDataCloudFile))
        {
            GoogleData = JsonConvert.DeserializeObject<GoogleData>(content);
            Debug.Log($"Successfully read google data: {GoogleData} =>  {GoogleData.ProjectID}, {GoogleData.Achievements[0].Name}");
            ReadGoogleData = true;
        }
        if (fileUrl == StorageManager.Instance.BuildUrl(versionsCloudFile))
        {
            
        } 
    }
}
