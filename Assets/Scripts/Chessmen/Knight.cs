using System.Collections.Generic;

public class Knight : Chessman
{
	protected override List<Move> GetValidMoves(bool checkFriendlyKingSafety, Cell[,] board)
	{
		List<Move> result = new List<Move>();

		CheckMove(Y_Board + 2, X_Board - 1, result, checkFriendlyKingSafety, board);
		CheckMove(Y_Board + 2, X_Board + 1, result, checkFriendlyKingSafety, board);

		CheckMove(Y_Board - 2, X_Board - 1, result, checkFriendlyKingSafety, board);
		CheckMove(Y_Board - 2, X_Board + 1, result, checkFriendlyKingSafety, board);

		CheckMove(Y_Board + 1, X_Board + 2, result, checkFriendlyKingSafety, board);
		CheckMove(Y_Board - 1, X_Board + 2, result, checkFriendlyKingSafety, board);

		CheckMove(Y_Board + 1, X_Board - 2, result, checkFriendlyKingSafety, board);
		CheckMove(Y_Board - 1, X_Board - 2, result, checkFriendlyKingSafety, board);

		return result;
	}

	void CheckMove(int endY, int endX, List<Move> result, bool checkFriendlyKingSafety, Cell[,] board)
	{
		if (endY < 0 || endY > 7 || endX < 0 || endX > 7 || (board[endY, endX] & Friend) == Friend || !(checkFriendlyKingSafety ?
			!(isWhite ? Board.Instance.WhiteKing : Board.Instance.BlackKing).WillBeKilledAfterMove(Y_Board, X_Board, endY, endX, null) : true))
			return;

		if (board[endY, endX] == Cell.Empty)
			result.Add(new Move(endY, endX, isKill: false, isCastle: false));

		else
			result.Add(new Move(endY, endX, isKill: true, isCastle: false));
	}
}