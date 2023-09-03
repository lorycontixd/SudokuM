using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public abstract class BaseSettings
{
    public abstract bool Save();
    public abstract bool Load();
}
