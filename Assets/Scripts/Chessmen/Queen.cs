using System.Collections.Generic;

public class Queen : Chessman
{
	protected override List<Move> GetValidMoves(bool checkFriendlyKingSafety, Cell[,] board)
	{
		List<Move> rookMoves = RookMovementProvider.GetValidMoves(Y_Board, X_Board, isWhite, checkFriendlyKingSafety ?
			(isWhite ? Board.Instance.WhiteKing : Board.Instance.BlackKing) : null, board);

		List<Move> bishopMoves = BishopMovementProvider.GetValidMoves(Y_Board, X_Board, isWhite, checkFriendlyKingSafety ?
			(isWhite ? Board.Instance.WhiteKing : Board.Instance.BlackKing) : null, board);

		rookMoves.AddRange(bishopMoves);

		return rookMoves;
	}
}
