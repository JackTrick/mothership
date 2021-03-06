﻿using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class StateMachine {
	private Stack<GameState> stateStack_ = new Stack<GameState>();

	private Action<GameState> onStateChange_;
	public Action<GameState> onStateChange { set { onStateChange_ = value; } }

	public List<string> states;

	private bool paused_;

	public StateMachine()
	{
		states = new List<string>();
	}

	// get the top state, ie. current
	public GameState currentState {
		get {
			return ((stateStack_.Count > 0) ? stateStack_.Peek() : null);
		}
	}

	public bool InState(string name) {
		return currentState != null && currentState.name == name;
	}

	private void StateChanged() {
		GameState current = currentState;
		if (onStateChange_ != null && current != null) {
			onStateChange_(current);
		}
	}

	public void PushState(GameState state) {
		// exit current state
		if (stateStack_.Count > 0) {
			stateStack_.Peek().OnExit();
		}
		states.Add(state.name);
		// push new state
		state.OnPush();
		stateStack_.Push(state);
		// enter new state
		state.OnEnter();

		StateChanged();
	}

	// the name is only to sanity check, making sure 
	// you're taking down what you think you are...
	public void PopState(string stateName) {
		GameState top = stateStack_.Peek();
		if (top != null && top.name == stateName) {
			// exit top state
			top.OnExit();
			// pop top state
			top.OnPop();
			states.RemoveAt(states.Count - 1);
			stateStack_.Pop();
			// if there was a state below, enter it again
			if (stateStack_.Count > 0) {
				stateStack_.Peek().OnEnter();
			}
		} else {
			Debug.LogError("Unable to pop state named: " + stateName + " top: " + (top != null ? top.name : "null"));
		}

		StateChanged();
	}

	// hacky convenience function to make sure a state gets popped from the stack by 
	// popping any states on top of it as well
	public void ForcePopState(string stateName) {
		// safety check to make sure state is on the stack
		List<GameState> statesFound = stateStack_.Where(state => state.name == stateName).ToList();
		if (statesFound.Count == 0) {
			return;
		}

		// pop any states on top of the one we want to pop
		while (currentState != null && currentState.name != stateName) {
			PopState(currentState.name);
		}

		// pop the state we want to pop
		if (currentState != null) {
			PopState(currentState.name);
		}
	}

	// convenience function for popping all states off the stack to reset
	public void PopAll() {
		while (currentState != null) {
			PopState(currentState.name);
		}
	}

	public void Pause() {
		paused_ = true;
	}

	public void Unpause() {
		paused_ = false;
	}

	private void Start() {
	}

	public void Update() {
		if (paused_) {
			return;
		}
		float dt = UnityEngine.Time.smoothDeltaTime;
		if (stateStack_.Count > 0) {
			stateStack_.Peek().Update(dt);
		}
	}

	public override string ToString() {
		return string.Join(", ", states.ToArray());
	}
}
