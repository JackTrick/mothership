using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventUI : MonoBehaviour
{
	[SerializeField]
	private GameEventUI gameEventUI_;
	//private InventoryUI inventoryUI_;

	public EventUI()
	{
		
	}

	public void SelectEventClick()
	{
		GameController.Instance.ConfirmEventChoice ();
	}

	public void RenderEvent(GameEvent gameEvent)
	{
		gameEventUI_.Render (gameEvent);
		//gameEvent_ = gameEvent;
	}

	public void MakeEventChoice(Choice choice)
	{
		gameEventUI_.MakeEventChoice(choice);
	}
}


