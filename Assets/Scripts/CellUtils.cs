using UnityEngine;

public class CellUtils : MonoBehaviour
{
	public static void UpdateCells()
	{
		for (int y = 0; y < 8; y++)
		{
			for (int x = 0; x < 8; x++)
			{
				SetCell(y, x);
			}
		}
	}

	static void SetCell(int y, int x)
	{
		RaycastHit hit;
		Ray ray = new Ray(new Vector3(FindObjectOfType<Board>().GetWorldPos(x), 20, FindObjectOfType<Board>().GetWorldPos(y)), Vector3.down);

		if (Physics.Raycast(ray, out hit, 20, LayerMask.GetMask("Chessmen")))
		{
			if (hit.collider.GetComponent<Chessman>().isWhite)
				Board.Instance.GetCells()[y, x] = Cell.WhiteFigure;

			else
				Board.Instance.GetCells()[y, x] = Cell.BlackFigure;

			if (hit.collider.GetComponent<King>() != null)
				Board.Instance.GetCells()[y, x] |= Cell.King;
		}

		else Board.Instance.GetCells()[y, x] = Cell.Empty;
	}
}