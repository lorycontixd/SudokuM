using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FloatSetting : Setting<float>
{
    public override void OnValueChange(float value)
    {
    }

    public override void Restore()
    {
        throw new System.NotImplementedException();
    }
}
