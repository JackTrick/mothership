using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadUI : MonoBehaviour
{
	[SerializeField]
	private Button saveButton_;

	[SerializeField]
	private Button loadButton_;

	[SerializeField]
	private GameObject savePrompt_;

	[SerializeField]
	private Button savePromptCancelButton_;

	[SerializeField]
	private Button savePromptSaveButton_;

	[SerializeField]
	private InputField savePromptInput_;

	[SerializeField]
	private Text savePromptHelpText_;

	[SerializeField]
	private GameObject loadPrompt_;

	[SerializeField]
	private GameObject loadPromptElements_;

	[SerializeField]
	private Button loadPromptCancelButton_;

	private GameObject loadElementPrefab_;

	public SaveLoadUI ()
	{
	}

	private void Awake()
	{
		savePrompt_.SetActive (false);
		loadPrompt_.SetActive (false);
		saveButton_.gameObject.SetActive (true);
		loadButton_.gameObject.SetActive (true);

		loadElementPrefab_ = Resources.Load<GameObject> ("Prefabs/LoadElement");
	}

	public void OpenSavePrompt()
	{
		savePrompt_.SetActive (true);
		savePromptInput_.text = "";
		savePromptHelpText_.color = new Color (0.2f, 0.2f, 0.2f);
		savePromptHelpText_.text = "enter the filename to save";
		saveButton_.gameObject.SetActive (false);
		loadButton_.gameObject.SetActive (false);
	}

	public void OpenLoadPrompt(){
		loadPrompt_.SetActive (true);
		saveButton_.gameObject.SetActive (false);
		loadButton_.gameObject.SetActive (false);

		string[] fileEntries = SaveManager.Instance.GetSaveFileStrings ();
		GameObject loadElement;
		Button loadElementButton;
		int pathIndex = SaveManager.Instance.GetPath ().Length;
		int extensionLength = SaveManager.Instance.GetExtension ().Length;

		foreach (string filename in fileEntries) {
			loadElement = GameObject.Instantiate<GameObject> (loadElementPrefab_, loadPromptElements_.transform);
			loadElement.GetComponentInChildren<Text> ().text = filename.Substring (pathIndex, filename.Length - pathIndex - extensionLength);
			loadElementButton = loadElement.GetComponentInChildren<Button> ();
			loadElementButton.onClick.AddListener (() => LoadButtonHandler (filename));
		}
	}

	public void LoadButtonHandler(string filename)
	{
		CloseLoadPrompt ();
		SaveManager.Instance.LoadGame (filename);
	}

	public void CloseSavePrompt()
	{
		savePrompt_.SetActive (false);
		saveButton_.gameObject.SetActive (true);
		loadButton_.gameObject.SetActive (true);
	}

	public void CloseLoadPrompt()
	{
		loadPrompt_.SetActive (false);
		saveButton_.gameObject.SetActive (true);
		loadButton_.gameObject.SetActive (true);

		foreach (Transform child in loadPromptElements_.transform) {
			child.gameObject.GetComponentInChildren<Button> ().onClick.RemoveAllListeners ();
			GameObject.Destroy (child.gameObject);
		}
	}

	public void TryToSave()
	{
		Debug.Log ("trying to save");
		if (savePromptInput_.text == "") {
			savePromptHelpText_.color = new Color (1f, 0.2f, 0.2f);
			savePromptHelpText_.text = "filename required to save";
		} else {
			string ret = SaveManager.Instance.TryToSave (savePromptInput_.text);
			if (ret == "") {
				CloseSavePrompt ();
			} else {
				savePromptHelpText_.color = new Color (1f, 0.2f, 0.2f);
				savePromptHelpText_.text = "error occurred while saving";
			}
		}
	}
}


