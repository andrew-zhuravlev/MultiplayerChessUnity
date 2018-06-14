public class Bishop : Chessman
{
	protected override Move[] GetValidMoves(bool checkFriendlyKingSafety, Cell[,] board)
	{
		return BishopMovementProvider.GetValidMoves(Y, X, isWhite, checkFriendlyKingSafety ?
			(isWhite ? FindObjectOfType<Board>().WhiteKing : FindObjectOfType<Board>().BlackKing) : null, board);
	}
}
