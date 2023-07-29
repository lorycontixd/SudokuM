using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class LobbyPunEventSender
{
    public const byte GameStartEventCode = 0;
    public const byte UpdateGameSettingsEventCode = 1;
    public const byte HostMigrateEventCode = 2;


    public static void SendGameStart()
    {
        object[] content = new object[] { };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(GameStartEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
    public static void SendHostMigration(Player player)
    {

        object[] content = new object[] { player.NickName};
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(HostMigrateEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }
}
