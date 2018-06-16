using System.Collections.Generic;

//TODO: Fix the problem where after the move of knight nobody can move
public class Knight : Chessman
{
	protected override Move[] GetValidMoves(bool checkFriendlyKingSafety, Cell[,] board)
	{
		List<Move> result = new List<Move>();

		CheckMove(Y + 2, X - 1, result, checkFriendlyKingSafety, board);
		CheckMove(Y + 2, X + 1, result, checkFriendlyKingSafety, board);

		CheckMove(Y - 2, X - 1, result, checkFriendlyKingSafety, board);
		CheckMove(Y - 2, X + 1, result, checkFriendlyKingSafety, board);

		CheckMove(Y + 1, X + 2, result, checkFriendlyKingSafety, board);
		CheckMove(Y - 1, X + 2, result, checkFriendlyKingSafety, board);

		CheckMove(Y + 1, X - 2, result, checkFriendlyKingSafety, board);
		CheckMove(Y - 1, X - 2, result, checkFriendlyKingSafety, board);

		return result.ToArray();
	}

	void CheckMove(int endY, int endX, List<Move> result, bool checkFriendlyKingSafety, Cell[,] board)
	{
		if (endY < 0 || endY > 7 || endX < 0 || endX > 7 || (board[endY, endX] & Friend) == Friend || !(checkFriendlyKingSafety ?
			!(isWhite ? Board.WhiteKing : Board.BlackKing).WillBeKilledAfterMove(Y, X, endY, endX, null) : true))
			return;

		if (board[endY, endX] == Cell.Empty)
			result.Add(new Move(endY, endX, isKill: false, isCastle: false));

		else result.Add(new Move(endY, endX, isKill: true, isCastle: false));
	}
}