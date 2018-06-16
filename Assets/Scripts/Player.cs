using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
	Chessman selectedChessman;
	NetworkIdentity selectedChessman_NetworkIdentity;

	Board b;

	void Start()
	{
		if (!isLocalPlayer)
			return;

		Debug.Log("Start();");
		b = FindObjectOfType<Board>();
		b.Init();
	}

	void Update()
	{
		if (!isLocalPlayer)
			return;

		//isServer should definitely be fixed.
		if (!isLocalPlayer || isServer != b.WhiteMoves)
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
				FindObjectOfType<Board>().RemoveHighlighters();
			}

			else if (isServer == chessmanComponent.isWhite)
			{
				FindObjectOfType<Board>().RemoveHighlighters();
				selectedChessman = chessmanComponent;
				selectedChessman_NetworkIdentity = hit.collider.GetComponent<NetworkIdentity>();

				FindObjectOfType<Board>().DisplayHighlighters(chessmanComponent.GetValidMoves());
			}
		}
	}

	void CheckMovement()
	{
		if (!Input.GetMouseButtonDown(0))
			return;

		RaycastHit hit;
		if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, LayerMask.GetMask("Highlighters")))
		{
			MakeMove(selectedChessman_NetworkIdentity, (int)hit.transform.position.z, (int)hit.transform.position.x);
		}
	}

	//TODO: Fix problem: x and z are swapped.
	public void MakeMove(NetworkIdentity identity, int z, int x)
	{
		CmdMoveFigure(identity, selectedChessman.Y, selectedChessman.X, z, x, selectedChessman.isWhite, (selectedChessman as King) != null);
		FindObjectOfType<Board>().RemoveHighlighters();
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

		FindObjectOfType<Board>().SetCell(fromZ, fromX, FindObjectOfType<Board>().GetBoardPos(z), FindObjectOfType<Board>().GetBoardPos(x), isWhite, isKing);

		//Definitely change this;
		identity.GetComponent<Chessman>().OnMove(z, x);

		FindObjectOfType<Board>().WhiteMoves = !FindObjectOfType<Board>().WhiteMoves;

		identity.transform.position = new Vector3(x, 0, z);

		selectedChessman = null;
		selectedChessman_NetworkIdentity = null;
	}
}
