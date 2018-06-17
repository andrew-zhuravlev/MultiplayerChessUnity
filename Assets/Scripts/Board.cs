using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Board : NetworkBehaviour
{
	#region Singleton
	public static Board Instance { get; private set; }
	void Awake()
	{
		Instance = this;
	}
	void OnDestroy()
	{
		Instance = null;
	}
	#endregion

	#region Variables
	public King WhiteKing { get; private set; }
	public King BlackKing { get; private set; }
	public List<Chessman> WhiteChessmen { get; private set; }
	public List<Chessman> BlackChessmen { get; private set; }
	public Dictionary<int, Transform> AllChessmen { get; private set; }

	public Cell[,] Cells { get; private set; }

	[SyncVar]
	public bool WhiteMoves = true;

	const int CELL_SIZE = 9;
	#endregion

	#region Helper Methods
	public int GetActualPos(int boardCoordinate) { return boardCoordinate * CELL_SIZE; }

	public int GetBoardPos(int worldCoordinate) { return worldCoordinate / CELL_SIZE; }

	public Chessman GetChessmanByBoardIndex(int y, int x)
	{
		RaycastHit hit;
		Ray ray = new Ray(new Vector3(GetActualPos(x), 30f, GetActualPos(y)), Vector3.down);

		if (Physics.Raycast(ray, out hit, 30f, LayerMask.GetMask("Chessmen")))
			return hit.collider.GetComponent<Chessman>();

		Debug.LogWarning("GetChessmanByBoardIndex:: Collider not found. Position: (" + y + " " + x + ")");
		return null;
	}

	public bool CellIsInDanger(int y, int x, bool enemy_IsWhite)
	{
		return enemy_IsWhite ? WhiteChessmen.Any(whiteChessman => whiteChessman.CanKillCell(y, x))
			: BlackChessmen.Any(blackChessman => blackChessman.CanKillCell(y, x));
	}
	#endregion

	#region Initialization
	public void Init()
	{
		Init_Cells();
		Init_Chessmen();
	}

	void Init_Chessmen()
	{
		Chessman[] chessmen = FindObjectsOfType<Chessman>();

		WhiteChessmen = new List<Chessman>();
		BlackChessmen = new List<Chessman>();
		AllChessmen = new Dictionary<int, Transform>();

		for (int i = 0; i < chessmen.Length; i++)
		{
			if (chessmen[i].isWhite)
				WhiteChessmen.Add(chessmen[i]);
			else
				BlackChessmen.Add(chessmen[i]);
		}

		King[] kings = FindObjectsOfType<King>();

		for(int i = 0; i < 2; i++)
		{
			if (kings[i].isWhite)
				WhiteKing = kings[i];
			else
				BlackKing = kings[i];
		}

		foreach (var c in chessmen)
			AllChessmen.Add(c.transform.GetInstanceID(), c.transform);
	}

	void Init_Cells()
	{
		Cells = new Cell[8, 8];

		for (int i = 0; i < 4; i++)
			for (int j = 0; j < 8; j++)
				Cells[i < 2 ? i : i + 4, j] = i < 2 ? Cell.WhiteFigure : Cell.BlackFigure;

		Cells[0, 4] |= Cell.King;
		Cells[7, 4] |= Cell.King;
	}
	#endregion

	public void SetCell(int startY, int startX, int endY, int endX, bool isWhite, bool isKing)
	{
		Cells[startY, startX] = Cell.Empty;
		Cells[endY, endX] = (isWhite ? Cell.WhiteFigure : Cell.BlackFigure);

		if (isKing)
			Cells[endY, endX] |= Cell.King;
	}

	public void DisplayHighlighters(List<Move> possibleMoves)
	{
		if (possibleMoves.Count() == 0)
		{
			Debug.Log("No Possible moves");
			return;
		}

		foreach (var p in possibleMoves)
		{
			int objectIndex;
			float height_Y = 1;

			if (p.isCastle)
				objectIndex = 4;

			else if (p.isKill)
			{
				Chessman chessman = GetChessmanByBoardIndex(p.y, p.x);

				switch (chessman.tag)
				{
					case "Pawn": height_Y = 11; break;
					case "Rook": height_Y = 10; break;
					case "Knight": height_Y = 13; break;
					case "Bishop": height_Y = 14; break;
					case "Queen": height_Y = 13.5f; break;
					case "King":
						Debug.LogError("DisplayValidMoves:: Entered King tag in switch chessman.tag");
						break;
					default:
						Debug.LogError("DisplayValidMoves:: Wrong tag");
						break;
				}

				objectIndex = 1;
			}

			else
				objectIndex = 0;

			//Sets the highlighter at correct place
			GameObject highlighter = ObjectPooler.SharedInstance.GetPooledObject(objectIndex);
			Vector3 newPos = new Vector3(GetActualPos(p.x), height_Y, GetActualPos(p.y));
			highlighter.transform.position = newPos;
			highlighter.SetActive(true);
		}
	}

	//TODO: How does it remove kill highlighters??
	public void RemoveHighlighters()
	{
		foreach (var moveHighlighter in GameObject.FindGameObjectsWithTag("Move Highlighter"))
			moveHighlighter.SetActive(false);

		foreach (var castleMoveHighlighter in GameObject.FindGameObjectsWithTag("Castle Move Highlighter"))
			castleMoveHighlighter.SetActive(false);
	}
}
