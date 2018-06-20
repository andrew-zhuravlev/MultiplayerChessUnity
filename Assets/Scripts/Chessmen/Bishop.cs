using System.Collections.Generic;

public class Bishop : Chessman
{
	protected override List<Move> GetValidMoves(bool checkFriendlyKingSafety, Cell[,] board)
	{
		return BishopMovementProvider.GetValidMoves(Y_Board, X_Board, isWhite, checkFriendlyKingSafety ?
			(isWhite ? Board.Instance.WhiteKing : Board.Instance.BlackKing) : null, board);
	}
}
