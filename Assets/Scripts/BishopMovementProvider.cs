using System.Collections.Generic;

static class BishopMovementProvider
{
	public static Move[] GetValidMoves(int startY, int startX, bool isWhite, King friendlyKing, Cell[,] board)
	{
		List<Move> validMoves = new List<Move>();

		bool moveFurther = true;
		for (int y = startY + 1, x = startX + 1; y <= 7 && x <= 7 && moveFurther; y++, x++)
			MoveChecker.CheckMove(startY, startX, y, x, isWhite, board[y, x], validMoves, friendlyKing, ref moveFurther);

		moveFurther = true;
		for (int y = startY + 1, x = startX - 1; y <= 7 && x >= 0 && moveFurther; y++, x--)
			MoveChecker.CheckMove(startY, startX, y, x, isWhite, board[y, x], validMoves, friendlyKing, ref moveFurther);

		moveFurther = true;
		for (int y = startY - 1, x = startX + 1; y >= 0 && x <= 7 && moveFurther; y--, x++)
			MoveChecker.CheckMove(startY, startX, y, x, isWhite, board[y, x], validMoves, friendlyKing, ref moveFurther);

		moveFurther = true;
		for (int y = startY - 1, x = startX - 1; y >= 0 && x >= 0 && moveFurther; y--, x--)
			MoveChecker.CheckMove(startY, startX, y, x, isWhite, board[y, x], validMoves, friendlyKing, ref moveFurther);

		return validMoves.ToArray();
	}
}
