using System.Collections.Generic;

[System.Serializable]
public class LeaderboardEntry
{
    public string username;
    public int wins;
    public int losses;
}

[System.Serializable]
public class LeaderboardResponse
{
    public bool status;
    public List<LeaderboardEntry> data;
}
