using UnityEngine;

public class AddQueenToListHandler : MonoBehaviour 
{
	[Tooltip("When object is created should it be added to the list of chessmen in Board.")]
	[SerializeField] bool addToListOnStartup = false;

	void Start()
	{
		if (addToListOnStartup)
		{
			Queen queenComponent = this.GetComponent<Queen>();
			Board.Instance.AddQueenToList(queenComponent, queenComponent.isWhite);
		}
	}
}
