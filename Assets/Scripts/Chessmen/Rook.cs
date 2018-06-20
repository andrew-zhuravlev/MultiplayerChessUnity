using System.Collections.Generic;
using UnityEngine;

public class Rook : Chessman
{
	[HideInInspector] public bool hasMoved = false;

	protected override List<Move> GetValidMoves(bool checkFriendlyKingSafety, Cell[,] board)
	{
		return RookMovementProvider.GetValidMoves(Y_Board, X_Board, isWhite, checkFriendlyKingSafety ? 
			(isWhite ? Board.Instance.WhiteKing : Board.Instance.BlackKing) : null, board);
	}

	public override void OnMove(int z, int x)
	{
		hasMoved = true;
	}
}