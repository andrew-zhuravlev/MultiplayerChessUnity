﻿using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public abstract class Chessman : MonoBehaviour
{
	protected Board board;

	public void Start()
	{
		board = Board.Instance;
	}

	public bool isWhite;

	public int Y { get { return board.GetBoardPos((int)transform.position.z); } }
	public int X { get { return board.GetBoardPos((int)transform.position.x); } }
	
	public bool ThreatForEnemyKing(Cell[,] newBoard)
	{
		return GetValidMoves(checkFriendlyKingSafety: false, board: newBoard)
			.Any(move => move.isKill && (newBoard[move.y, move.x] & Cell.King) == Cell.King);
	}

	public bool CanKillCell(int y, int x)
	{
		return GetValidMoves(checkFriendlyKingSafety: false, board: board.Cells)
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

	protected abstract List<Move> GetValidMoves(bool checkFriendlyKingSafety, Cell[,] board);

	public List<Move> GetValidMoves()
	{
		return GetValidMoves(checkFriendlyKingSafety: true, board: board.Cells);
	}

	public virtual void OnMove(int z, int x) { }
}