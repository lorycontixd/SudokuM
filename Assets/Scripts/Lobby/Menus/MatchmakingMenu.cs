using ExitGames.Client.Photon;
using Michsky.MUIP;
using Photon.Pun;
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
    [SerializeField] private TextMeshProUGUI connectingText;
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private GameObject roomItemPrefab;
    [SerializeField] private Transform roomItemHolder;
    [SerializeField] private SwitchManager privateSwitch;
    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private List<GameObject> deactivatableUI = new List<GameObject>();
    [SerializeField] private TextMeshProUGUI regionText;

    [Header("Settings")]
    [SerializeField] private float connectionTimeout = 30;
    [SerializeField] private RoomSearchMode searchMode;


    private RoomListCache roomListCache;
    private List<MatchmakingListItem> roomListItems = new List<MatchmakingListItem>();
    private float connectionTimeoutTimestamp = 0;
    private bool IsConnecting = false;
    private Hashtable pendingRoomCodeGenerated = new Hashtable();


    private void Start()
    {
        roomListCache = GetComponent<RoomListCache>();
        roomListCache.onRoomListUpdate += UpdateRooms;
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
        if (!PhotonNetwork.IsConnected)
        {
            SetConnectionTexts(false);
            IsConnecting = true;
            ConnectToEU();
        }
        else
        {
            SetConnectionTexts(true);
            SetUsernameText(PhotonNetwork.LocalPlayer.NickName);
        }
    }

    #region Pun Callbacks

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        IsConnecting = false;
        connectionTimeoutTimestamp = 0;
        SetConnectionTexts(true);
        SetUsernameText(PhotonNetwork.LocalPlayer.NickName);
        regionText.text = $"Region: {PhotonNetwork.CloudRegion}";
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
        controller.SwitchMenu(MenuType.LOBBY);
    }
    #endregion


    #region Buttons
    public void ButtonBack()
    {
        controller.SwitchMenu(MenuType.MAIN);
    }
    public void ButtonCreate()
    {
        bool isPrivate = privateSwitch.isOn;
        if ( PhotonNetwork.IsConnectedAndReady)
        {
            CreateRoom(isPrivate);
        }
    }
    public void ButtonCodeSubmit()
    {
        if (searchMode == RoomSearchMode.CODE)
        {
            string code = codeInput.text;
            if (code != string.Empty)
            {
                List<RoomInfo> roomInfos = roomListCache.cachedRoomList.Values.ToList();
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
    #endregion

    void ConnectToEU()
    {
        AppSettings euSettings = new AppSettings();
        euSettings.UseNameServer = true;
        euSettings.FixedRegion = "eu";
        euSettings.AppIdRealtime = "8420cc2c-c728-4c55-aae1-7ef584128ec3"; // TODO: replace with your own PUN AppId unlocked for China region
        PhotonNetwork.ConnectUsingSettings(euSettings);
    }

    public void CreateRoom(bool isPrivate)
    {
        int index = roomListCache.RoomCacheCount;
        string roomName = $"Room{index}";
        RoomOptions opts = new RoomOptions();
        opts.MaxPlayers = 2;
        opts.IsOpen = true;
        opts.IsVisible = true;
        Hashtable ht = new Hashtable
        {
            { "code", RoomCodeGenerator.GenerateCode() },
            { "p", isPrivate },
            { "ig", false }
        };
        pendingRoomCodeGenerated = ht;
        opts.CustomRoomPropertiesForLobby = new string[] { "code", "p", "ig" };
        opts.CustomRoomProperties = ht;
        PhotonNetwork.CreateRoom(roomName, opts, TypedLobby.Default );
    }


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
        for (int i=0; i<roomListCache.RoomCacheCount; i++)
        {
            RoomInfo info = roomListCache.cachedRoomList.Values.ToList()[i];
            Debug.Log($"Room ==> Name: {info.Name}, code: {info.CustomProperties["code"]}");
        }
    }

    private void SetConnectionTexts(bool isConnected)
    {
        connectingText.gameObject.SetActive(!isConnected);
        foreach(var obj in deactivatableUI)
        {
            if (obj != null)
            {
                obj.SetActive(isConnected);
            }
        }
    }

    private void SetUsernameText(string username)
    {
        if (usernameText != null)
        {
            usernameText.text = $"Playing as: {username}";
        }
    }
}
