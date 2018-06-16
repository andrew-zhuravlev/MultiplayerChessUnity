using System.Collections.Generic;
using System.Linq;

public class King : Chessman
{
	bool hasMoved = false;

	List<Chessman> Enemies
	{
		get { return isWhite ? Board.Instance.BlackChessmen : Board.Instance.WhiteChessmen; }
	}

	public override void OnMove(int z, int x)
	{
		if (Y - Board.Instance.GetBoardPos(z) == 0)
		{
			int xDelta = Board.Instance.GetBoardPos(x) - X;
			int row = isWhite ? 0 : 7;

			//TODO: Fix this.
			//if (xDelta == -3)
			//	Board.Instance.MakeMove(Board.Instance.GetChessmanByBoardIndex(row, 0), Board.Instance.GetActualPos(row), Board.Instance.GetActualPos(2));

			//else if (xDelta == 2)
			//	Board.Instance.MakeMove(Board.Instance.GetChessmanByBoardIndex(row, 7), Board.Instance.GetActualPos(row), Board.Instance.GetActualPos(5));
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
		Rook rook = Board.Instance.GetChessmanByBoardIndex(row, 7) as Rook;

		if (rook != null 
			&& (isWhite ? rook.isWhite : !rook.isWhite) 
			&& Board.Instance.Cells[row, 5] == Cell.Empty
			&& Board.Instance.Cells[row, 6] == Cell.Empty
			
			&& !Board.Instance.CellIsInDanger(row, 5, !isWhite)
			&& !Board.Instance.CellIsInDanger(row, 6, !isWhite))
		{
			validMoves.Add(new Move(row, 6, isKill: false, isCastle: true));
		}

		rook = Board.Instance.GetChessmanByBoardIndex(row, 0) as Rook;

		if (rook != null 
			&& (isWhite ? rook.isWhite : !rook.isWhite) 
			&& Board.Instance.Cells[row, 1] == Cell.Empty
			&& Board.Instance.Cells[row, 2] == Cell.Empty
			&& Board.Instance.Cells[row, 3] == Cell.Empty
			
			&& !Board.Instance.CellIsInDanger(row, 1, !isWhite)
			&& !Board.Instance.CellIsInDanger(row, 2, !isWhite)
			&& !Board.Instance.CellIsInDanger(row, 3, !isWhite))
		{
			validMoves.Add(new Move(row, 1, isKill: false, isCastle: true));
		}
	}

	public bool IsUnderThreat()
	{
		return ToBeKilled(Board.Instance.Cells, Enemies);
	}

	public bool WillBeKilledAfterMove(int startY, int startX, int endY, int endX, King kingComponent)
	{
		Cell[,] boardAfterMove = (Cell[,])Board.Instance.Cells.Clone();
		List<Chessman> enemies = new List<Chessman>(Enemies);

		if ((boardAfterMove[endY, endX] & Enemy) == Enemy)
			enemies.Remove(Board.Instance.GetChessmanByBoardIndex(endY, endX));

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
