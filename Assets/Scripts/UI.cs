using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
	[SerializeField] GameObject backgroundPanel;

	public void DisplayEndGameUI(GameEnd end)
	{
		backgroundPanel.SetActive(true);
		Text endGameText = backgroundPanel.transform.GetChild(0).GetChild(0).GetComponent<Text>();

		if (end == GameEnd.Draw)
			endGameText.text = "Draw!";

		bool server = FindObjectsOfType<Player>().First(player => player.isLocalPlayer).isServer;

		GameEnd winCase = server ? GameEnd.ServerWin : GameEnd.ClientWin;

		if (winCase == end)
			endGameText.text = "YOU WON!";
		else
			endGameText.text = "YOU LOST!";
	}
}
