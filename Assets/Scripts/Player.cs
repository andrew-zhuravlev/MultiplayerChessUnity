using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour 
{
	Chessman selectedChessman;
	NetworkIdentity selectedChessmanNetworkIdentity;

	void Update()
	{
		if (isServer != FindObjectOfType<Board>().WhiteMoves)
			return;

		if (selectedChessman == null)
			CheckSelection();

		else
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
				selectedChessmanNetworkIdentity = hit.collider.GetComponent<NetworkIdentity>();

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
			int x = (int)hit.transform.position.x, z = (int)hit.transform.position.z;

			FindObjectOfType<Board>().MakeMove(selectedChessman, selectedChessmanNetworkIdentity, z, x);

			selectedChessman = null;
		}
	}
}
