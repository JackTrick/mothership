using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Xml.Serialization;

/**
 *	Events
 *	requirements/triggers
 *	text
 *	images
 *	multiple choices
 *		availability criteria
 *		effect on success/failure
 *			change stats
 *			set triggers
 *			display result
 *			challenge
 *				difficulty
 *				modifiers
 *				(optional) assignment
 *			reward
 *			
 * Unique individual
 * 	name
 * 	gender
 * 	morale
 * 		personal
 * 		factional
 * 	portait
 * 	species
 * 	faction	
 * 	assigned department
 * 		department traits
 * 		experience bar
 * 	traits
 * 		name
 * 		symbol
 * 		effects
 * 			positive
 * 			negative
 * 		(optional) expiration behavior
 * 
 * Population
 * 	name
 * 	species
 * 	department assigned
 * 	faction
 * 	morale
 * 		personal
 * 		factional
 * 	quantity
 * 
 * Faction
 * 	name
 * 	symbol
 * 	morale
 * 	relationships w/other factions
 * 	morale changers
 * 		action
 * 		morale delta
 * 
 * Resources
 * 	name
 * 	symbol
 * 	quantity
 * 
 * Shuttles
 * 	mission
 * 	individual
 * 
 * Recruits
 * 	name
 * 	portait
 * 	species
 * 	faction
 * 	skill
 * 	experience bar
 * 	gender
 * 	morale (pers/fac)
 * 	hidden potential traits
 * 
 * Ship
 * 
 * Locations
 * 
 * Nemesis
 * 
 * Components
 */


/*
* General ship screen
* 	people (bridge, uniques, recruits)
* 	populations
* 	assignments
* 	actions
*
*
*
* Start of year, allocate resources
* do a bunch of potential actions. Every time you do an action, time passes, and an event triggers
*
*
* arrive at a location, engines cooling off. begin powering engines. engines powered up. 
* these are the seasons
*
* at its smallest level, this could be a game without populations or components.
* it has resoures, people with skill levels, areas of need to be met.
*/
public class GameController : MonoBehaviour {

	private static GameController instance_;
	public static GameController Instance { get { return instance_; } }

	private StateMachine stateMachine_;
	private Ship ship_;
	public Ship PlayerShip { get { return ship_; } }

	// Use this for initialization
	void Start () {
		stateMachine_ = new StateMachine ();

		ship_ = new Ship ();
		GameEventManager.Instance.LoadEvents ();

		//stateMachine_.PushState (new StartState ());
		stateMachine_.PushState (new EventState ());
	}
	
	// Update is called once per frame
	void Update () {
		stateMachine_.Update ();
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

	public void StartGame()
	{
		stateMachine_.PopAll ();
		//stateMachine_.PushState (new CreationState ());
		stateMachine_.PushState (new ShipStatusState ());
	}

	public void ConfirmShipCreation()
	{
		stateMachine_.PopAll ();
		stateMachine_.PushState (new ShipStatusState ());
	}

	public void TriggerNextEvent()
	{
		stateMachine_.PopAll ();
		stateMachine_.PushState (new EventState ());
	}

	public void ConfirmEventChoice()
	{
		ship_.ChangePopulation (-10000);
		if (ship_.Population == 0) {
			ShowConclusion ();
		} else {
			ShowShipStatus ();
		}
	}

	public void ShowShipStatus()
	{
		stateMachine_.PopAll ();
		stateMachine_.PushState (new ShipStatusState ());
	}

	public void ShowConclusion()
	{
		stateMachine_.PopAll ();
		stateMachine_.PushState (new ConclusionState ());
	}

	public void ReturnToStart()
	{
		stateMachine_.PopAll ();
		stateMachine_.PushState (new StartState ());
	}
}
