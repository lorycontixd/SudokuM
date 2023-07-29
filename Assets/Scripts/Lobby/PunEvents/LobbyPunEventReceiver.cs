using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPunEventReceiver : MonoBehaviour, IOnEventCallback
{
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        
        switch(eventCode)
        {
            case LobbyPunEventSender.GameStartEventCode:
                if (LobbyMenuController.Instance.activeMenu.Type != MenuType.LOBBY)
                {
                    return;
                    //Debug.LogError($"Player must be in a game lobby to receive events");
                }
                LobbyMenu menu = (LobbyMenu)LobbyMenuController.Instance.activeMenu;
                if (menu != null)
                {
                    menu.StartGame();
                }
                break;

            case LobbyPunEventSender.HostMigrateEventCode:
                object[] data = (object[])photonEvent.CustomData;
                string username = (string)data[0];
                if (username == PhotonNetwork.LocalPlayer.NickName)
                {
                    Managers.NotificationManager.Instance.Info("Host migration", "You are the new host of the room");
                }
                break;
        }
    }
}
