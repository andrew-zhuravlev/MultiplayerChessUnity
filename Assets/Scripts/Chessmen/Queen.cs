public class Queen : Chessman
{
	protected override Move[] GetValidMoves(bool checkFriendlyKingSafety, Cell[,] board)
	{
		Move[] rookMoves = RookMovementProvider.GetValidMoves(Y, X, isWhite, checkFriendlyKingSafety ?
			(isWhite ? FindObjectOfType<Board>().WhiteKing : FindObjectOfType<Board>().BlackKing) : null, board);

		Move[] bishopMoves = BishopMovementProvider.GetValidMoves(Y, X, isWhite, checkFriendlyKingSafety ?
			(isWhite ? FindObjectOfType<Board>().WhiteKing : FindObjectOfType<Board>().BlackKing) : null, board);

		Move[] validMoves = new Move[rookMoves.Length + bishopMoves.Length];

		rookMoves.CopyTo(validMoves, 0);
		bishopMoves.CopyTo(validMoves, rookMoves.Length);

		return validMoves;
	}
}
