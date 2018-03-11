﻿using System.Collections;
using System.Collections.Generic;
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
	Dictionary<string, bool> flags_;
	Dictionary<string, bool> completedEventIDs_;

	public GameEventManager()
	{
		events_ = new List<GameEvent> ();
		flags_ = new Dictionary<string, bool>();
		completedEventIDs_ = new Dictionary<string, bool>();
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

	public void LoadEvents()
	{
		XmlDocument xmlDoc = new XmlDocument(); // xmlDoc is the new xml document.
		xmlDoc.Load(eventXMLFile_);
		XmlNodeList eventList = xmlDoc.GetElementsByTagName("event"); // array of the level nodes. 

		foreach (XmlNode eventInfo in eventList) {
			GameEvent gameEvent = new GameEvent(eventInfo);
			events_.Add(gameEvent);
		}
	}

	public GameEvent SpawnEvent()
	{
		WeightedPool<GameEvent> pool = GetWeightedPool (true);
		GameEvent gameEvent;

		if (pool.Count == 0) {
			pool = GetWeightedPool(false);
		}

		if (pool.Count == 0) {
			Debug.LogError ("Tried to spawn an event, but found none were eligible.");
		}

		gameEvent = pool.GetRandomItem();
		if (gameEvent.OccurOnlyOnce) {
			gameEvent.NeverSpawnAgain = true;
		}

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
			if (gameEvent.Mandatory == mandatory && gameEvent.CanSpawn()) {
				pool.AddToPool (gameEvent, gameEvent.Weight.Value);
			}
		}
		return pool;
	}

	public void SetFlag(string flag)
	{
		flags_.Add (flag, true);
	}

	public void ClearFlag(string flag)
	{
		flags_.Remove (flag);
	}

	public bool IsFlagSet(string flag)
	{
		if (flags_.ContainsKey (flag)) {
			return flags_ [flag];
		}
		return false;
	}

	public void SetEventCompleted(string id)
	{
		completedEventIDs_.Add (id, true);
	}

	public bool IsEventCompleted(string id)
	{
		if (completedEventIDs_.ContainsKey (id)) {
			return completedEventIDs_ [id];
		}
		return false;
	}
}


