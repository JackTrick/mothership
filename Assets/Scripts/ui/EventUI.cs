using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventUI : MonoBehaviour
{
	private GameEvent gameEvent_;

	public EventUI()
	{
		
	}

	public void SelectEventClick()
	{
		GameController.Instance.ConfirmEventChoice ();
	}

	public void RenderEvent(GameEvent gameEvent)
	{
		gameEvent_ = gameEvent;
	}
}


