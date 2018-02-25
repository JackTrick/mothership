using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConclusionUI : MonoBehaviour
{
	public ConclusionUI()
	{
	}

	public void RestartGameClick()
	{
		GameController.Instance.ReturnToStart ();
	}
}


