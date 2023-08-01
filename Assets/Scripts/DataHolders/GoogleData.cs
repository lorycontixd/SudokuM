using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class GoogleData
{
    public struct Achievement
    {
        public int Id;
        public string PlayId;
        public string Name;
        public bool Incremental;
    }
    public struct Leaderboard
    {
        public int Id;
        public string PlayId;
        public string Name;
    }

    public string ProjectID;
    public List<Achievement> Achievements;
    public List<Leaderboard> Leaderboards;


}
