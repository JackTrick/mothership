﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartState : GameUIState {
	public const string STATE_NAME = "start";

	private StartUI startUI_;

	public StartState(): base(STATE_NAME, "UI/ui_start"){

	}

	// Use this for initialization
	void Start () {
		
	}

	
	override public void OnPush() {
		base.OnPush ();
	}

	override public void OnEnter() {
		base.OnEnter ();
		startUI_ = UI.GetComponent<StartUI> ();
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
