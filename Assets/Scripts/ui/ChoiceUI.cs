using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceUI : MonoBehaviour
{
	private GameEvent gameEvent_;

	private Choice choice_;

	[SerializeField]
	private Button choiceButton_;

	[SerializeField]
	private Text choiceButtonText_;

	[SerializeField]
	private Text desc_;

	[SerializeField]
	private Text effects_;

	public ChoiceUI()
	{
	}

	public void Destroy()
	{
		choiceButton_.onClick.RemoveAllListeners ();
	}

	public void Render(Choice choice)
	{
		// TODO, don't show the choice if triggers haven't been set
		// TODO, show the choice as disabled if we can't afford the cost
		choice_ = choice;
		choiceButtonText_.text = choice.Name;
		desc_.text = choice.Desc;
		effects_.text = choice.GetEffectsString();

		if (choice.CanAffordChoice ()) {
			choiceButton_.interactable = true;
			choiceButton_.onClick.AddListener (ChooseSelected);
		} else {
			choiceButton_.interactable = false;
		}
	}

	public void ChooseSelected()
	{
		Debug.Log ("Chose: " + choice_.Name);
		//choiceButton_.gameObject.SetActive (false);
		choiceButton_.interactable = false;
		GameController.Instance.MakeEventChoice (choice_);
	}

	public bool IsChoice(Choice choice)
	{
		return choice == choice_;
	}
}

