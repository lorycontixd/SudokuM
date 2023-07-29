using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : BaseMenu
{
    [Header("UI")]
    [SerializeField] private InputField nameInput;
    [SerializeField] private Toggle rememberNameToggle;
    
    public override void Close()
    {
    }

    public override void Open()
    {
        if (PlayerPrefs.HasKey("username"))
        {
            nameInput.SetTextWithoutNotify(PlayerPrefs.GetString("username"));
            rememberNameToggle.isOn = true;
        }
    }


    public void ButtonPlay()
    {
        if (nameInput != null)
        {
            if (ValidateName())
            {
                if (controller != null)
                {
                    if (rememberNameToggle.isOn)
                    {
                        PlayerPrefs.SetString("username", nameInput.text);
                    }
                    else
                    {
                        if (PlayerPrefs.HasKey("username"))
                        {
                            PlayerPrefs.DeleteKey("username");
                        }
                    }
                    PhotonNetwork.NickName = nameInput.text;
                    controller.SwitchMenu(MenuType.MATCHMAKING);
                }
            }
        }
        
    }

    private bool ValidateName()
    {
        bool lengthValid = nameInput.text.Length > 5 && nameInput.text.Length < 20;
        if (!lengthValid)
        {
            Managers.NotificationManager.Instance.Warning("Invalid username", $"Length of username must be between 5 and 20 characters, not {nameInput.text.Length} ");
            return false;
        }
        return true;
    }

    public void ButtonQuit() {
        
    }

    #region Pun callbacks
    

    #endregion
}
