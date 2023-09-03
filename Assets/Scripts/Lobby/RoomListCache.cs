
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using System;
using UnityEngine;

public class RoomListCache : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static RoomListCache _instance;
    public static RoomListCache Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    private TypedLobby customLobby = new TypedLobby("MainLobby", LobbyType.Default);

    public Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    public int RoomCacheCount { get { return cachedRoomList.Count; } }
    public Action<Dictionary<string, RoomInfo>> onRoomListUpdate;

    private void Start()
    {
        Debug.Log($"[RoomListCache] Start");
    }
    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby(customLobby);
    }

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
        Debug.Log($"[RoomListCache] Joined lobby => {PhotonNetwork.CurrentLobby.Name}");
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