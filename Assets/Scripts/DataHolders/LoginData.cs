
using Newtonsoft.Json;
using System;

[Serializable]
public class LoginData
{
    public int ID;
    public int UserID;
    public DateTime LoginDate;
    public string DeviceModel = "";
    public string DeviceName = "";
    public string DeviceType = "";
    public string Logindata = "";

    public LoginData() { }

    public LoginData(int iD, int userID, DateTime loginDate, string deviceModel, string deviceName, string deviceType, string logindata)
    {
        ID = iD;
        UserID = userID;
        LoginDate = loginDate;
        DeviceModel = deviceModel;
        DeviceName = deviceName;
        DeviceType = deviceType;
        Logindata = logindata;
    }
    public LoginData(int iD, int userID, string loginDateStr, string deviceModel, string deviceName, string deviceType, string logindata)
    {
        ID = iD;
        UserID = userID;
        LoginDate = DateTime.Parse(loginDateStr);
        DeviceModel = deviceModel;
        DeviceName = deviceName;
        DeviceType = deviceType;
        Logindata = logindata;
    }

    public LoginData (string text, string readMode = "j")
    {
        if (readMode == "j")
        {
            LoginData data = JsonConvert.DeserializeObject<LoginData>(text);
            this.ID = data.ID;
            this.UserID = data.UserID;
            this.LoginDate = data.LoginDate;
            this.DeviceModel = data.DeviceModel;
            this.DeviceName = data.DeviceName;
            this.DeviceType = data.DeviceType;
            this.Logindata = data.Logindata;
        }
    }
}
