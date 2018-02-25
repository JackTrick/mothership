using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipStatusState : GameUIState {
	public const string STATE_NAME = "ship_status";

	private ShipStatusUI statusUI_;

	public ShipStatusState(): base(STATE_NAME, "UI/ui_shipstatus"){

	}

	// Use this for initialization
	void Start () {

	}

	override public void OnPush() {
		base.OnPush ();
	}

	override public void OnEnter() {
		base.OnEnter ();
		statusUI_ = UI.GetComponent<ShipStatusUI> ();
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
