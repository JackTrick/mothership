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
	private Dictionary<string, string> nameBank_;

	public GameEventManager()
	{
		events_ = new List<GameEvent> ();
		flags_ = new Dictionary<string, bool>();
		nameBank_ = new Dictionary<string, string>();
		completedEventIDs_ = new Dictionary<string, bool>();
		neverSpawnEventIDs_ = new Dictionary<string, bool>();
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
	}

	public bool SaveProgress(StreamWriter writer)
	{
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
		return true;
	}

	public bool TriggersTriggered(List<Trigger> triggers)
	{
		if (triggers == null)
			return true;
		
		bool valid = true;
		foreach (Trigger trigger in triggers) {
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
		}
		return valid;
	}

	public void Reset()
	{
		flags_.Clear ();
		nameBank_.Clear ();
		completedEventIDs_.Clear ();
		neverSpawnEventIDs_.Clear ();

		// TODO, reset events rather than clearing and reloading them
		events_.Clear ();
		LoadEvents ();
	}

	public void LoadEvents()
	{
		Debug.Log ("Loading events");
		XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
		xmlDoc.Load(eventXMLFile_);
		XmlNodeList eventList = xmlDoc.GetElementsByTagName("event"); // array of the level nodes. 

		foreach (XmlNode eventInfo in eventList) {
			GameEvent gameEvent = new GameEvent(eventInfo);
			events_.Add(gameEvent);
		}
		Debug.Log ("Events loaded");
	}

	public GameEvent SpawnEvent()
	{
		WeightedPool<GameEvent> pool = GetWeightedPool (true);
		GameEvent gameEvent;

		if (pool.Count == 0) {
			pool = GetWeightedPool(false);
		}

		if (pool.Count == 0) {
			// TODO, end the game if this happens
			Debug.LogError ("Tried to spawn an event, but found none were eligible.");
		}

		gameEvent = pool.GetRandomItem();

		Debug.Log ("Spawning event: " + gameEvent);
		return gameEvent;
	}

	// put together a pool of eligible events, checking if they're mandatory or not.
	public WeightedPool<GameEvent> GetWeightedPool(bool mandatory)
	{
		WeightedPool<GameEvent> pool = new WeightedPool<GameEvent> ();

		GameEvent gameEvent;

		for (int i = 0; i < events_.Count; ++i) {
			gameEvent = events_[i];

			// make sure the event matches the mandatory criteria, and that this event is eligible
			if (gameEvent.Mandatory == mandatory && CanSpawn(gameEvent)) {
				pool.AddToPool (gameEvent, gameEvent.Weight.Value);
			}
		}
		return pool;
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

	public void SetEventCompleted(GameEvent gameEvent)
	{
		if (!completedEventIDs_.ContainsKey (gameEvent.Id)){
			completedEventIDs_.Add (gameEvent.Id, true);
		}
		if (gameEvent.OccurOnlyOnce) {
			neverSpawnEventIDs_.Add (gameEvent.Id, true);
		}

	}

	public void SetEventIDCompleted(string id)
	{
		if (!completedEventIDs_.ContainsKey (id)) {
			completedEventIDs_.Add (id, true);
		}
	}

	public void NeverSpawnEventID(string id)
	{
		if (!neverSpawnEventIDs_.ContainsKey (id)) {
			neverSpawnEventIDs_.Add (id, true);
		}
	}

	public bool CanSpawn(GameEvent gameEvent)
	{
		//if GameEventManager.Instance.
		return (!neverSpawnEventIDs_.ContainsKey (gameEvent.Id) && TriggersTriggered (gameEvent.Triggers));
	}
}


