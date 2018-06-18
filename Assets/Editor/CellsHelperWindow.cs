using UnityEngine;
using UnityEditor;

public class CellsHelperWindow : EditorWindow
{
	static bool update;

	[MenuItem("Window/CellHelperWindow")]
	public static void ShowWindow()
	{
		GetWindow<CellsHelperWindow>();
	}

	private void OnInspectorUpdate()
	{
		Repaint();
	}

	void OnGUI()
	{
		if (EditorApplication.isPlaying && Board.Instance.GetCells() != null)
		{
			if (GUILayout.Button("Update the Cells") || update)
				UpdateCells();

			DisplayCells();
		}
		else update = true;
	}

	public static void DisplayCells()
	{
		for (int y = Board.Instance.GetCells().GetLength(0) - 1; y >= 0; y--)
		{
			EditorGUILayout.BeginHorizontal();

			for (int x = 0; x < Board.Instance.GetCells().GetLength(1); x++)
			{
				if (Board.Instance.GetCells()[y, x] == Cell.Empty)
					GUI.color = Color.gray;

				else if ((Board.Instance.GetCells()[y, x] & Cell.King) == Cell.King)
					GUI.color = Color.green;

				else if (Board.Instance.GetCells()[y, x] == Cell.BlackFigure)
					GUI.color = Color.black;

				else
					GUI.color = Color.white;

				GUILayout.Box("", GUILayout.Width(30), GUILayout.Height(30));
			}

			EditorGUILayout.EndHorizontal();
		}
	}

	public static void UpdateCells()
	{
		for (int y = 0; y < 8; y++)
			for (int x = 0; x < 8; x++)
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

		update = false;
	}
}