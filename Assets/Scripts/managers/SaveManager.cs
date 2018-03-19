using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
	private static SaveManager instance_;
	public static SaveManager Instance { get { return instance_; } }

	private const string PATH = "Assets/Resources/Saves/";
	private const string EXTENSION = ".txt";

	[SerializeField]
	private string defaultFileToLoad_ = "";

	public SaveManager ()
	{
	}

	private void Awake()
	{
		if (instance_ != null && instance_ != this)
		{
			Destroy(this.gameObject);
		} else {
			instance_ = this;
		}
	}

	public void LoadDefaultFile()
	{
		if (defaultFileToLoad_ != "") {
			LoadGame (PATH + defaultFileToLoad_ + EXTENSION);
		}
	}

	public void TestSave()
	{
		string path = "Assets/Resources/Saves/test.txt";
		/*
		if (System.IO.File.Exists (path)) 
		{
			var textFile = File.ReadAllText("C:/Hello.txt");
			File.WriteAllText("C:/Hello.txt", textFile + "Clicked the second button");
		}
		else
			File.WriteAllText ("C:/Hello.txt", "Clicked the second button");
		*/
		if (File.Exists (path)) {
			File.Delete (path);
		}
		FileStream file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);

		StreamWriter writer = new StreamWriter(file);
		//StreamReader reader = new StreamReader(file);

		//Write some text to the test.txt file
		//StreamWriter writer = new StreamWriter(path, true);
		writer.WriteLine("Test 23");
		writer.Close();
	}

	public string[] GetSaveFileStrings()
	{
		return Directory.GetFiles (PATH, "*"+EXTENSION);
	}

	public string GetPath()
	{
		return PATH;
	}

	public string GetExtension()
	{
		return EXTENSION;
	}

		
	public string TryToSave(string filename){
		Debug.Log ("Trying to save game: " + filename);
		string path = PATH + filename + EXTENSION;

		if (File.Exists (path)) {			
			File.Delete (path);
		}
		FileStream file = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
		StreamWriter writer = new StreamWriter(file);

		GameEventManager.Instance.SaveProgress(writer);
		ItemManager.Instance.SaveProgress(writer);

		writer.Close();
		return "";
	}

	public bool LoadGame(string filename)
	{
		Debug.Log ("Loading game: " + filename);
		if (!File.Exists (filename)) {			
			Debug.LogError ("File to load not found: " + filename);
			return false;
		}
		FileStream file = File.Open(filename, FileMode.Open, FileAccess.Read);
		StreamReader reader = new StreamReader (file);

		string line = "";

		bool readingFlags = false;
		bool readingNames = false;
		bool readingCompletedEvents = false;
		bool readingNeverSpawnEvents = false;
		bool readingInventory = false;

		GameEventManager.Instance.ResetSave ();
		ItemManager.Instance.ResetSave ();

		// TODO, this is the WORST. Make it better.
		while((line = reader.ReadLine()) != null)  
		{  
			if (line.Equals ("[flags]")) {
				readingFlags = true;
				readingNames = false;
				readingCompletedEvents = false;
				readingNeverSpawnEvents = false;
				readingInventory = false;
			} else if (line.Equals ("[namebank]")) {
				readingFlags = false;
				readingNames = true;
				readingCompletedEvents = false;
				readingNeverSpawnEvents = false;
				readingInventory = false;
			} else if (line.Equals ("[completedEventIDs]")) {
				readingFlags = false;
				readingNames = false;
				readingCompletedEvents = true;
				readingNeverSpawnEvents = false;
				readingInventory = false;
			} else if (line.Equals ("[neverSpawnEventIDs]")) {
				readingFlags = false;
				readingNames = false;
				readingCompletedEvents = false;
				readingNeverSpawnEvents = true;
				readingInventory = false;
			} else if (line.Equals ("[inventory]")) {
				readingFlags = false;
				readingNames = false;
				readingCompletedEvents = false;
				readingNeverSpawnEvents = false;
				readingInventory = true;
			} else {
				if (readingFlags) {
					GameEventManager.Instance.SetFlag (line);
				} 
				else if (readingNames) {
					string name = reader.ReadLine ();
					GameEventManager.Instance.SetName (line, name);
				}
				else if(readingCompletedEvents){
					GameEventManager.Instance.SetEventIDCompleted (line);
				}
				else if(readingNeverSpawnEvents){
					GameEventManager.Instance.NeverSpawnEventID (line);
				}
				else if(readingInventory){
					Item temp = new Item (line);
					string itemType = line;
					int amount = int.Parse(reader.ReadLine());
					bool defined = reader.ReadLine() == "True";
					int value = int.Parse(reader.ReadLine());
					IntNull cap = new IntNull(value, defined);
					defined = reader.ReadLine() == "True";
					value = int.Parse(reader.ReadLine());
					IntNull producePer = new IntNull(value, defined);
					defined = reader.ReadLine() == "True";
					value = int.Parse(reader.ReadLine());
					IntNull turnsToProduce = new IntNull(value, defined);
					int turnCounter = int.Parse(reader.ReadLine());

					temp.Amount = amount;
					temp.Cap = cap;
					temp.ProducePer = producePer;
					temp.TurnsToProduce = turnsToProduce;
					temp.TurnCounter = turnCounter;

					ItemManager.Instance.SetInventoryItem (itemType, temp);
				}
			}


		}  
		reader.Close ();
		GameController.Instance.GameLoaded ();

		return true;
	}
}

