using System.Collections.Generic;
using System.Linq;

public class King : Chessman
{
	bool hasMoved = false;
	bool wasUnderThreat = false;

	List<Chessman> Enemies
	{
		get { return isWhite ? Board.Instance.BlackChessmen : Board.Instance.WhiteChessmen; }
	}

	public override void OnMove(int z, int x)
	{
		hasMoved = true;
	}

	protected override List<Move> GetValidMoves(bool checkSafety, Cell[,] board)
	{
		List<Move> validMoves = new List<Move>();

		for (int i = -1; i <= 1; i++)
		{
			CheckMove(Y_Board + 1, X_Board + i, validMoves, checkSafety, board);
			CheckMove(Y_Board - 1, X_Board + i, validMoves, checkSafety, board);
		}

		CheckMove(Y_Board, X_Board - 1, validMoves, checkSafety, board);
		CheckMove(Y_Board, X_Board + 1, validMoves, checkSafety, board);

		if (checkSafety)
			CheckCastle(validMoves);

		return validMoves;
	}

	void CheckMove(int endY, int endX, List<Move> validMoves, bool checkSafety, Cell[,] board)
	{
		bool isOutOfBounds = endY < 0 || endY > 7 || endX < 0 || endX > 7;

		if (isOutOfBounds || board[endY, endX] == Friend || (checkSafety && WillBeKilledAfterMove(Y_Board, X_Board, endY, endX, this)))
			return;

		if (board[endY, endX] == Cell.Empty)
			validMoves.Add(new Move(endY, endX, isKill: false, isCastle: false));

		else
			validMoves.Add(new Move(endY, endX, isKill: true, isCastle: false));
	}

	void CheckCastle(List<Move> validMoves)
	{
		if (wasUnderThreat || hasMoved)
			return;

		if (IsUnderThreat())
		{
			wasUnderThreat = true;
			return;
		}

		int row = isWhite ? 0 : 7;
		Rook rook = Board.Instance.GetComponentInChessman<Rook>(row, 7);

		if (rook != null
			&& !rook.hasMoved
			&& (isWhite == rook.isWhite) 
			&& Board.Instance.GetCells()[row, 5] == Cell.Empty
			&& Board.Instance.GetCells()[row, 6] == Cell.Empty
			
			&& !Board.Instance.CellIsInDanger(row, 5, !isWhite)
			&& !Board.Instance.CellIsInDanger(row, 6, !isWhite))
		{
			validMoves.Add(new Move(row, 6, isKill: false, isCastle: true));
		}

		rook = Board.Instance.GetComponentInChessman<Rook>(row, 0);

		if (rook != null
			&& !rook.hasMoved 
			&& (isWhite == rook.isWhite) 
			&& Board.Instance.GetCells()[row, 1] == Cell.Empty
			&& Board.Instance.GetCells()[row, 2] == Cell.Empty
			&& Board.Instance.GetCells()[row, 3] == Cell.Empty
			
			&& !Board.Instance.CellIsInDanger(row, 1, !isWhite)
			&& !Board.Instance.CellIsInDanger(row, 2, !isWhite)
			&& !Board.Instance.CellIsInDanger(row, 3, !isWhite))
		{
			validMoves.Add(new Move(row, 1, isKill: false, isCastle: true));
		}
	}

	public bool IsUnderThreat()
	{
		return ToBeKilled(Board.Instance.GetCells(), Enemies);
	}

	public bool WillBeKilledAfterMove(int startY, int startX, int endY, int endX, King kingComponent)
	{
		Cell[,] boardAfterMove = (Cell[,])Board.Instance.GetCells().Clone();
		List<Chessman> enemies = new List<Chessman>(Enemies);

		if ((boardAfterMove[endY, endX] & Enemy) == Enemy)
			enemies.Remove(Board.Instance.GetComponentInChessman<Chessman>(endY, endX));

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
