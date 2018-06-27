using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

public class Board_FOR_TEST : NetworkBehaviour
{
	#region Singleton
	public static Board_FOR_TEST Instance { get; private set; }
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
	public GameObject WhiteQueenPrefab { get; private set; }
	public GameObject BlackQueenPrefab { get; private set; }

	public King WhiteKing { get; private set; }
	public King BlackKing { get; private set; }

	public List<Chessman> WhiteChessmen { get; private set; }
	public List<Chessman> BlackChessmen { get; private set; }

	Cell[,] cells;
	public Cell[,] GetCells()
	{
		return cells;
	}

	[SyncVar]
	bool _whiteMoves = true;

	public bool WhiteMoves
	{
		get
		{
			return _whiteMoves;
		}
	}

	[Server]
	public void SwapPlayer()
	{
		_whiteMoves = !_whiteMoves;
	}

	const int CELL_SIZE = 9;
	#endregion

	#region Helper Methods
	public void AddQueenToList(Queen queen, bool isWhite)
	{
		(isWhite ? WhiteChessmen : BlackChessmen).Add(queen);
	}

	public void RemoveChessman(Chessman toRemove)
	{
		(toRemove.isWhite ? WhiteChessmen : BlackChessmen).Remove(toRemove);
	}

	public int GetWorldPos(int boardCoordinate) { return boardCoordinate * CELL_SIZE; }

	public int GetBoardPos(int worldCoordinate) { return worldCoordinate / CELL_SIZE; }

	public TComponent GetComponentInChessman<TComponent>(int z_Board, int x_Board)
	{
		RaycastHit hit;
		Ray ray = new Ray(new Vector3(GetWorldPos(x_Board), 30f, GetWorldPos(z_Board)), Vector3.down);

		if (Physics.Raycast(ray, out hit, 30f, LayerMask.GetMask("Chessmen")))
			return hit.collider.GetComponent<TComponent>();

		Debug.LogWarning("GetChessmanByBoardIndex:: Collider not found. Position: (" + z_Board + " " + x_Board + ")");
		return default(TComponent);
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

		WhiteQueenPrefab = Resources.Load("White Queen", typeof(GameObject)) as GameObject;
		BlackQueenPrefab = Resources.Load("Black Queen", typeof(GameObject)) as GameObject;
	}

	// This should work, but on the FOR_TESTING Scene obviously should be both kings.
	void Init_Chessmen()
	{
		Chessman[] chessmen = FindObjectsOfType<Chessman>();

		WhiteChessmen = new List<Chessman>();
		BlackChessmen = new List<Chessman>();

		for (int i = 0; i < chessmen.Length; i++)
		{
			if (chessmen[i].isWhite)
				WhiteChessmen.Add(chessmen[i]);
			else
				BlackChessmen.Add(chessmen[i]);
		}

		King[] kings = FindObjectsOfType<King>();

		for (int i = 0; i < 2; i++)
		{
			if (kings[i].isWhite)
				WhiteKing = kings[i];
			else
				BlackKing = kings[i];
		}
	}

	void Init_Cells()
	{       
		cells = new Cell[8, 8];

		// This part differs from original Board script.
		CellUtils.UpdateCells();
	}
#endregion

#region Cell Methods
	public void FreeCell(int z, int x)
	{
		cells[z, x] = Cell.Empty;
	}

	public void SetCell(int fromZ, int fromX, int toZ, int toX, bool isWhite, bool isKing)
	{
		GetCells()[fromZ, fromX] = Cell.Empty;
		GetCells()[toZ, toX] = (isWhite ? Cell.WhiteFigure : Cell.BlackFigure);

		if (isKing)
			GetCells()[toZ, toX] |= Cell.King;
	}
#endregion

#region Highlighters
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
				objectIndex = 2;

			else if (p.isKill)
			{
				Chessman chessman = GetComponentInChessman<Chessman>(p.z, p.x);

				//Getting the height of Red Cube above the figure to kill.
				//My figures will be replaced anyway, this should be fixed.
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
			GameObject highlighter = ObjectPooler.Instance.GetPooledObject(objectIndex);
			Vector3 newPos = new Vector3(GetWorldPos(p.x), height_Y, GetWorldPos(p.z));
			highlighter.transform.position = newPos;
			highlighter.SetActive(true);
		}
	}

	public void RemoveHighlighters()
	{
		for (int i = 0; i < 3; i++)
		{
			foreach (GameObject pooled in ObjectPooler.Instance.GetAllPooledObjects(i))
			{
				if (pooled.activeInHierarchy)
					pooled.SetActive(false);
			}
		}
	}
#endregion
}
