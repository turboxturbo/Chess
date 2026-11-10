using NUnit.Framework;

public class ChessTests
{
    // 1. Фигура размещается на доске
    [Test]
    public void Board_PlacePiece_PieceAppearsOnCell()
    {
        Board board = new Board();
        Pawn pawn = new Pawn(PieceColor.White, 3);

        board.PlacePiece(pawn, 3, 3);

        Assert.AreEqual(pawn, board.ChessBoard[3, 3].CurrentPiece);
    }

    // 2. Фигура перемещается на новую клетку
    [Test]
    public void Board_TryMovePiece_PieceMovesToNewCell()
    {
        Board board = new Board();
        Pawn pawn = new Pawn(PieceColor.White, 3);
        board.PlacePiece(pawn, 3, 1);

        bool moved = board.TryMovePiece(3, 1, 3, 2, out _);

        Assert.IsTrue(moved);
        Assert.AreEqual(pawn, board.ChessBoard[3, 2].CurrentPiece);
        Assert.IsNull(board.ChessBoard[3, 1].CurrentPiece);
    }

    // 3. ClearWhitePieces удаляет только белые фигуры
    [Test]
    public void Board_ClearWhitePieces_BlackKingRemains()
    {
        Board board = new Board();
        board.PlacePiece(new Pawn(PieceColor.White, 3), 0, 0);
        board.PlacePiece(new BlackKing(1, 3), 4, 7);

        board.ClearWhitePieces();

        Assert.IsNull(board.ChessBoard[0, 0].CurrentPiece);
        Assert.IsNotNull(board.ChessBoard[4, 7].CurrentPiece);
    }

    // 4. TakeDamage уменьшает HP фигуры
    [Test]
    public void ChessPiece_TakeDamage_ReducesHealth()
    {
        Pawn pawn = new Pawn(PieceColor.White, 3);

        pawn.TakeDamage(2);

        Assert.AreEqual(1, pawn.Health);
    }

    // 5. HP не уходит ниже нуля при избыточном уроне
    [Test]
    public void ChessPiece_TakeDamage_ClampsAtZero()
    {
        Pawn pawn = new Pawn(PieceColor.White, 3);

        pawn.TakeDamage(10);

        Assert.AreEqual(0, pawn.Health);
    }

    // 6. Выстрел из пустого патронника невозможен
    [Test]
    public void ShotgunAmmo_TryFire_ReturnsFalseWhenChamberEmpty()
    {
        ShotgunAmmo ammo = new ShotgunAmmo();
        ammo.TryFire(); // тратим единственный патрон

        bool result = ammo.TryFire();

        Assert.IsFalse(result);
    }

    // 7. Дробовик наносит урон фигуре прямо по курсу
    [Test]
    public void ShotgunAttack_Fire_HitsPieceDirectlyAhead()
    {
        Board board = new Board();
        board.PlacePiece(new BlackKing(1, 3), 0, 0);
        Pawn target = new Pawn(PieceColor.White, 3);
        board.PlacePiece(target, 0, 2);

        ShotgunAttack shotgun = new ShotgunAttack(
            pelletCount: 8,
            pelletDamage: 1,
            spreadDegrees: 0f,
            range: 5f,
            seed: 42);

        var damage = shotgun.Fire(board, 0, 0, 0f, 1f);

        Assert.IsTrue(damage.ContainsKey(target));
        Assert.Greater(damage[target], 0);
    }
}
