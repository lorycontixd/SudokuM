using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum MenuType
{
    MAIN,
    MATCHMAKING,
    LOBBY,
    SETTINGS
}


public interface AnimatedButton
{
    public bool IsMouseOver { get; set; }

    public void OnMouseEnter()
    {
        IsMouseOver = true;
    }
    public void OnMouseExit()
    {
        IsMouseOver = false;
    }
}


public abstract class BaseMenu : MonoBehaviourPunCallbacks
{
    public string Name;
    public MenuType Type;
    protected LobbyMenuController controller;

    public void SetController(LobbyMenuController controller)
    {
        this.controller = controller;
    }

    public virtual bool CanClose()
    {
        return true;
    }
    public virtual bool CanOpen()
    {
        return true;
    }
    public abstract void Open();
    public abstract void Close();
}
