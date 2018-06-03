using System.Collections.Generic;
using UnityEngine;

public class MoveChecker : MonoBehaviour 
{
	public static void CheckMove(int startY, int startX, int endY, int endX, bool isWhite, Cell endPos_Cell, List<Move> validMoves, King friendlyKing, ref bool moveFurther)
	{
		bool isFriend = (endPos_Cell & (isWhite ? Cell.WhiteFigure : Cell.BlackFigure)) == (isWhite ? Cell.WhiteFigure : Cell.BlackFigure);
		bool kingWillBeKilled = friendlyKing != null && friendlyKing.WillBeKilledAfterMove(startY, startX, endY, endX, null);

		if (endPos_Cell == Cell.Empty)
		{
			if (!kingWillBeKilled)
				validMoves.Add(new Move(endY, endX, false, false));
		}

		else
		{
			moveFurther = false;

			if (isFriend)
				return;

			if (!kingWillBeKilled)
				validMoves.Add(new Move(endY, endX, true, false));
		}
	}
}
