using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TimerManager : MonoBehaviour
{
    #region Singleton
    private static TimerManager _instance;
    public static TimerManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    public bool IsActive = true;
    public List<Timer> timers = new List<Timer>();

    public UnityEvent<Timer> onTimerEnd;


    private void Update()
    {
        if (IsActive)
        {
            TickAll();
        }
    }

    public bool AddTimer(Timer timer)
    {
        if (TimerExists(timer.Name))
        {
            return false;
        }
        timers.Add(timer);
        timer.onTimerEnd += OnTimerEnd;
        return true;
    }
    public bool TimerExists(string name)
    {
        return timers.Any(t => t.Name == name);
    }
    public Timer GetTimer(string name)
    {
        return timers.FirstOrDefault(t => t.Name == name);
    }

    private void OnTimerEnd(Timer timer)
    {
        onTimerEnd?.Invoke(timer);
        if (timer.RemoveOnCompletion)
        {
            if (timers.Contains(timer))
            {
                timers.Remove(timer);
            }
        }
    }

    public void TickAll()
    {
        foreach (Timer timer in timers)
        {
            timer.Tick(Time.deltaTime);
        }
    }
}
