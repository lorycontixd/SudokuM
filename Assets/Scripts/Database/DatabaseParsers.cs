using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DatabaseParsers
{
    public static int ExpectedUserLength = 8;


    /*public static User ParseUser(string text)
    {
        string[] values = text.Split('\t');
        if (values.Length != ExpectedUserLength)
        {
            Debug.LogWarning($"Error: user values must be {ExpectedUserLength}, not {values.Length}");
        }
        int id = int.Parse(values[0]);
        int roleid = int.Parse(values[1]);
        string username = values[2];
        string googleid = values[3];
        string firstname = values[4];
        string lastname = values[5];
        string email = values[6];
        int timeraceScore = int.Parse(values[7]);
        User user = new User(id, roleid, username, googleid, firstname, lastname, email, timeraceScore);
        return user;
    }*/

    /*public static AddTimeraceScoreQuery ParseAddTimerace(string text)
    {
        
        return new AddTimeraceScoreQuery(text);
    }*/

 /*   public static AddLoginInstanceQuery ParseAddLogin(string text, string extrainfo = "")
    {
        string[] values = text.Split('\t');
        int id = int.Parse(values[0]);
        int userid = int.Parse(values[1]);
        DateTime logindata = DateTime.Parse(values[2]);
        string deviceModel = values[4];
        string deviceName = values[5];
        string deviceType = values[6];
        return new AddLoginInstanceQuery(id,userid,logindata,deviceModel,deviceName,deviceType, extrainfo);
    }*/
}
