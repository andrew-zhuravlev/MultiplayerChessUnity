using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
	Chessman selectedChessman;
	NetworkIdentity selectedChessman_NetworkIdentity;

	void Start()
	{
		if (!isLocalPlayer)
			return;

		Board.Instance.Init();
	}

	void Update()
	{
		if (!isLocalPlayer || isServer != Board.Instance.WhiteMoves)
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
				Board.Instance.RemoveHighlighters();
			}

			else if (isServer == chessmanComponent.isWhite)
			{
				Board.Instance.RemoveHighlighters();
				selectedChessman = chessmanComponent;
				selectedChessman_NetworkIdentity = hit.collider.GetComponent<NetworkIdentity>();

				Board.Instance.DisplayHighlighters(chessmanComponent.GetValidMoves());
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

	public void MakeMove(NetworkIdentity identity, int z, int x)
	{
		CmdMoveFigure(identity, selectedChessman.Y, selectedChessman.X, z, x, selectedChessman.isWhite, (selectedChessman as King) != null);
		Board.Instance.RemoveHighlighters();
	}

	[Command]
	public void CmdMoveFigure(NetworkIdentity identity, int fromZ, int fromX, int z, int x, bool isWhite, bool isKing)
	{
		RpcMoveFigure(identity, fromZ, fromX, z, x, isWhite, isKing);
	}

	[ClientRpc]
	public void RpcMoveFigure(NetworkIdentity identity, int fromZ, int fromX, int z, int x, bool isWhite, bool isKing)
	{
		Debug.Log("RpcMoveFigure()");

		Board.Instance.SetCell(fromZ, fromX, Board.Instance.GetBoardPos(z), Board.Instance.GetBoardPos(x), isWhite, isKing);
		Board.Instance.WhiteMoves = !Board.Instance.WhiteMoves;

		identity.transform.position = new Vector3(x, 0, z);
		identity.GetComponent<Chessman>().OnMove(z, x);

		selectedChessman = null;
		selectedChessman_NetworkIdentity = null;
	}
}
