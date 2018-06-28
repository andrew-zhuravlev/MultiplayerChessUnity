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
			{
				CellUtils.UpdateCells();
				update = false;
			}

			DisplayCells();

			// Display amount of chessmen after cells.
			GUILayout.Label("WhiteChessmen: " + Board.Instance.WhiteChessmen.Count);
			GUILayout.Label("BlackChessmen: " + Board.Instance.BlackChessmen.Count);
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
}
