using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUI : MonoBehaviour
{
	public StartUI()
	{
	}

	public void StartGameClick()
	{
		GameController.Instance.StartGame();
	}
}


