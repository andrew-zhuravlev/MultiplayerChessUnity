using UnityEngine;
using UnityEngine.Networking;

public class Server : NetworkBehaviour 
{
	public static Server Instance;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

		else
			Destroy(gameObject);
	}

	public override void OnStartServer()
	{
		print("OnStartServer");
		Board.Instance.Init();
	}

	public bool WhiteMoves = true;

	[ClientRpc]
	public void RpcMoveFigure(NetworkIdentity identity, int x, int z)
	{
		identity.transform.position = new Vector3(x, 0, z);
		WhiteMoves = !WhiteMoves;
	}
}
