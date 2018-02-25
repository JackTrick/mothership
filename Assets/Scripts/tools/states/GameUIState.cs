using System;
using UnityEngine;

public class GameUIState : GameState
{
	private string prefabName_;

	private GameObject ui_ = null;
	public GameObject UI { get { return ui_; } }

	public GameUIState (string name, string prefabName) :base(name)
	{
		prefabName_ = prefabName;
	}

	private void LoadUI()
	{
		if (ui_ == null) {
			ui_ = GameObject.Instantiate (Resources.Load (prefabName_) as GameObject, UIManager.Instance.transform);
		}
	}

	private void ClearUI()
	{
		if(ui_ != null){
			GameObject.Destroy (ui_);
		}
	}

	override public void OnEnter() {
		base.OnEnter ();
		LoadUI();
	}

	override public void OnExit() {
		base.OnExit ();
		ClearUI();
	}
}


