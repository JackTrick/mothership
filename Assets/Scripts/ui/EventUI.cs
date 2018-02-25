using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventUI : MonoBehaviour
{
	public EventUI()
	{
	}

	public void SelectEventClick()
	{
		GameController.Instance.ConfirmEventChoice ();
	}
}


