using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public abstract class Chessman : MonoBehaviour
{
	public bool isWhite;

	void OnDisable()
	{
		Debug.Log("OnDisable");

		if (Board.Instance != null)
			Board.Instance.RemoveChessman(this);
	}

	public int Y_Board { get { return Board.Instance.GetBoardPos((int)transform.position.z); } }

	public int X_Board { get { return Board.Instance.GetBoardPos((int)transform.position.x); } }

	public King EnemyKing
	{
		get { return isWhite ? Board.Instance.BlackKing : Board.Instance.WhiteKing; }
	}
	
	public bool ThreatForEnemyKing(Cell[,] newBoard)
	{
		return GetValidMoves(checkFriendlyKingSafety: false, board: newBoard)
			.Any(move => move.isKill && (newBoard[move.z, move.x] & Cell.King) == Cell.King);
	}

	public bool CanKillCell(int y, int x)
	{
		return GetValidMoves(checkFriendlyKingSafety: false, board: Board.Instance.GetCells())
			.Any(move => move.z == y && move.x == x);
	}

	protected List<Chessman> Enemies
	{
		get { return isWhite ? Board.Instance.BlackChessmen : Board.Instance.WhiteChessmen; }
	}

	protected List<Chessman> FriendsIncludingMe
	{
		get { return isWhite ? Board.Instance.WhiteChessmen : Board.Instance.BlackChessmen; }
	}

	protected Cell Enemy
	{
		get { return isWhite ? Cell.BlackFigure : Cell.WhiteFigure; }
	}

	protected Cell Friend
	{
		get { return isWhite ? Cell.WhiteFigure : Cell.BlackFigure; }
	}

	protected abstract List<Move> GetValidMoves(bool checkFriendlyKingSafety, Cell[,] board);

	public List<Move> GetValidMoves()
	{
		return GetValidMoves(checkFriendlyKingSafety: true, board: Board.Instance.GetCells());
	}

	public virtual void OnMove(int z, int x) { }
}