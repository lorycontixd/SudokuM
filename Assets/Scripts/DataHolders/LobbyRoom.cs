using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class LobbyRoom
{
    public User hostUser;
    public User otherUser;
    public GameMode gameMode;
    public DateTime creationDate;
}
