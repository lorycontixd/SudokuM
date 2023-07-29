using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    public bool IsPanelOpen { get { return _isPanelOpen; } }

    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private TextMeshProUGUI pingText;
    [SerializeField] private TextMeshProUGUI regionText;
    [SerializeField] private TextMeshProUGUI versionText;

    private float currentFPS = 0;
    private float currentPing = 0;
    private bool _isPanelOpen = false;

    private void Start()
    {
        Close();
    }
    private void Update()
    {
        currentFPS = (int)(1f / Time.unscaledDeltaTime);
        if (PhotonNetwork.IsConnected)
        {
            currentPing = PhotonNetwork.GetPing();
        }
        UpdateDebugPanel();
    }

    public void Open()
    {
        _isPanelOpen = true;
        gameObject.SetActive(true);
    }
    public void Close()
    {
        _isPanelOpen = false;
        gameObject.SetActive(false);
    }
    public void Toggle()
    {
        gameObject.SetActive(!_isPanelOpen);
        _isPanelOpen = !_isPanelOpen;
    }

    private void UpdateDebugPanel()
    {
        if (_isPanelOpen)
        {
            fpsText.text = $"FPS: {currentFPS}";
            pingText.text = (PhotonNetwork.IsConnected) ? $"Ping: {currentPing}" : "Ping: Not connected";
            regionText.text = (PhotonNetwork.IsConnected) ? $"Region: {PhotonNetwork.CloudRegion}" : "Region: Not connected";
            versionText.text = $"Game Version: {VersionManager.Instance.GetVersionString()}";
        }
    }
}
