[System.Serializable]
public class GameStartRequest
{
    public int IdUser;
    public int WavesCount;
    public bool Status;
}

[System.Serializable]
public class GameStartAnswer
{
    public bool status;
    public GameData data;
}

[System.Serializable]
public class GameEndAnswer
{
    public bool status;
}

[System.Serializable]
public class GameData
{
    public int idGame;
}
