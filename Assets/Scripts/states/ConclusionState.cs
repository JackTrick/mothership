using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConclusionState : GameUIState {
	public const string STATE_NAME = "conclusion";

	private ConclusionUI conclusionUI_;

	public ConclusionState(): base(STATE_NAME, "UI/ui_conclusion"){

	}

	// Use this for initialization
	void Start () {

	}

	override public void OnPush() {
		base.OnPush ();
	}

	override public void OnEnter() {
		base.OnEnter ();
		conclusionUI_ = UI.GetComponent<ConclusionUI> ();
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
