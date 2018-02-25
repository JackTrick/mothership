using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreationState : GameUIState {
	public const string STATE_NAME = "creation";

	private CreationUI creationUI_;

	public CreationState(): base(STATE_NAME, "UI/ui_creation"){

	}

	// Use this for initialization
	void Start () {

	}

	override public void OnPush() {
		base.OnPush ();
	}

	override public void OnEnter() {
		base.OnEnter ();
		creationUI_ = UI.GetComponent<CreationUI> ();
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
