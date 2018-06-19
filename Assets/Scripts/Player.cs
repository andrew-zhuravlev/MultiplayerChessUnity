using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

public class Player : NetworkBehaviour
{
	Chessman selectedChessman;
	NetworkIdentity selectedChessman_NetworkIdentity;

	Board board;

	void Start()
	{
		board = Board.Instance;

		if (isLocalPlayer)
		{
			board.Init();

			FindObjectOfType<PlayerCamera>().SetDefaultPos(pointToWhite: isServer);
		}
	}

	void Update()
	{
		if (!isLocalPlayer || isServer != board.WhiteMoves)
			return;

		CheckSelection();
		CheckMovement();
	}

	void CheckSelection()
	{
		if (!Input.GetMouseButtonDown(0))
			return;

		RaycastHit hit;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Chessmen")))
		{
			Chessman chessmanComponent = hit.collider.GetComponent<Chessman>();

			if (chessmanComponent == null)
				Debug.LogError("CheckSelection:: ChessmanComponent is null");

			else if (selectedChessman == chessmanComponent)
			{
				selectedChessman = null;
				selectedChessman_NetworkIdentity = null;

				board.RemoveHighlighters();
			}

			else if (isServer == chessmanComponent.isWhite)
			{
				board.RemoveHighlighters();

				selectedChessman = chessmanComponent;
				selectedChessman_NetworkIdentity = hit.collider.GetComponent<NetworkIdentity>();

				board.DisplayHighlighters(chessmanComponent.GetValidMoves());
			}
		}
	}

	void CheckMovement()
	{
		if (!Input.GetMouseButtonDown(0))
			return;

		RaycastHit hit;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Highlighters")))
			MakeMove((int)hit.transform.position.z, (int)hit.transform.position.x);
	}

	void MakeMove(int toZ, int toX)
	{
		CmdMoveFigure(selectedChessman_NetworkIdentity, selectedChessman.Y_Board, selectedChessman.X_Board, toZ, toX);

		selectedChessman = null;
		selectedChessman_NetworkIdentity = null;

		board.RemoveHighlighters();
	}

	// TODO: Check if it was correct client.
	[Command]
	void CmdMoveFigure(NetworkIdentity identity, int fromZ_Board, int fromX_Board, int toZ_World, int toX_World)
	{
		if (NetworkServer.connections.Count < 2)
			return;

		Chessman identityChessman = identity.GetComponent<Chessman>();

		int boardToX = board.GetBoardPos(toX_World);
		int boardToZ = board.GetBoardPos(toZ_World);

		if (!identityChessman.GetValidMoves().Any(move => move.x == boardToX && move.z == boardToZ))
		{
			Debug.LogError("NOT LEGIT MOVE");
			return;
		}

		King kingComponent = identityChessman as King;
		bool isKing = kingComponent != null;

		if (isKing && kingComponent.Y_Board - board.GetBoardPos(toZ_World) == 0)
		{
			int xDelta = board.GetBoardPos(toX_World) - kingComponent.X_Board;

			if (xDelta == 2 || xDelta == -3)
			{
				int row = kingComponent.isWhite ? 0 : 7;

				int rookOldX = xDelta == -3 ? 0 : 7;
				int rookNewX = xDelta == -3 ? 2 : 5;

				bool rookIsWhite = row == 0;

				Chessman rook = board.GetChessmanByBoardIndex(row, rookOldX);
				ServerMoveFigure(rook, rook.GetComponent<NetworkIdentity>(), row, rookOldX, board.GetWorldPos(row), board.GetWorldPos(rookNewX), rookIsWhite, false);
			}
		}

		board.SwapPlayer();
		ServerMoveFigure(identityChessman, identity, fromZ_Board, fromX_Board, toZ_World, toX_World, identityChessman.isWhite, isKing);
	}

	[Server]
	void ServerMoveFigure(Chessman chessman, NetworkIdentity networkIdentity, int fromZ_Board, int fromX_Board, int toZ_World, int toX_World, bool isWhite, bool isKing)
	{
		board.SetCell(fromZ_Board, fromX_Board, board.GetBoardPos(toZ_World), board.GetBoardPos(toX_World), isWhite, isKing);
		RpcMoveFigure(networkIdentity, fromZ_Board, fromX_Board, toZ_World, toX_World, isWhite, isKing);
		chessman.OnMove(toZ_World, toX_World);
	}

	[ClientRpc]
	void RpcMoveFigure(NetworkIdentity identity, int fromZ_Board, int fromX_Board, int toZ_World, int toX_World, bool isWhite, bool isKing)
	{
		board.SetCell(fromZ_Board, fromX_Board, board.GetBoardPos(toZ_World), board.GetBoardPos(toX_World), isWhite, isKing);
		identity.transform.position = new Vector3(toX_World, 0, toZ_World);
		identity.GetComponent<Chessman>().OnMove(toZ_World, toX_World);
	}
}
