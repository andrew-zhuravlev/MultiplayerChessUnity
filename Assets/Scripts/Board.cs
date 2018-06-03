using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Board : MonoSingleton<Board>
{
	#region Variables
	[HideInInspector] public King WhiteKing;
	[HideInInspector] public King BlackKing;
	[HideInInspector] public List<Chessman> WhiteChessmen = new List<Chessman>();
	[HideInInspector] public List<Chessman> BlackChessmen = new List<Chessman>();

	public Dictionary<int, Transform> AllChessmen = new Dictionary<int, Transform>();

	public Cell[,] Cells { get; private set; }

	const int CELL_SIZE = 9;

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
		print("Board Init");
		Init_FindChessmen();
		Init_Cells();
	}

	void Init_FindChessmen()
	{
		Chessman[] chessmen = FindObjectsOfType<Chessman>();

		WhiteChessmen = chessmen.Where(chessman => chessman.isWhite).ToList();
		BlackChessmen = chessmen.Where(chessman => !chessman.isWhite).ToList();

		WhiteKing = FindObjectsOfType<King>().First(k => k.isWhite);
		BlackKing = FindObjectsOfType<King>().First(k => !k.isWhite);

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

	void SetCell(int startY, int startX, int endY, int endX, bool isWhite, King kingComponent)
	{
		Cells[startY, startX] = Cell.Empty;
		Cells[endY, endX] = (isWhite ? Cell.WhiteFigure : Cell.BlackFigure);

		if (kingComponent != null)
			Cells[endY, endX] |= Cell.King;
	}

	public void DisplayHighlighters(Move[] possibleMoves)
	{
		if (possibleMoves.Count() == 0)
		{
			Debug.Log("No Possible moves");
			return;
		}

		foreach (var p in possibleMoves)
		{
			int objectIndex; float height_Y = 1;

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
					case "King": Debug.LogError("DisplayValidMoves:: Entered King tag in switch chessman.tag"); break;
					default: Debug.LogError("DisplayValidMoves:: No Such Tag Found"); break;
				}

				objectIndex = 1;
			}

			else
				objectIndex = 0;

			GameObject highlighter = ObjectPooler.SharedInstance.GetPooledObject(objectIndex);
			highlighter.SetActive(true);

			Vector3 newPos = new Vector3(GetActualPos(p.x), height_Y, GetActualPos(p.y));
			highlighter.transform.position = newPos;
		}
	}

	public void RemoveHighlighters()
	{
		foreach (var moveHighlighter in GameObject.FindGameObjectsWithTag("Move Highlighter"))
			moveHighlighter.SetActive(false);

		foreach (var castleMoveHighlighter in GameObject.FindGameObjectsWithTag("Castle Move Highlighter"))
			castleMoveHighlighter.SetActive(false);
	}

	public void MakeMove(Chessman c, NetworkIdentity networkIdentity, int z, int x)
	{
		SetCell(c.Y, c.X, GetBoardPos(z), GetBoardPos(x), c.isWhite, c as King);
		c.OnMove(z, x);

		Server.Instance.RpcMoveFigure(networkIdentity, x, z);

		RemoveHighlighters();
	}
}
