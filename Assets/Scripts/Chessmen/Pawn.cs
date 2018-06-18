using System.Collections.Generic;

//TODO: Взятие на проходе
public class Pawn : Chessman
{
	protected override List<Move> GetValidMoves(bool checkFriendlyKingSafety, Cell[,] board)
	{
		List<Move> validMoves = new List<Move>();

		int endY, endX;

		if (isWhite)
			endY = Y_Board + 1;

		else
			endY = Y_Board - 1;

		for (int i = -1; i <= 1; i++)
		{
			endX = X_Board + i;

			CheckMove(endY, endX, validMoves, checkFriendlyKingSafety, board);
		}

		bool initialCell = isWhite ? Y_Board == 1 : Y_Board == 6;
		if (initialCell && board[isWhite ? Y_Board + 1 : Y_Board - 1, X_Board] == Cell.Empty)
			CheckMove(isWhite ? Y_Board + 2 : Y_Board - 2, X_Board, validMoves, checkFriendlyKingSafety, board);

		return validMoves;
	}

	void CheckMove(int endY, int endX, List<Move> validMoves, bool checkFriendlyKingSafety, Cell[,] board)
	{
		if (endX < 0 || endX > 7)
			return;

		int xDelta = endX - X_Board;
		Cell curCell = board[endY, endX];
		bool validKillMove = xDelta != 0 && (curCell & Enemy) == Enemy;
		bool validForwardMove = xDelta == 0 && curCell == Cell.Empty;

		bool isValidMove = validKillMove || validForwardMove;
		if (checkFriendlyKingSafety)
			isValidMove = isValidMove && !(isWhite ? this.board.WhiteKing : this.board.BlackKing).WillBeKilledAfterMove(Y_Board, X_Board, endY, endX, null);

		if (isValidMove)
			validMoves.Add(new Move(endY, endX, isKill: validKillMove, isCastle: false));
	}

	public override void OnMove(int z, int x)
	{
		if (isWhite ? Y_Board == 7 : Y_Board == 0)
			TurnIntoQueen();
	}

	void TurnIntoQueen()
	{
		//TODO:
		//Something to do with networking
		//Instantiate(isWhite ? ObjectPooler.SharedInstance.GetPooledObject(2) : ObjectPooler.SharedInstance.GetPooledObject(3));

		//Probably use NetworkServer.Spawn(); or NetworkServer.SpawnWithClientAuthority();

		Destroy(gameObject);
	}
}
