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
		if (EditorApplication.isPlaying && FindObjectOfType<Board>().Cells != null)
		{
			if (GUILayout.Button("Update the Cells") || update)
				UpdateCells();

			DisplayCells();
		}
		else update = true;
	}

	public static void DisplayCells()
	{
		for (int y = FindObjectOfType<Board>().Cells.GetLength(0) - 1; y >= 0; y--)
		{
			EditorGUILayout.BeginHorizontal();

			for (int x = 0; x < FindObjectOfType<Board>().Cells.GetLength(1); x++)
			{
				if (FindObjectOfType<Board>().Cells[y, x] == Cell.Empty)
					GUI.color = Color.gray;

				else if ((FindObjectOfType<Board>().Cells[y, x] & Cell.King) == Cell.King)
					GUI.color = Color.green;

				else if (FindObjectOfType<Board>().Cells[y, x] == Cell.BlackFigure)
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
				Ray ray = new Ray(new Vector3(FindObjectOfType<Board>().GetActualPos(x), 20, FindObjectOfType<Board>().GetActualPos(y)), Vector3.down);

				if (Physics.Raycast(ray, out hit, 20, LayerMask.GetMask("Chessmen")))
				{
					if (hit.collider.GetComponent<Chessman>().isWhite)
						FindObjectOfType<Board>().Cells[y, x] = Cell.WhiteFigure;

					else
						FindObjectOfType<Board>().Cells[y, x] = Cell.BlackFigure;

					if (hit.collider.GetComponent<King>() != null)
						FindObjectOfType<Board>().Cells[y, x] |= Cell.King;
				}

				else FindObjectOfType<Board>().Cells[y, x] = Cell.Empty;
			}

		update = false;
	}
}