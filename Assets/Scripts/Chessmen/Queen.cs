using System.Collections.Generic;
using System.Linq;

public class Queen : Chessman
{
	protected override List<Move> GetValidMoves(bool checkFriendlyKingSafety, Cell[,] board)
	{
		List<Move> rookMoves = RookMovementProvider.GetValidMoves(Y, X, isWhite, checkFriendlyKingSafety ?
			(isWhite ? this.board.WhiteKing : this.board.BlackKing) : null, board);

		List<Move> bishopMoves = BishopMovementProvider.GetValidMoves(Y, X, isWhite, checkFriendlyKingSafety ?
			(isWhite ? this.board.WhiteKing : this.board.BlackKing) : null, board);

		return rookMoves.Concat(bishopMoves).ToList();
	}
}
