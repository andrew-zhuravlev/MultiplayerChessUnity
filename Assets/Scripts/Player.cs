using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
	Chessman selectedChessman;
	NetworkIdentity selectedChessman_NetworkIdentity;

	Board board;

	void Start()
	{
		board = Board.Instance;

		//Doesn't matter for this particular game but will make board initialize if there is third player who connects.
		if (isLocalPlayer)
		{
			board.Init();

			if(!isServer)
				FindObjectOfType<PlayerCamera>().PointToBlack();
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
			MakeMove(selectedChessman_NetworkIdentity, (int)hit.transform.position.z, (int)hit.transform.position.x);
	}

	void MakeMove(NetworkIdentity identity, int z, int x)
	{
		CmdMoveFigure(identity, selectedChessman.Y, selectedChessman.X, z, x, selectedChessman.isWhite, (selectedChessman as King) != null);
		board.RemoveHighlighters();
	}

	[Command]
	void CmdMoveFigure(NetworkIdentity identity, int fromZ, int fromX, int z, int x, bool isWhite, bool isKing)
	{
		board.WhiteMoves = !board.WhiteMoves;
		RpcMoveFigure(identity, fromZ, fromX, z, x, isWhite, isKing);
	}

	[ClientRpc]
	void RpcMoveFigure(NetworkIdentity identity, int fromZ, int fromX, int z, int x, bool isWhite, bool isKing)
	{
		board.SetCell(fromZ, fromX, board.GetBoardPos(z), board.GetBoardPos(x), isWhite, isKing);

		identity.transform.position = new Vector3(x, 0, z);
		identity.GetComponent<Chessman>().OnMove(z, x);

		//Executes on every client. One extra (redundant) step. So that one of them is already null.
		selectedChessman = null;
		selectedChessman_NetworkIdentity = null;
	}
}
