using System;

public sealed class ShotgunAmmo
{
    public const int ReserveCapacity = 5;
    public int Reserve { get; private set; }
    public bool IsLoaded { get; private set; }

    public ShotgunAmmo(int reserve = ReserveCapacity, bool isLoaded = true)
    {
        if (reserve < 0 || reserve > ReserveCapacity)
            throw new ArgumentOutOfRangeException(nameof(reserve));
        Reserve = reserve;
        IsLoaded = isLoaded;
    }

    public bool TryFire()
    {
        if (!IsLoaded) return false;
        IsLoaded = false;
        return true;
    }

    public bool TryReload()
    {
        if (IsLoaded || Reserve == 0) return false;
        Reserve--;
        IsLoaded = true;
        return true;
    }

    public void OnKingMoved()
    {
        // Replenish the reserve first, then transfer a shell to an empty chamber.
        Reserve = Math.Min(ReserveCapacity, Reserve + 1);
        TryReload();
    }
}
