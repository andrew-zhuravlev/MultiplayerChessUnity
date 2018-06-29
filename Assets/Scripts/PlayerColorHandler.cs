using UnityEngine;
using UnityEngine.Networking;

// Defines if player should be black or white.
public class PlayerColorHandler : NetworkBehaviour 
{
	#region Singleton
	public static PlayerColorHandler Instance { get; private set; }
	void Awake()
	{
		Instance = this;
	}
	void OnDestroy()
	{
		Instance = null;
	}
	#endregion

	[SyncVar]
	bool _serverPlaysAsWhite;

	public bool ServerPlaysAsWhite
	{
		get { return _serverPlaysAsWhite; }
	}

	void Start()
	{
		if (!isServer)
			return;

		_serverPlaysAsWhite = Random.Range(0, 2) == 0;
	}
}
