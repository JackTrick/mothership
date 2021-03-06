﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class GameEventManager : MonoBehaviour
{
	private static GameEventManager instance_;
	public static GameEventManager Instance { get { return instance_; } }

	[SerializeField]
	private string eventXMLFile_;

	private List<GameEvent> events_;

	// to save
	private Dictionary<string, bool> flags_;
	private Dictionary<string, bool> completedEventIDs_;
	private Dictionary<string, bool> neverSpawnEventIDs_;
	private Dictionary<string, int> sleepingEventIDs_;
	private Dictionary<string, int> eventWeights_;
	private Dictionary<string, string> nameBank_;
	// TODO: add logic for the event weights, and probably need to save it too.
	// save event weights, and when we spawn refer to that dictionary, not the data on the event itself
	// increment those weights accordingly as times goes on

	private int numTurns_;
	private string nextEvent_ = null;

	public GameEventManager()
	{
		events_ = new List<GameEvent> ();
		flags_ = new Dictionary<string, bool>();
		nameBank_ = new Dictionary<string, string>();
		completedEventIDs_ = new Dictionary<string, bool>();
		neverSpawnEventIDs_ = new Dictionary<string, bool>();
		sleepingEventIDs_ = new Dictionary<string, int>();
		eventWeights_ = new Dictionary<string, int>();
		numTurns_ = 0;
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

	public void ResetSave()
	{
		flags_ = new Dictionary<string, bool>();
		nameBank_ = new Dictionary<string, string>();
		completedEventIDs_ = new Dictionary<string, bool>();
		neverSpawnEventIDs_ = new Dictionary<string, bool>();
		sleepingEventIDs_ = new Dictionary<string, int>();
		eventWeights_ = new Dictionary<string, int>();
		numTurns_ = 0;
	}

	public bool SaveProgress(StreamWriter writer)
	{
		writer.WriteLine ("[numturns]");
		writer.WriteLine (numTurns_);
		writer.WriteLine ("[flags]");
		foreach (KeyValuePair<string, bool> entry in flags_) {
			writer.WriteLine (entry.Key);
		}
		writer.WriteLine ("[namebank]");
		foreach (KeyValuePair<string, string> entry in nameBank_) {
			writer.WriteLine (entry.Key);
			writer.WriteLine (entry.Value);
		}
		writer.WriteLine ("[completedEventIDs]");
		foreach (KeyValuePair<string, bool> entry in completedEventIDs_) {
			writer.WriteLine (entry.Key);
		}
		writer.WriteLine ("[neverSpawnEventIDs]");
		foreach (KeyValuePair<string, bool> entry in neverSpawnEventIDs_) {
			writer.WriteLine (entry.Key);
		}
		writer.WriteLine ("[sleepingEventIDs]");
		foreach (KeyValuePair<string, int> entry in sleepingEventIDs_) {
			writer.WriteLine (entry.Key);
			writer.WriteLine (entry.Value);
		}
		writer.WriteLine ("[eventWeights]");
		foreach (KeyValuePair<string, int> entry in eventWeights_) {
			writer.WriteLine (entry.Key);
			writer.WriteLine (entry.Value);
		}
		return true;
	}

	public bool TriggersTriggered(List<Trigger> triggers)
	{
		if (triggers == null)
			return true;
		
		bool valid = true;
		foreach (Trigger trigger in triggers) {
			//Debug.LogWarning (trigger.Logic);
			switch (trigger.Logic) {
			case Trigger.TriggerLogic.AND:
				valid = valid && trigger.Triggered ();
				break;
			case Trigger.TriggerLogic.OR:
				valid = valid || trigger.Triggered ();
				break;
			case Trigger.TriggerLogic.NOT:
			case Trigger.TriggerLogic.NOTAND:
				valid = valid && !trigger.Triggered ();
				break;
			case Trigger.TriggerLogic.NOTOR:
				valid = valid || !trigger.Triggered ();
				break;
			}
			if (valid) {
				//Debug.LogWarning ("valid!");
			}
		}
		if (valid) {
			//Debug.LogError ("CAN SPAWN!");
		} else {
			//Debug.LogError ("CAN'T SPAWN");
		}
		return valid;
	}

	public void Reset()
	{
		flags_.Clear ();
		nameBank_.Clear ();
		completedEventIDs_.Clear ();
		neverSpawnEventIDs_.Clear ();
		sleepingEventIDs_.Clear ();
		eventWeights_.Clear ();
		numTurns_ = 0;

		// TODO, reset events rather than clearing and reloading them
		events_.Clear ();
		LoadEvents ();
	}

	public void DoEventNext(string id)
	{
		nextEvent_ = id;
	}

	public void LoadEvents ()
	{
		Debug.Log ("Loading events");
		TextAsset textAsset = (TextAsset)Resources.Load (eventXMLFile_);
		XmlDocument xmlDoc = new XmlDocument (); // xmlDoc is the new xml document.
		//xmlDoc.Load (eventXMLFile_);
		xmlDoc.LoadXml(textAsset.text);
		XmlNodeList eventList = xmlDoc.GetElementsByTagName ("event"); // array of the level nodes. 

		foreach (XmlNode eventInfo in eventList) {
			GameEvent gameEvent = new GameEvent(eventInfo);
			events_.Add(gameEvent);
		}
		Debug.Log ("Events loaded");
	}

	public GameEvent SpawnEvent()
	{
		GameEvent gameEvent;
		if (nextEvent_ == null) {
			//Debug.LogWarning ("~~~~~ WEIGHTED POOL");
			WeightedPool<GameEvent> pool = GetWeightedPool (true);

			if (pool.Count == 0) {
				Debug.Log ("Found no mandatory events. Getting non-mandatory");
				pool = GetWeightedPool (false);
			}

			if (pool.Count == 0) {
				// TODO, end the game if this happens
				Debug.LogError ("Tried to spawn an event, but found none were eligible.");
			}

			gameEvent = pool.GetRandomItem ();
		} else {
			gameEvent = GetEvent(nextEvent_);
			nextEvent_ = null;
		}
		Debug.Log ("Spawning event: " + gameEvent);
		return gameEvent;
	}

	public GameEvent GetEvent(string id){
		GameEvent ret = null;
		for (int i = 0; i < events_.Count; ++i) {
			ret = events_ [i];
			if (ret.Id == id) {
				return ret;
			}
		}
		return ret;
	}

	// put together a pool of eligible events, checking if they're mandatory or not.
	public WeightedPool<GameEvent> GetWeightedPool(bool mandatory)
	{
		WeightedPool<GameEvent> pool = new WeightedPool<GameEvent> ();

		GameEvent gameEvent;

		for (int i = 0; i < events_.Count; ++i) {
			gameEvent = events_[i];

			// make sure the event matches the mandatory criteria, and that this event is eligible
			if (gameEvent.Mandatory == mandatory)
			{	if (CanSpawn (gameEvent)) {
					// check if that event is sleeping
					if (sleepingEventIDs_.ContainsKey (gameEvent.Id)) {
						// if so, decrement its sleep count
						--sleepingEventIDs_ [gameEvent.Id];
						// and if it's now zero, remove it from sleeping
						if (sleepingEventIDs_ [gameEvent.Id] == 0) {
							sleepingEventIDs_.Remove (gameEvent.Id);
						}
					} else {						
						int actualWeight = GetEventWeightForSpawn (gameEvent);
						//Debug.LogWarning (gameEvent.Id + ":" + actualWeight);
						pool.AddToPool (gameEvent, actualWeight);
					}
				} else {
					//Debug.LogWarning ("Should NOT be able to spawn");
				}
			}
		}
		return pool;
	}

	// gets the actual weight of an event in preparation for spawn, and also increments weight values
	private int GetEventWeightForSpawn(GameEvent gameEvent)
	{
		Debug.Log ("GetEventWeightForSpawn: " + gameEvent.Id);
		int actualWeight = gameEvent.Weight.Value;
		IntNull weightPerTurn = gameEvent.WeightPerTurn;
		int maxWeight = int.MaxValue;
		int minWeight = int.MinValue;

		// if we're already keeping track of the weight, update it
		if (eventWeights_.ContainsKey (gameEvent.Id)) {
			actualWeight = eventWeights_ [gameEvent.Id];
			if (weightPerTurn.Defined) {
				actualWeight += weightPerTurn.Value;
			}
			if (gameEvent.MaxWeight.Defined) {
				maxWeight = gameEvent.MaxWeight.Value;
			}
			if (gameEvent.MinWeight.Defined) {
				minWeight = gameEvent.MinWeight.Value;
			}
			actualWeight = Mathf.Clamp (actualWeight, minWeight, maxWeight);
			eventWeights_ [gameEvent.Id] = actualWeight;
		} else {
			// otherwise, just put it in the storage
			eventWeights_.Add (gameEvent.Id, actualWeight);
		}
		return actualWeight;
	}

	public void SetFlag(string flag)
	{
		if (!flags_.ContainsKey (flag)) {
			flags_.Add (flag, true);
		}
	}

	public void ClearFlag(string flag)
	{
		if (flags_.ContainsKey (flag)) {
			flags_.Remove (flag);
		}
	}

	public bool IsFlagSet(string flag)
	{
		if (flags_.ContainsKey (flag)) {
			return flags_ [flag];
		}
		return false;
	}

	public int NumTurns()
	{
		return numTurns_;
	}

	public void SetNumTurns(int numTurns)
	{
		numTurns_ = numTurns;
	}

	public bool IsEventCompleted(string id)
	{
		if (completedEventIDs_.ContainsKey (id)) {
			return completedEventIDs_ [id];
		}
		return false;
	}

	public void SetName(string key, string name)
	{
		//Debug.LogError ("Setting: " + key + " to " + name);
		nameBank_.Add (key, name);
	}

	public string GetName(string key){
		if (nameBank_.ContainsKey (key)) {
			return nameBank_ [key];
		}
		return "";
	}

	public void SetEventCompleted(GameEvent gameEvent, Choice choice)
	{
		if (!completedEventIDs_.ContainsKey (gameEvent.Id)){
			completedEventIDs_.Add (gameEvent.Id, true);
		}
		if (gameEvent.OccurOnlyOnce || choice.LastResult.NeverAgain) {
			neverSpawnEventIDs_.Add (gameEvent.Id, true);
		}
		if (choice.LastResult.SleepFor.Defined) {
			sleepingEventIDs_.Add (gameEvent.Id, choice.LastResult.SleepFor.Value);
		}

		++numTurns_;
		//Debug.LogWarning ("Num turns is now..." + numTurns_);
	}

	public void SetEventIDCompleted(string id)
	{
		if (!completedEventIDs_.ContainsKey (id)) {
			completedEventIDs_.Add (id, true);
		}
	}

	public void NeverSpawnEventID(string id)
	{
		//Debug.LogError ("NEVER SPAWN : " + id);
		if (!neverSpawnEventIDs_.ContainsKey (id)) {
			neverSpawnEventIDs_.Add (id, true);
		}
	}

	public void SleepEventID(string id, int amount)
	{
		sleepingEventIDs_.Add (id, amount);
	}

	public void SetEventWeight(string id, int amount)
	{
		eventWeights_.Add (id, amount);
	}

	public bool CanSpawn(GameEvent gameEvent)
	{
		//if GameEventManager.Instance.
		//Debug.LogError("================ Checking if " + gameEvent.Id + " can spawn");
		//Debug.LogError (!neverSpawnEventIDs_.ContainsKey (gameEvent.Id));
		return (!neverSpawnEventIDs_.ContainsKey (gameEvent.Id) && TriggersTriggered (gameEvent.Triggers));
	}
}


