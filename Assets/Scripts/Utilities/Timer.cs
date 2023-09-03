using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public string Name { get; private set; } = "New Timer";
    public float Duration { get; private set; }
    public bool IsActive { get; private set; }
    public bool RemoveOnCompletion { get; private set; }

    public float RemainingTime { get => timestamp; }
    public float ElapsedTime { get => Duration - timestamp; }

    public Action<Timer> onTimerEnd;
    
    private float timestamp;

    public Timer()
    {
        Name = "New Timer";
        Duration = 0f;
        IsActive = false;
        timestamp = 0f;
    }

    public Timer(string name, float duration, bool start = false, bool remove = false)
    {
        Name = name;
        Duration = duration;
        IsActive = start;
        timestamp = Duration;
        RemoveOnCompletion = remove;
    }

    public void Tick(float time, float multiplier = 1f)
    {
        timestamp -= time * multiplier;
        if ( timestamp <= 0f && IsActive)
        {
            onTimerEnd?.Invoke(this);
            IsActive = false;
        }
    }

    public void Play()
    {
        if (!IsActive)
        {
            IsActive = true;
        }
    }
    public void Pause()
    {
        if (IsActive)
        {
            IsActive = false;
        }
    }
    public void Stop()
    {
        if (IsActive)
        {
            IsActive = false;
            Reset();
        }
    }
    public void Reset()
    {
        timestamp = Duration;
    }
}
