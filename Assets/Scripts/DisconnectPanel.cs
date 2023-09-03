using Michsky.MUIP;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DisconnectPanel : MonoBehaviour
{
    public float LoadingImageRotationSpeed = 10f;

    [Header("UI Components")]
    [SerializeField] private Transform parentPanel;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button closeButton;
    [SerializeField] private Image loadingImage;



    private void Start()
    {
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            loadingImage.rectTransform.Rotate(new Vector3(0f, 0f, LoadingImageRotationSpeed * Time.deltaTime)) ;
        }
    }


    public void Open()
    {
        loadingImage.rectTransform.rotation = Quaternion.identity;
        gameObject.SetActive(true);
        parentPanel.gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
        parentPanel.gameObject.SetActive(false);
    }
    public void UpdateUI()
    {
        bool iDisconnected = !NetworkManager.Instance.IsConnectedToGame;
        if (!iDisconnected)
        {
            Assert.IsTrue(NetworkManager.Instance.DisconnectedPlayer != null, "If i dont disconnect, disconnecting player must not be null");
        }
        Debug.Log($"Updating Disconnect Panel UI ==> IsMeDIsconnect? {iDisconnected}");
        // Update ui
        if (iDisconnected)
        {
            titleText.text = $"Lost connection to server";
            descriptionText.text = "We are trying to reconnect you to the game. In the meantime, please check your internete connection.";
        }
        else
        {
            string uname = NetworkManager.Instance.DisconnectedPlayer.NickName;
            titleText.text = $"{uname} disconnected";
            descriptionText.text = $"Player {uname} lost connection to the server. Please wait while we try to reestablish the connection.";
        }

    }

}
