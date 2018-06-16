using UnityEngine;
using System.Linq;

public abstract class Chessman : MonoBehaviour
{
	public bool isWhite;

	public int Y { get { return Board.Instance.GetBoardPos((int)transform.position.z); } }
	public int X { get { return Board.Instance.GetBoardPos((int)transform.position.x); } }
	
	public bool ThreatForEnemyKing(Cell[,] newBoard)
	{
		return GetValidMoves(checkFriendlyKingSafety: false, board: newBoard)
			.Any(move => move.isKill && (newBoard[move.y, move.x] & Cell.King) == Cell.King);
	}

	public bool CanKillCell(int y, int x)
	{
		return GetValidMoves(checkFriendlyKingSafety: false, board: Board.Instance.Cells)
			.Any(move => move.y == y && move.x == x);
	}

	protected Cell Enemy
	{
		get { return isWhite ? Cell.BlackFigure : Cell.WhiteFigure; }
	}

	protected Cell Friend
	{
		get { return isWhite ? Cell.WhiteFigure : Cell.BlackFigure; }
	}

	protected abstract Move[] GetValidMoves(bool checkFriendlyKingSafety, Cell[,] board);

	public Move[] GetValidMoves()
	{
		return GetValidMoves(checkFriendlyKingSafety: true, board: Board.Instance.Cells);
	}

	public virtual void OnMove(int z, int x) { }
}