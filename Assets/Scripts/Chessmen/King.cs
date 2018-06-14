using System.Collections.Generic;
using System.Linq;

public class King : Chessman
{
	bool hasMoved = false;

	List<Chessman> Enemies
	{
		get { return isWhite ? FindObjectOfType<Board>().BlackChessmen : FindObjectOfType<Board>().WhiteChessmen; }
	}

	public override void OnMove(int z, int x)
	{
		if (Y - FindObjectOfType<Board>().GetBoardPos(z) == 0)
		{
			int xDelta = FindObjectOfType<Board>().GetBoardPos(x) - X;
			int row = isWhite ? 0 : 7;

			//TODO: Fix this.
			//if (xDelta == -3)
			//	FindObjectOfType<Board>().MakeMove(FindObjectOfType<Board>().GetChessmanByBoardIndex(row, 0), FindObjectOfType<Board>().GetActualPos(row), FindObjectOfType<Board>().GetActualPos(2));

			//else if (xDelta == 2)
			//	FindObjectOfType<Board>().MakeMove(FindObjectOfType<Board>().GetChessmanByBoardIndex(row, 7), FindObjectOfType<Board>().GetActualPos(row), FindObjectOfType<Board>().GetActualPos(5));
		}

		hasMoved = true;
	}

	protected override Move[] GetValidMoves(bool checkSafety, Cell[,] board)
	{
		List<Move> validMoves = new List<Move>();

		for (int i = -1; i <= 1; i++)
		{
			CheckMove(Y + 1, X + i, validMoves, checkSafety, board);
			CheckMove(Y - 1, X + i, validMoves, checkSafety, board);
		}

		CheckMove(Y, X - 1, validMoves, checkSafety, board);
		CheckMove(Y, X + 1, validMoves, checkSafety, board);

		if (checkSafety)
			CheckCastle(validMoves);

		return validMoves.ToArray();
	}

	void CheckMove(int endY, int endX, List<Move> validMoves, bool checkSafety, Cell[,] board)
	{
		bool isOutOfBounds = endY < 0 || endY > 7 || endX < 0 || endX > 7;

		if (isOutOfBounds || board[endY, endX] == Friend || (checkSafety && WillBeKilledAfterMove(Y, X, endY, endX, this)))
			return;

		if (board[endY, endX] == Cell.Empty)
			validMoves.Add(new Move(endY, endX, isKill: false, isCastle: false));

		else
			validMoves.Add(new Move(endY, endX, isKill: true, isCastle: false));
	}

	void CheckCastle(List<Move> validMoves)
	{
		if (hasMoved || IsUnderThreat())
			return;

		int row = isWhite ? 0 : 7;
		Rook rook = FindObjectOfType<Board>().GetChessmanByBoardIndex(row, 7) as Rook;

		if (rook != null 
			&& (isWhite ? rook.isWhite : !rook.isWhite) 
			&& FindObjectOfType<Board>().Cells[row, 5] == Cell.Empty
			&& FindObjectOfType<Board>().Cells[row, 6] == Cell.Empty
			
			&& !FindObjectOfType<Board>().CellIsInDanger(row, 5, !isWhite)
			&& !FindObjectOfType<Board>().CellIsInDanger(row, 6, !isWhite))
		{
			validMoves.Add(new Move(row, 6, isKill: false, isCastle: true));
		}

		rook = FindObjectOfType<Board>().GetChessmanByBoardIndex(row, 0) as Rook;

		if (rook != null 
			&& (isWhite ? rook.isWhite : !rook.isWhite) 
			&& FindObjectOfType<Board>().Cells[row, 1] == Cell.Empty
			&& FindObjectOfType<Board>().Cells[row, 2] == Cell.Empty
			&& FindObjectOfType<Board>().Cells[row, 3] == Cell.Empty
			
			&& !FindObjectOfType<Board>().CellIsInDanger(row, 1, !isWhite)
			&& !FindObjectOfType<Board>().CellIsInDanger(row, 2, !isWhite)
			&& !FindObjectOfType<Board>().CellIsInDanger(row, 3, !isWhite))
		{
			validMoves.Add(new Move(row, 1, isKill: false, isCastle: true));
		}
	}

	public bool IsUnderThreat()
	{
		return ToBeKilled(FindObjectOfType<Board>().Cells, Enemies);
	}

	public bool WillBeKilledAfterMove(int startY, int startX, int endY, int endX, King kingComponent)
	{
		Cell[,] boardAfterMove = (Cell[,])FindObjectOfType<Board>().Cells.Clone();
		List<Chessman> enemies = new List<Chessman>(Enemies);

		if ((boardAfterMove[endY, endX] & Enemy) == Enemy)
			enemies.Remove(FindObjectOfType<Board>().GetChessmanByBoardIndex(endY, endX));

		boardAfterMove[startY, startX] = Cell.Empty;
		boardAfterMove[endY, endX] = isWhite ? Cell.WhiteFigure : Cell.BlackFigure;

		if (kingComponent != null)
			boardAfterMove[endY, endX] |= Cell.King;

		return ToBeKilled(boardAfterMove, enemies);
	}

	bool ToBeKilled(Cell[,] cells, List<Chessman> enemies)
	{
		return enemies.Any(enemy => enemy.ThreatForEnemyKing(cells));
	}
}
