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
    [SerializeField] private Image lockedImage;
    [SerializeField] private Image unlockedImage;
    [SerializeField] private Image rankedImage;
    [SerializeField] private Image unrankedImage;

    public string RoomName { get { return roomInfo.Name; } }
    public string RoomCode { get { return roomInfo.CustomProperties["code"].ToString(); } }
    public Action<MatchmakingListItem> onButtonClick;

    private RoomInfo roomInfo;

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        if (roomInfo != null)
        {
            bool isRanked = (bool)roomInfo.CustomProperties["r"];
            this.roomInfo = roomInfo;
            roomNameText.text = roomInfo.Name;
            playerCountText.text = $"{roomInfo.PlayerCount}/{roomInfo.MaxPlayers}";
            unlockedImage.gameObject.SetActive(roomInfo.IsOpen);
            lockedImage.gameObject.SetActive(!roomInfo.IsOpen);
            rankedImage.gameObject.SetActive(isRanked);
            unrankedImage.gameObject.SetActive(!isRanked);
        }
    }


    public void OnButtonClick()
    {
        onButtonClick?.Invoke(this);
    }
}
