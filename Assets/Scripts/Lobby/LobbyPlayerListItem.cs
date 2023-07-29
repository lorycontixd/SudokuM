using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyPlayerListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;

    public void SetPlayer(Player player)
    {
        nameText.text = player.NickName;
    }
}
