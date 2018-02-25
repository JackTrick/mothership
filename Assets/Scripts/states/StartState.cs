using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartState : GameState {
	public const string STATE_NAME = "start";

	public StartState(): base(STATE_NAME){

	}

	// Use this for initialization
	void Start () {
		
	}
	
	override public void OnPush() {
		base.OnPush ();
	}

	override public void OnEnter() {
		base.OnEnter ();
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
