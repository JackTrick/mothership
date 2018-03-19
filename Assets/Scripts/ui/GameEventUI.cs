using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEventUI : MonoBehaviour
{
	private GameEvent gameEvent_;

	[SerializeField]
	private Text title_;

	[SerializeField]
	private Text desc_;

	[SerializeField]
	private Text resultTitle_;

	[SerializeField]
	private Text resultDesc_;

	[SerializeField]
	private Text resultOtherDesc_;

	[SerializeField]
	private Button resultConfirmButton_;

	[SerializeField]
	private Image image_;

	[SerializeField]
	private List<ChoiceUI> choiceUIs_;

	[SerializeField]
	private GameObject choices_;

	[SerializeField]
	private GameObject resultUI_;

	private bool gameOver_ = false;

	public GameEventUI ()
	{
	}


	private void Awake()
	{
		resultUI_.SetActive (false);
		resultConfirmButton_.onClick.AddListener (ClickResultContinue);
	}

	public void Destroy(){
		resultConfirmButton_.onClick.RemoveAllListeners ();
	}

	public void ClickResultContinue()
	{
		// TODO: end of event stuff, sleep_for, etc
		GameController.Instance.ConfirmEventChoice ();
	}

	public void Render(GameEvent gameEvent)
	{
		resultUI_.SetActive (false);
		foreach (Transform child in choices_.transform) {
			GameObject.Destroy (child.gameObject);
		}

		gameEvent_ = gameEvent;
		title_.text = gameEvent.Name;
		desc_.text = gameEvent.Desc;
		desc_.text = desc_.text.Replace ("\\n", "\n");
		if(gameEvent.ImageName != null){
			Sprite eventImage = Resources.Load<Sprite>("Sprites/Events/"+gameEvent.ImageName);
			image_.sprite = eventImage;
		}

		foreach (Choice c in gameEvent_.Choices) {
			if (c.CanShowChoice ()) {
				ChoiceUI choiceUI = GameObject.Instantiate (Resources.Load<ChoiceUI> ("Prefabs/ChoiceUI"), choices_.transform);
				choiceUI.Render (c);
			}
		}
		//title_ = gameEvent
	}

	public void MakeEventChoice(Choice choice, bool gameOver)
	{
		GameEventManager.Instance.SetEventCompleted (gameEvent_);
		gameOver_ = gameOver;

		//choices_.transform.DetachChildren ();
		foreach(ChoiceUI choiceUI in choices_.GetComponentsInChildren<ChoiceUI>())
		{
			if (!choiceUI.IsChoice (choice)) {
				GameObject.Destroy (choiceUI.gameObject);
			}

		}
		resultUI_.SetActive (true);

		if (choice.LastChallengeResultString != "") {
			resultTitle_.gameObject.SetActive (true);
			resultTitle_.text = choice.LastChallengeResultString;
		} else {
			resultTitle_.gameObject.SetActive (false);
		}
		Result result = choice.LastResult;
		resultDesc_.text = result.Desc;

		string otherText = "";
		string lastString = "";
		List<ResultEffect> effects = result.Effects;
		if (effects != null) {
			for (int i = 0; i < effects.Count; ++i) {
				ResultEffect effect = effects [i];

				if (lastString != "") {
					otherText += "\n";
				}
				lastString = effect.ReadableString ();
				otherText += lastString;
			}
		}
		resultOtherDesc_.text = otherText;
	}
}

