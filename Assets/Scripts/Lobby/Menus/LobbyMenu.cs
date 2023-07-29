using ExitGames.Client.Photon;
using Michsky.MUIP;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class LobbyMenu : BaseMenu
{
    #region HostMigrationMode
    public enum HostMigrationMode
    {
        FIRST,
        RANDOM
    }
    #endregion

    [SerializeField] private GameObject playerListItemPrefab = null;

    [Header("UI")]
    [SerializeField] private Transform playerListHolder;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI roomCodeText;
    [SerializeField] private TextMeshProUGUI waitingForHostText;
    [SerializeField] private TextMeshProUGUI boardInitializingText;
    [SerializeField] private ButtonManager startButton;
    [SerializeField] private ButtonManager settingsButton;
    [SerializeField] private ButtonManager quitButton;

    [Header("Settings")]
    [SerializeField] private HostMigrationMode hostMigrationMode = HostMigrationMode.FIRST;
    [SerializeField] private bool AllowSinglePlay;


    public override void Close()
    {
    }

    public override void Open()
    {
        SetUI();
        UpdatePlayerList();
    }


    private void SetUI()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogError($"Player must be in a PUN room to set UI");
            return;
        }
        roomNameText.text = $"Room name: {PhotonNetwork.CurrentRoom.Name}";
        roomCodeText.text = $"Room code: {PhotonNetwork.CurrentRoom.CustomProperties["code"]}";
        if (waitingForHostText != null)
        {
            waitingForHostText.gameObject.SetActive(!PhotonNetwork.IsMasterClient);
        }
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        settingsButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        startButton.Interactable(true);
        settingsButton.Interactable(true);
        quitButton.Interactable(true);
        boardInitializingText.gameObject.SetActive(false);
    }

    private void ClearPlayerList()
    {
        for (int i=0; i<playerListHolder.childCount; ++i)
        {
            Destroy(playerListHolder.GetChild(i).gameObject);
        }
    }
    private void UpdatePlayerList()
    {
        ClearPlayerList();
        if (playerListItemPrefab == null)
        {
            Debug.LogWarning($"Trying to update Lobby player list but no prefab was passed.");
            return;
        }
        for( int i=0; i<PhotonNetwork.CurrentRoom.Players.Count; i++)
        {
            GameObject go = Instantiate(playerListItemPrefab, playerListHolder);
            LobbyPlayerListItem item = go.GetComponent<LobbyPlayerListItem>();
            item.SetPlayer(PhotonNetwork.CurrentRoom.Players.Values.ToList()[i]);
        }
    }

    public void StartGame()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            bool canLoad = AllowSinglePlay ? (PhotonNetwork.CurrentRoom.PlayerCount == 1 || PhotonNetwork.CurrentRoom.PlayerCount == 2) : PhotonNetwork.CurrentRoom.PlayerCount == 2;
            if (canLoad)
            {
                StartCoroutine(StartGameCo());
            }
            else
            {
                Managers.NotificationManager.Instance.Warning("Cannot start game", "Wait for a second player to join before starting the game");
            }
        }
    }
    private IEnumerator StartGameCo(float delay = 1f)
    {
        boardInitializingText.gameObject.SetActive(true);
        yield return new WaitForSeconds(delay);
        ExitGames.Client.Photon.Hashtable ht = PhotonNetwork.CurrentRoom.CustomProperties;
        ht["ig"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(ht);
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel("Game");
    }

    private Player PickNewMasterclient()
    {
        List<Player> players = PhotonNetwork.CurrentRoom.Players.Values.ToList();
        Player newHost = null;
        if (hostMigrationMode == HostMigrationMode.FIRST)
        {
            if (players.Count > 0)
            {
                newHost = players[0];
            }
        }
        else if (hostMigrationMode == HostMigrationMode.RANDOM)
        {
            Random random = new Random(50);
            int index = random.Next(players.Count);
            newHost = players[index];
        }
        return newHost;
    }


    #region Pun Callbacks
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
        SetUI();
    }
    #endregion

    #region Buttons
    public void ButtonStart()
    {
        startButton.Interactable(false);
        settingsButton.Interactable(false);
        quitButton.Interactable(false);
        if (PhotonNetwork.IsMasterClient)
        {
            LobbyPunEventSender.SendGameStart();
        }
    }
    public void ButtonSettings()
    {

    }
    public void ButtonQuit()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Player newHost = PickNewMasterclient();
            PhotonNetwork.SetMasterClient(newHost);
            LobbyPunEventSender.SendHostMigration(newHost);
        }
        PhotonNetwork.LeaveRoom();
        controller.SwitchMenu(MenuType.MATCHMAKING);
    }
    #endregion
}
