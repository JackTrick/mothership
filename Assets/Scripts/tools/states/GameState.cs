using System;
using UnityEngine;

public class GameState {
	// states have a string name for ease of debugging
	private string name_;
	public string name { get { return name_; } }

	public GameState(string name) {
		name_ = name;
	}

	// ===========================

	// you should generally override the following

	virtual public void OnPush() {
		#if LOG_STATE_CHANGES
		Debug.Log ("OnPush: " + name);
		#endif
	}

	virtual public void OnEnter() {
		#if LOG_STATE_CHANGES
		Debug.Log ("OnEnter: " + name);
		#endif
	}

	virtual public void OnExit() {
		#if LOG_STATE_CHANGES
		Debug.Log ("OnExit: " + name);
		#endif
	}

	virtual public void OnPop() {
		#if LOG_STATE_CHANGES
		Debug.Log ("OnPop: " + name);
		#endif
	}

	virtual public void Update(float delta) {
		// ... stub
	}

	virtual public void DebugSolve() {
		// ... stub
	}

	//if considerRaycast is set to true, clicks will not register if another object takes it
	virtual public void OnScreenClick(bool considerRaycast) {
		// ... stub
	}

	// ===========================
}
