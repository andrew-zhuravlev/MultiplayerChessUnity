using System.Collections.Generic;

//TODO: Взятие на проходе
public class Pawn : Chessman
{
	protected override Move[] GetValidMoves(bool checkFriendlyKingSafety, Cell[,] board)
	{
		List<Move> validMoves = new List<Move>();

		int endY, endX;

		if (isWhite)
			endY = Y + 1;

		else
			endY = Y - 1;

		for (int i = -1; i <= 1; i++)
		{
			endX = X + i;

			CheckMove(endY, endX, validMoves, checkFriendlyKingSafety, board);
		}

		bool initialCell = isWhite ? Y == 1 : Y == 6;
		if (initialCell && board[isWhite ? Y + 1 : Y - 1, X] == Cell.Empty)
			CheckMove(isWhite ? Y + 2 : Y - 2, X, validMoves, checkFriendlyKingSafety, board);

		return validMoves.ToArray();
	}

	void CheckMove(int endY, int endX, List<Move> validMoves, bool checkFriendlyKingSafety, Cell[,] board)
	{
		if (endX < 0 || endX > 7)
			return;

		int xDelta = endX - X;
		Cell curCell = board[endY, endX];
		bool validKillMove = xDelta != 0 && (curCell & Enemy) == Enemy;
		bool validForwardMove = xDelta == 0 && curCell == Cell.Empty;

		bool isValidMove = validKillMove || validForwardMove;
		if (checkFriendlyKingSafety)
			isValidMove = isValidMove && !(isWhite ? FindObjectOfType<Board>().WhiteKing : FindObjectOfType<Board>().BlackKing).WillBeKilledAfterMove(Y, X, endY, endX, null);

		if (!isValidMove)
			return;

		if (validKillMove)
			validMoves.Add(new Move(endY, endX, true, isCastle: false));

		else
			validMoves.Add(new Move(endY, endX, false, isCastle: false));
	}

	public override void OnMove(int z, int x)
	{
		if (isWhite ? Y == 7 : Y == 0)
			TurnIntoQueen();
	}

	void TurnIntoQueen()
	{
		//TODO:
		//Something to do with networking
		//Instantiate(isWhite ? ObjectPooler.SharedInstance.GetPooledObject(2) : ObjectPooler.SharedInstance.GetPooledObject(3));

		Destroy(gameObject);
	}
}
