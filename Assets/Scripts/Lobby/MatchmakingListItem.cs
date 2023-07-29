using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MatchmakingListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Image visibleImage;
    [SerializeField] private Image unvisibleImage;
    [SerializeField] private Image lockedImage;
    [SerializeField] private Image unlockedImage;

    public string RoomName { get { return roomInfo.Name; } }
    public string RoomCode { get { return roomInfo.CustomProperties["code"].ToString(); } }
    public Action<MatchmakingListItem> onButtonClick;

    private RoomInfo roomInfo;

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        if (roomInfo != null)
        {
            this.roomInfo = roomInfo;
            roomNameText.text = roomInfo.Name;
            playerCountText.text = $"{roomInfo.PlayerCount}/{roomInfo.MaxPlayers}";
            visibleImage.gameObject.SetActive(roomInfo.IsVisible);
            unvisibleImage.gameObject.SetActive(!roomInfo.IsVisible);
            unlockedImage.gameObject.SetActive(roomInfo.IsOpen);
            lockedImage.gameObject.SetActive(!roomInfo.IsOpen);
        }
    }


    public void OnButtonClick()
    {
        onButtonClick?.Invoke(this);
    }
}
