
public interface IShotgunThreatProvider
{
    float GetDanger(Board board, int x, int y);
}

public class NoShotgunThreatProvider : IShotgunThreatProvider
{
    public float GetDanger(Board board, int x, int y)
    {
        return 0f;
    }
}
