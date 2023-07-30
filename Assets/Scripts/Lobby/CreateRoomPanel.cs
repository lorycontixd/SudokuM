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
    [Header("Panels")]
    [SerializeField] private Transform parent;
    [SerializeField] private Transform uiHolder;

    [Header("UI")]
    [SerializeField] private ButtonManager createButton;
    [SerializeField] private ButtonManager cancelButton;
    [SerializeField] private Transform privateSwitchParent;
    [SerializeField] private SwitchManager privateSwitch;
    [SerializeField] private TMP_InputField roomName;
    [SerializeField] private CustomDropdown gamemodeDropdown;

    private MatchmakingMenu matchmakingMenu;

    private void Start()
    {
        
    }

    public void Open(MatchmakingMenu menu)
    {
        this.matchmakingMenu = menu;
        parent.gameObject.SetActive(true);
    }
    public void Close()
    {

        parent.gameObject.SetActive(false);
    }
    #region UI Events
    public void CancelCreateRoom()
    {
        Close();
    }
    public void CreateRoomButton()
    {
        if (ValidateInput() && PhotonNetwork.IsConnectedAndReady)
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
    }
    public void OnDropdownDeselect()
    {

    }
    public void OnDropdownValueChanged(Int32 val)
    {
        privateSwitchParent.gameObject.SetActive(true);
    }
    #endregion

    private bool ValidateInput()
    {
        string roomname = roomName.text;
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
        bool isPrivate = privateSwitch.isOn;
        string roomname = roomName.text;
        GameMode gamemode = (GameMode)gamemodeDropdown.selectedItemIndex;

        RoomOptions opts = new RoomOptions();
        opts.MaxPlayers = 2;
        opts.IsOpen = true;
        opts.IsVisible = true;
        string code = RoomCodeGenerator.GenerateCode();
        Hashtable ht = new Hashtable
        {
            { "code", code }, // code 
            { "p", isPrivate },
            { "ig", false },
            { "mode",  gamemodeDropdown.selectedItemIndex }
        };
        if (matchmakingMenu != null)
        {
            matchmakingMenu.SetPendingRoomCreation(ht);
        }
        opts.CustomRoomPropertiesForLobby = new string[] { "code", "p", "ig", "mode" };
        opts.CustomRoomProperties = ht;
        PhotonNetwork.CreateRoom(roomname, opts, TypedLobby.Default);
    }
}
