
using System;

[Serializable]
public class GameInstance
{
    public int ID;
    public int User1;
    public int User2;
    public int GameModeID;
    public GameMode GameMode;
    public string GameCode = "";
    public bool IsRankedGame;
    public bool PhotonRoomIsPrivate;
    public string PhotonRoomPrivateCode;
    public int PhotonServerTimestamp;
    public string PhotonServerAddress;
    public string PhotonGameVersionHost;
    public string PhotonAppVersionHost;
    public string PhotonCloudRegionHost;
    public DateTime StartTime;

    public GameInstance(int iD, int user1, int user2, int gameModeID, string gameCode, bool isRankedGame, bool photonRoomIsPrivate, string photonRoomPrivateCode, int photonServerTimestamp, string photonServerAddress, string photonGameVersionHost, string photonAppVersionHost, string photonCloudRegionHost, DateTime startTime)
    {
        ID = iD;
        User1 = user1;
        User2 = user2;
        GameModeID = gameModeID;
        GameMode = (GameMode)GameModeID;
        GameCode = gameCode;
        IsRankedGame = isRankedGame;
        PhotonRoomIsPrivate = photonRoomIsPrivate;
        PhotonRoomPrivateCode = photonRoomPrivateCode;
        PhotonServerTimestamp = photonServerTimestamp;
        PhotonServerAddress = photonServerAddress;
        PhotonGameVersionHost = photonGameVersionHost;
        PhotonAppVersionHost = photonAppVersionHost;
        PhotonCloudRegionHost = photonCloudRegionHost;
        StartTime = startTime;
    }
    public GameInstance(int iD, int user1, int user2, int gameModeID, string gameCode, bool isRankedGame, bool photonRoomIsPrivate, string photonRoomPrivateCode, int photonServerTimestamp, string photonServerAddress, string photonGameVersionHost, string photonAppVersionHost, string photonCloudRegionHost, string startTime)
    {
        ID = iD;
        User1 = user1;
        User2 = user2;
        GameModeID = gameModeID;
        GameMode = (GameMode)gameModeID;
        GameCode = gameCode;
        IsRankedGame = isRankedGame;
        PhotonRoomIsPrivate = photonRoomIsPrivate;
        PhotonRoomPrivateCode = photonRoomPrivateCode;
        PhotonServerTimestamp = photonServerTimestamp;
        PhotonServerAddress = photonServerAddress;
        PhotonGameVersionHost = photonGameVersionHost;
        PhotonAppVersionHost = photonAppVersionHost;
        PhotonCloudRegionHost = photonCloudRegionHost;
        StartTime = DateTime.Parse(startTime) ;
    }


    public GameInstance(string text)
    {
        string[] values = text.Split('\t');
        this.ID = int.Parse(values[0]);
        this.User1 = int.Parse(values[1]);
        this.User2 = int.Parse(values[2]);
        this.GameModeID = int.Parse(values[3]);
        this.GameMode = (GameMode)this.GameModeID;
        this.GameCode = values[4];
        this.IsRankedGame = Convert.ToBoolean(int.Parse(values[5]));
        this.PhotonRoomIsPrivate = Convert.ToBoolean(int.Parse(values[6]));
        this.PhotonRoomPrivateCode = values[6];
        this.PhotonServerTimestamp = int.Parse(values[8]);
        this.PhotonServerAddress = values[9];
        this.PhotonGameVersionHost = values[10];
        this.PhotonAppVersionHost = values[11];
        this.PhotonCloudRegionHost = values[12];
        this.StartTime = DateTime.Parse(values[13]);
    }
}
