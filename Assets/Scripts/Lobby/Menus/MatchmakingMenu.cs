using ExitGames.Client.Photon;
using Managers;
using Michsky.MUIP;
using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MatchmakingMenu : BaseMenu
{
    #region Search Mode
    public enum RoomSearchMode
    {
        NAME,
        CODE
    }
    #endregion


    [Header("UI")]
    public UserFrame userFrame;
    [SerializeField] private Button leaderboardButton;
    [SerializeField] private Button tempButton;
    [SerializeField] private Transform panelsParent;
    [SerializeField] private LeaderboardPanel leaderboardPanel;
    [SerializeField] private Transform createRoomPanelParent;
    [SerializeField] private CreateRoomPanel createRoomPanel;
    [SerializeField] private GameObject roomItemPrefab;
    [SerializeField] private Transform roomItemHolder;
    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private TextMeshProUGUI regionText;

    [Header("Settings")]
    [SerializeField] private float connectionTimeout = 30;
    [SerializeField] private RoomSearchMode searchMode;

    public override MenuType Type => MenuType.MATCHMAKING;

    private List<MatchmakingListItem> roomListItems = new List<MatchmakingListItem>();
    private float connectionTimeoutTimestamp = 0;
    private bool IsConnecting = false;
    private Hashtable pendingRoomCodeGenerated = new Hashtable();


    private void Start()
    {
        RoomListCache.Instance.onRoomListUpdate += UpdateRooms;

        DatabaseManager.Instance.onQuery.AddListener(OnDbQuery);
    }

    private void OnDbQuery(DatabaseQueryResultType resType, QueryData data)
    {
        if (data.QueryType == QueryType.GETUSER)
        {
            if (resType == DatabaseQueryResultType.SUCCESS)
            {
                if (data.extraInfo == "enter_room")
                {
                    GetUserQuery udata = (GetUserQuery)data;
                    User other = udata.user;
                    SessionManager.Instance.SetOtherUser(other);
                    controller.SwitchMenu(MenuType.LOBBY);
                }
            }
        }
    }

    private void Update()
    {
        if (IsConnecting)
        {
            connectionTimeoutTimestamp += Time.deltaTime;
            if (connectionTimeoutTimestamp > connectionTimeout)
            {
                Debug.LogWarning($"Connection timeout while conneting to server");
            }
        }
    }

    public override void Close()
    {
    }

    public override void Open()
    {
        if (createRoomPanelParent != null)
        {
            createRoomPanelParent.gameObject.SetActive(false);
        }
        if (panelsParent != null)
        {
            panelsParent.gameObject.SetActive(false);
        }
        createRoomPanel.Close();
        leaderboardPanel.Close();
        if (SessionManager.Instance.ActiveUser != null)
        {
            userFrame.UpdateUI();
        }
        UpdateRooms(RoomListCache.Instance.cachedRoomList);
    }

    #region Pun Callbacks
    public override void OnConnectedToMaster()
    {
    }
    public override void OnJoinedLobby()
    {
        Debug.Log($"Joined lobby ===> {PhotonNetwork.CurrentLobby.Name}, {PhotonNetwork.CurrentLobby.Type}");
        IsConnecting = false;
        connectionTimeoutTimestamp = 0;
        //SetConnectionTexts(true);
        regionText.text = $"Region: {PhotonNetwork.CloudRegion}";
        //AchievementsManager.Instance.GiveFirstTimeAchievement();
    }
    public override void OnCreatedRoom()
    {
        if (pendingRoomCodeGenerated != null)
        {
            Debug.Log($"Setting room properties! Code: {pendingRoomCodeGenerated["code"]}");
            PhotonNetwork.CurrentRoom.SetCustomProperties(pendingRoomCodeGenerated);
        }
        pendingRoomCodeGenerated = null;
        controller.SwitchMenu(MenuType.LOBBY);
    }
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1 && !PhotonNetwork.IsMasterClient)
        {
            int otherId = (int)PhotonNetwork.PlayerListOthers.First().CustomProperties["uid"];
            DatabaseManager.Instance.GetUserFull(otherId, "enter_room");
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        SessionManager.Instance.SetHostUser(null);
        SessionManager.Instance.SetOtherUser(null);
        SessionManager.Instance.SetLoginData(null);
        controller.SwitchMenu(MenuType.MAIN);
    }
    #endregion


    #region Buttons
    public void ButtonBack()
    {
        controller.SwitchMenu(MenuType.MAIN);
    }
    public void ButtonCreate()
    {
        createRoomPanel.Open(this);
    }

    public void ButtonDisconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }
    public void ButtonCodeSubmit()
    {
        if (searchMode == RoomSearchMode.CODE)
        {
            string code = codeInput.text;
            if (code != string.Empty)
            {
                List<RoomInfo> roomInfos = RoomListCache.Instance.cachedRoomList.Values.ToList();
                RoomInfo roomWithCode = roomInfos.FirstOrDefault(r => r.CustomProperties["code"].ToString().ToLower() == code.ToLower());
                if (roomWithCode != null)
                {
                    // Room exists
                    string roomName = roomWithCode.Name;
                    PhotonNetwork.JoinRoom(roomName);
                }
                else
                {
                    Debug.LogWarning($"The room with code {code} does not exist.");
                    Managers.NotificationManager.Instance.Warning("Error joining room", $"The room with code {code} does not exist.");
                }
            }
        }
        else
        {
            string name = codeInput.text;
            if ( name != string.Empty )
            {
                PhotonNetwork.JoinRoom(name);
            }
        }
    }

    public void ButtonLeaderboard()
    {
        panelsParent.gameObject.SetActive(true);
        leaderboardPanel.Open();
    }
    public void ButtonTemp()
    {
    }
    #endregion

    


    public void UpdateRooms(Dictionary<string, RoomInfo> rooms)
    {
        Debug.Log($"[MatchmakingMenu] UpdateRooms => count: {rooms.Count}");
        ClearRoomList();
        for(int i=0; i<rooms.Count; i++)
        {
            GameObject go = Instantiate(roomItemPrefab, roomItemHolder);
            MatchmakingListItem item = go.GetComponent<MatchmakingListItem>();
            item.SetRoomInfo(rooms.Values.ToList()[i]);
            item.onButtonClick += OnRoomButtonPress;
        }
    }
    private void ClearRoomList()
    {
        for (int i=0; i<roomItemHolder.childCount; i++)
        {
            Destroy(roomItemHolder.GetChild(i).gameObject);
        }
        roomListItems.Clear();
    }


    private void OnRoomButtonPress(MatchmakingListItem item)
    {
        if (!PhotonNetwork.InRoom && PhotonNetwork.IsConnectedAndReady)
        {
            if ( item != null)
            {
                PhotonNetwork.JoinRoom(item.RoomName);
            }
            else
            {
                Debug.LogError($"Matchmaking item is null??");
            }
        }
    }
    private void ShowAllRoomCodes()
    {
        for (int i=0; i<RoomListCache.Instance.RoomCacheCount; i++)
        {
            RoomInfo info = RoomListCache.Instance.cachedRoomList.Values.ToList()[i];
            Debug.Log($"Room ==> Name: {info.Name}, code: {info.CustomProperties["code"]}");
        }
    }

    public void SetPendingRoomCreation(Hashtable props)
    {
        this.pendingRoomCodeGenerated = props;
    }
}
