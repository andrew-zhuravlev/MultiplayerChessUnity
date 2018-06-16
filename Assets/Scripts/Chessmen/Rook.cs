using UnityEngine;

public class Rook : Chessman
{
	[HideInInspector] public bool hasMoved = false;

	protected override Move[] GetValidMoves(bool checkFriendlyKingSafety, Cell[,] board)
	{
		return RookMovementProvider.GetValidMoves(Y, X, isWhite, checkFriendlyKingSafety ? 
			(isWhite ? Board.Instance.WhiteKing : Board.Instance.BlackKing) : null, board);
	}

	public override void OnMove(int z, int x)
	{
		hasMoved = true;
	}
}