using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Michsky.MUIP;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CreateRoomPanel : MonoBehaviour
{
    public string timeoutTimerName = "CreateRoomTimeout";

    [Header("Panels")]
    [SerializeField] private Transform parent;
    [SerializeField] private Transform uiHolder;

    [Header("UI")]
    [SerializeField] private ButtonManager createButton;
    [SerializeField] private ButtonManager cancelButton;
    [SerializeField] private Transform privateSwitchParent;
    [SerializeField] private SwitchManager privateSwitch;
    [SerializeField] private Transform rankedSwitchParent;
    [SerializeField] private SwitchManager rankedSwitch;
    [SerializeField] private TMP_InputField roomNameInput;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private CustomDropdown gamemodeDropdown;

    private MatchmakingMenu matchmakingMenu;

    private void Start()
    {
        TimerManager.Instance.onTimerEnd.AddListener(OnTimerEnd);
    }

    private void OnTimerEnd(Timer timer)
    {
        if (timer.Name == timeoutTimerName)
        {
            Debug.LogWarning($"Create room timed out");
            ToggleUI(true);
        }
    }

    public void Open(MatchmakingMenu menu)
    {
        Debug.Log($"OPENING ROOM PANEL");
        this.matchmakingMenu = menu;
        parent.gameObject.SetActive(true);
        ToggleUI(true);
        gameObject.SetActive(true);
        UpdateUI();
    }
    public void Close()
    {
        parent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public void UpdateUI()
    {
        roomNameText.text = $"{PhotonNetwork.LocalPlayer.NickName}'s Room";
    }

    public void ResetUI()
    {
        gamemodeDropdown.selectedItemIndex = 0;
        rankedSwitch.isOn = false;
        privateSwitch.isOn = false;
    }


    #region UI Events
    public void CancelCreateRoom()
    {
        Close();
    }
    public void CreateRoomButton()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            CreateRoom();
        }
        else
        {
            Close();
        }
    }
    public void OnDropdownSelect()
    {
        privateSwitchParent.gameObject.SetActive(false);
        rankedSwitchParent.gameObject.SetActive(false);
    }
    public void OnDropdownDeselect()
    {

    }
    public void OnDropdownValueChanged(Int32 val)
    {
        privateSwitchParent.gameObject.SetActive(true);
        rankedSwitchParent.gameObject.SetActive(true);
    }
    #endregion

    private bool ValidateInput()
    {
        string roomname = roomNameInput.text;
        if (roomname == string.Empty)
        {
            Managers.NotificationManager.Instance.Warning("Invalid room name", "Please insert a valid room name");
            return false;
        }
        if (roomname.Length < 3)
        {
            Managers.NotificationManager.Instance.Warning("Invalid room name", "Room name must have at least 3 characters");
            return false;
        }
        return true;
    }
    
    public void CreateRoom()
    {
        ToggleUI(false);

        bool isPrivate = privateSwitch.isOn;
        bool isRanked = rankedSwitch.isOn;
        //string roomname = roomNameInput.text;
        string roomname = $"{PhotonNetwork.LocalPlayer.NickName}'s Room";
        GameMode gamemode = (GameMode)gamemodeDropdown.selectedItemIndex;

        RoomOptions opts = new RoomOptions();
        opts.MaxPlayers = 2;
        opts.IsOpen = true;
        opts.IsVisible = true;
        opts.PlayerTtl = 20000;
        //opts.EmptyRoomTtl = NetworkManager.Instance.LightMaxReconnectSeconds * 1000;
        string code = RoomCodeGenerator.GenerateCode();
        Hashtable ht = new Hashtable
        {
            { "code", code }, // code 
            { "p", isPrivate }, // is private
            { "r", isRanked }, // is ranked
            { "ig", false }, // ?
            { "mode",  gamemodeDropdown.selectedItemIndex } // game mode
        };
        if (matchmakingMenu != null)
        {
            matchmakingMenu.SetPendingRoomCreation(ht);
        }
        opts.CustomRoomPropertiesForLobby = new string[] { "code", "p", "r", "ig", "mode" };
        opts.CustomRoomProperties = ht;
        PhotonNetwork.CreateRoom(roomname, opts, TypedLobby.Default);
    }

    private void ToggleUI(bool active)
    {
        createButton.Interactable(active);
        cancelButton.Interactable(active);
    }
}
