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
		CheckAction();
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

	void CheckAction()
	{
		if (!Input.GetMouseButtonDown(0))
			return;

		RaycastHit hit;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Highlighters", "Chessmen")))
		{
			// This also could be a chessman, so it fixes the bug.
			if (hit.collider.CompareTag("Move Highlighter"))
				MakeMove((int)hit.transform.position.z, (int)hit.transform.position.x);
		}
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

		List<Move> allValidMoves = identityChessman.GetValidMoves();
		int validMoveIndex = allValidMoves.FindIndex(move => move.x == boardToX && move.z == boardToZ);

		if (validMoveIndex == -1)
		{
			Debug.LogError("NOT LEGIT MOVE");
			return;
		}

		King kingComponent = identityChessman as King;
		bool isKing = kingComponent != null;
		bool pawnGotToEnd = false;

		if (identityChessman is Pawn)
		{
			pawnGotToEnd = identityChessman.isWhite ? board.GetBoardPos(toZ_World) == 7 : board.GetBoardPos(toZ_World) == 0;
			if (pawnGotToEnd)
			{
				GameObject queen = Instantiate(identityChessman.isWhite ? board.WhiteQueenPrefab : board.BlackQueenPrefab,
					new Vector3(toX_World, 0, toZ_World), Quaternion.identity);
				Queen queen_QueenComponent = queen.GetComponent<Queen>();

				NetworkServer.Spawn(queen);

				board.AddQueenToList(queen_QueenComponent, identityChessman.isWhite);
				RpcAddQueenToList(queen.GetComponent<NetworkIdentity>(), identityChessman.isWhite);

				NetworkIdentity pawnToKill = board.GetComponentInChessman<NetworkIdentity>(fromZ_Board, fromX_Board);
				ServerKillFigure(pawnToKill);

				board.SetCell(fromZ_Board, fromX_Board, board.GetBoardPos(toZ_World), board.GetBoardPos(toX_World), queen_QueenComponent.isWhite, false);
				RpcSetCell(fromZ_Board, fromX_Board, board.GetBoardPos(toZ_World), board.GetBoardPos(toX_World), queen_QueenComponent.isWhite, false);
			}
		}

		if (allValidMoves[validMoveIndex].isKill)
		{
			NetworkIdentity toKill = board.GetComponentInChessman<NetworkIdentity>(allValidMoves[validMoveIndex].z, allValidMoves[validMoveIndex].x);
			ServerKillFigure(toKill);
		}

		else if (allValidMoves[validMoveIndex].isCastle)
		{
			int xDelta = board.GetBoardPos(toX_World) - kingComponent.X_Board;
			int row = kingComponent.isWhite ? 0 : 7;

			int rookOldX = xDelta == -3 ? 0 : 7;
			int rookNewX = xDelta == -3 ? 2 : 5;

			Chessman rook = board.GetComponentInChessman<Chessman>(row, rookOldX);
			ServerMoveFigure(rook, rook.GetComponent<NetworkIdentity>(), row, rookOldX, board.GetWorldPos(row), board.GetWorldPos(rookNewX), row == 0, false);
		}

		board.SwapPlayer();

		if (!pawnGotToEnd)
			ServerMoveFigure(identityChessman, identity, fromZ_Board, fromX_Board, toZ_World, toX_World, identityChessman.isWhite, isKing);
	}

	[Server]
	void ServerKillFigure(NetworkIdentity toKill)
	{
		Chessman toKill_Chessman = toKill.GetComponent<Chessman>();

		int z_Board = toKill_Chessman.Y_Board;
		int x_Board = toKill_Chessman.X_Board;
		bool isWhite = toKill_Chessman.isWhite;

		board.RemoveChessman(toKill_Chessman);

		RpcRemoveChessmanFromBoard(isWhite, z_Board, x_Board);

		NetworkServer.Destroy(NetworkServer.FindLocalObject(toKill.netId));
	}

	[Server]
	void ServerMoveFigure(Chessman chessman, NetworkIdentity networkIdentity, int fromZ_Board, int fromX_Board, int toZ_World, int toX_World, bool isWhite, bool isKing)
	{
		board.SetCell(fromZ_Board, fromX_Board, board.GetBoardPos(toZ_World), board.GetBoardPos(toX_World), isWhite, isKing);
		RpcMoveFigure(networkIdentity, fromZ_Board, fromX_Board, toZ_World, toX_World, isWhite, isKing);
		chessman.OnMove(toZ_World, toX_World);
	}

	[ClientRpc]
	void RpcAddQueenToList(NetworkIdentity queen, bool isWhite)
	{
		// TODO: Check if data is valid before doing this.
		board.AddQueenToList(queen.GetComponent<Queen>(), isWhite);
	}

	[ClientRpc]
	void RpcRemoveChessmanFromBoard(bool isWhite, int z_Board, int x_Board)
	{
		if(!isServer)
			board.RemoveChessman((isWhite ? board.WhiteChessmen : board.BlackChessmen).First(chessman => chessman.Y_Board == z_Board && chessman.X_Board == x_Board));
	}

	[ClientRpc]
	void RpcMoveFigure(NetworkIdentity identity, int fromZ_Board, int fromX_Board, int toZ_World, int toX_World, bool isWhite, bool isKing)
	{
		board.SetCell(fromZ_Board, fromX_Board, board.GetBoardPos(toZ_World), board.GetBoardPos(toX_World), isWhite, isKing);
		identity.transform.position = new Vector3(toX_World, 0, toZ_World);
		identity.GetComponent<Chessman>().OnMove(toZ_World, toX_World);
	}

	[ClientRpc]
	void RpcSetCell(int fromZ_Board, int fromX_Board, int toZ_Board, int toX_Board, bool isWhite, bool isKing)
	{
		board.SetCell(fromZ_Board, fromX_Board, toZ_Board, toX_Board, isWhite, isKing);
	}
}
