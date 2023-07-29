
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using System;
using UnityEngine;

public class RoomListCache : MonoBehaviourPunCallbacks
{
    private TypedLobby customLobby = new TypedLobby("customLobby", LobbyType.Default);

    public Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    public int RoomCacheCount { get { return cachedRoomList.Count; } }
    public Action<Dictionary<string, RoomInfo>> onRoomListUpdate;


    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }

    public override void OnJoinedLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log($"[RoomListCache] OnROomListUpdate => count: {roomList.Count}");
        UpdateCachedRoomList(roomList);
        onRoomListUpdate?.Invoke(cachedRoomList);
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();
    }
}