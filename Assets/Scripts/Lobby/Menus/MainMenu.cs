using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : BaseMenu
{
    public override MenuType Type => MenuType.MAIN;

    [SerializeField] private bool IsSettingsMenuActive = false;
   

    public override void Close()
    {
    }

    public override void Open()
    {
    }

    public void UpdateUI()
    {
    }

    public void ButtonSingleplayer()
    {
        Managers.NotificationManager.Instance.Warning("Mode unavailable", "This game mode is currently unavailable. Try again later!");
        return;
    }

    public void ButtonMultiplayer ()
    {
        if (SessionManager.Instance.ActiveUser != null && PhotonNetwork.IsConnectedAndReady)
        {
            controller.SwitchMenu(MenuType.MATCHMAKING);
        }
        else
        {
            controller.SwitchMenu(MenuType.LOGIN);
        }

    }

    public void ButtonSettings()
    {
        if (!IsSettingsMenuActive)
        {
            Managers.NotificationManager.Instance.Info("Feature incoming", "This feature is on its way. Please be patient!");
            return;
        }
        controller.SwitchMenu(MenuType.SETTINGS);
    }

    #region Pun callbacks

    #endregion
}
