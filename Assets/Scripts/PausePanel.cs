using Michsky.MUIP;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PausePanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private ProgressBar progessBar;
    [SerializeField] private TextMeshProUGUI requestingPlayerText;
    [SerializeField] private TextMeshProUGUI myPausesText;
    [SerializeField] private TextMeshProUGUI otherPausesText;
    [SerializeField] private ButtonManager unpauseButton;
    private bool isOn;

    public void Open()
    {
        Debug.Log("Opening pausepanel");
        gameObject.SetActive(true);
        isOn = true;
        unpauseButton.gameObject.SetActive(PauseManager.Instance.PausingPlayer == PhotonNetwork.LocalPlayer);
        progessBar.maxValue = PauseManager.Instance.PauseDuration;
        progessBar.ChangeValue(progessBar.maxValue);

    }
    public void Close()
    {
        isOn = false;
        gameObject.SetActive(false);
    }
    private void Update()
    {
        if (isOn && PauseManager.Instance.IsPaused)
        {
            if (progessBar != null)
            {
                progessBar.ChangeValue(PauseManager.Instance.PauseTime);
            }
        }
    }
    public void UpdateUI()
    {
        if (PauseManager.Instance.IsPaused)
        {
            requestingPlayerText.text = $"Pause requested by user {PauseManager.Instance.PausingPlayer.NickName}";
            myPausesText.text = $"My pauses left: {PauseManager.Instance.MaxPauses - PauseManager.Instance.MyPauses}";
            otherPausesText.text = $"Other player pauses left: {PauseManager.Instance.MaxPauses - PauseManager.Instance.OtherPauses}";
        }
    }
}
