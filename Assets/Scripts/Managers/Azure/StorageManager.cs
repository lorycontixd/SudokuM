using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Azure.StorageServices;
using RESTClient;
using System;
using System.Net;

public class StorageManager : MonoBehaviour
{
    #region Singleton
    private static StorageManager _instance;
    public static StorageManager Instance { get { return _instance; } }

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

    public enum Operation
    {
        GET,
        PUT,
        DELETE
    }

    ////// Credentials
    [SerializeField] private string storageAccountName;
    [SerializeField] private string storageUrl;
    [SerializeField] private string storageAccessKey;
    [SerializeField] private string storageConnectionKey;

    ////// Privates
    public bool IsReady { get; private set; } = false;
    private StorageServiceClient client;
    private BlobService blobService;
    private string CurrentRequest = "";

    ////// Events
    public Action<string, Operation, bool, RestResponse, string> onStorageOperationComplete; // (request name, operation type, success, response, data)


    private void Start()
    {
        client = StorageServiceClient.Create(storageAccountName, storageAccessKey);
        blobService = client.GetBlobService();
        if (blobService != null)
        {
            Debug.Log($"[StorageManager] Successfully connected to blob: {client.Account}, {client.Url}");
        }
        IsReady = true;
    }

    public string BuildUrl(string file)
    {
        return $"{storageUrl}/{file}";
    }

    public void GetData(string file)
    {
        StartCoroutine(blobService.GetTextBlob(GetTextBlobComplete, file));
    }
    private void GetTextBlobComplete(RestResponse response)
    {
        onStorageOperationComplete?.Invoke(response.Url, Operation.GET, !response.IsError, response, response.Content);
        if (response.IsError)
        {
            Debug.LogWarning($"[StorageManager] Error getting blob: {response.ErrorMessage}");
            return;
        }
    }
}
