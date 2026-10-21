# Контекст проекта Chess

Unity-проект с шахматной доской 8×8.

## Текущая структура

- `Assets/Scripts/GameManager.cs` — создаёт и заполняет игровую доску.
- `Assets/Scripts/Cell.cs` — содержит классы `Cell`, `ChessPiece`, `CellPosition`, а также перечисления `PieceType` и `PieceColor`.
- `Assets/Scripts/BoardManager.cs` — переводит координаты доски в координаты Tilemap/мира.

## Текущая цель

Расставлять фигуры ближе к центру:

- сильные фигуры на первом ряду: `(3, 0)` и `(4, 0)`;
- пешки на втором ряду: `(2, 1)`–`(5, 1)`.

Перед расстановкой необходимо создать все 64 объекта `Cell` в массиве `Cell[,] board`.

## Важно

- `UnityEngine.Random` — статический класс: используйте `Random.Range(...)` или `Random.value`, а не `new Random()`.
- Чтобы создавать фигуру с заданным типом и цветом, `ChessPiece` нужен конструктор `ChessPiece(PieceType type, PieceColor color)`.
