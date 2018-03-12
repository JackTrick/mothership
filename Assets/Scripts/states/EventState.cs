using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventState : GameUIState {
	public const string STATE_NAME = "event";

	private EventUI eventUI_;

	public EventState(): base(STATE_NAME, "UI/ui_event"){

	}

	// Use this for initialization
	void Start () {

	}

	override public void OnPush() {
		base.OnPush ();
	}

	override public void OnEnter() {
		base.OnEnter ();
		eventUI_ = UI.GetComponent<EventUI> ();

		GameEvent gameEvent = GameEventManager.Instance.SpawnEvent();
		eventUI_.RenderEvent (gameEvent);
	}

	override public void OnExit() {
		base.OnExit ();
	}

	override public void OnPop() {
		base.OnPop ();
	}

	override public void Update(float delta) {
		//Debug.Log ("Update in Start State");
	}
}
