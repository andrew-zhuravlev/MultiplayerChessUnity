using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour 
{
	Chessman selectedChessman;
	NetworkIdentity selectedChessmanNetworkIdentity;

	bool isWhite;
	
	void Start()
	{
		isWhite = isServer ? true : false;

		print(isWhite);
	}

	void Update()
	{
		if (isWhite != Server.Instance.WhiteMoves)
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

			else if (isWhite == chessmanComponent.isWhite)
			{
				Board.Instance.RemoveHighlighters();
				selectedChessman = chessmanComponent;
				selectedChessmanNetworkIdentity = hit.collider.GetComponent<NetworkIdentity>();

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
		{
			int x = (int)hit.transform.position.x, z = (int)hit.transform.position.z;

			Board.Instance.MakeMove(selectedChessman, selectedChessmanNetworkIdentity, z, x);

			selectedChessman = null;
		}
	}
}
