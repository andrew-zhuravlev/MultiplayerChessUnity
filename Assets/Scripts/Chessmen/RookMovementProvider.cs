using System.Collections.Generic;

public static class RookMovementProvider
{
	public static List<Move> GetValidMoves(int startY, int startX, bool isWhite, King friendlyKing, Cell[,] board)
	{
		List<Move> validMoves = new List<Move>();

		bool moveFurther = true;
		for (int y = startY + 1; y <= 7 && moveFurther; y++)
			MoveChecker.CheckMove(startY, startX, y, startX, isWhite, board[y, startX], validMoves, friendlyKing, ref moveFurther);

		moveFurther = true;
		for (int y = startY - 1; y >= 0 && moveFurther; y--)
			MoveChecker.CheckMove(startY, startX, y, startX, isWhite, board[y, startX], validMoves, friendlyKing, ref moveFurther);

		moveFurther = true;
		for (int x = startX + 1; x <= 7 && moveFurther; x++)
			MoveChecker.CheckMove(startY, startX, startY, x, isWhite, board[startY, x], validMoves, friendlyKing, ref moveFurther);

		moveFurther = true;
		for (int x = startX - 1; x >= 0 && moveFurther; x--)
			MoveChecker.CheckMove(startY, startX, startY, x, isWhite, board[startY, x], validMoves, friendlyKing, ref moveFurther);

		return validMoves;
	}
}