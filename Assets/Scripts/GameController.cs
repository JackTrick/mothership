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

	private bool gameOver_;

	public InventoryUI inventoryUI_;

	// Use this for initialization
	void Start () {
		stateMachine_ = new StateMachine ();

		gameOver_ = false;

		ship_ = new Ship ();
		GameEventManager.Instance.LoadEvents ();

		//stateMachine_.PushState (new StartState ());

		stateMachine_.PushState (new EventState ());
		SaveManager.Instance.LoadDefaultFile();
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
		/*
		ship_.ChangePopulation (-10000);
		if (ship_.Population == 0) {
			ShowConclusion ();
		} else {
			ShowShipStatus ();
		}
		*/
		if (stateMachine_.currentState is EventState) {
			if (!gameOver_) {
				EventState state = (EventState)stateMachine_.currentState;
				state.NextEvent ();
			} else {
				ShowConclusion ();
			}
		}
	}

	public void GameLoaded()
	{
		stateMachine_.PopAll ();
		stateMachine_.PushState (new EventState ());
		InventoryChanged ();
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
		ItemManager.Instance.Reset ();
		InventoryChanged ();
		inventoryUI_.Reset ();
		GameEventManager.Instance.Reset ();


		stateMachine_.PopAll ();
		stateMachine_.PushState (new EventState ());
	}

	/******
	 * 
	 * 
	 ******/

	public void MakeEventChoice(Choice choice)
	{
		if (stateMachine_.currentState is EventState) {
			EventState state = (EventState)stateMachine_.currentState;

			choice.PerformChallengeSetResult ();
			// TODO deduct costs for the choice, update the inventory
			DeductCosts(choice);
			gameOver_ = ExecuteResult (choice.LastResult);
			state.MakeEventChoice(choice, gameOver_);
		}
	}

	private void DeductCosts(Choice choice)
	{
		List<Cost> costs = choice.Costs;
		foreach (Cost cost in costs) {
			bool percent = cost.Percent.Defined;
			int amount = 0;
			if(percent){
				amount = cost.Percent.Value;
			}
			else{
				amount = -1*cost.Amount.Value;
			}

			ItemManager.Instance.ChangeItemAmount (cost.ItemType, amount, percent);
		}
		InventoryChanged ();
	}

	private bool ExecuteResult(Result result)
	{
		List<ResultEffect> effects = result.Effects;

		bool gameOver = false;

		foreach(ResultEffect effect in effects)
		{
			bool percent = effect.Percent.Defined;
			int amount = 0;
			if(percent){
				amount = effect.Percent.Value;
			}
			else{
				amount = effect.Amount.Value;
			}

			switch(effect.Type){
			case ResultEffect.ResultEffectType.SetItemAmount:
				ItemManager.Instance.SetItemAmount(effect.Value, amount);
				break;
			case ResultEffect.ResultEffectType.ChangeItemAmount:
				ItemManager.Instance.ChangeItemAmount(effect.Value, amount, percent);
				break;
			case ResultEffect.ResultEffectType.AddBuff:
				// TODO
				break;
			case ResultEffect.ResultEffectType.SetName:
				GameEventManager.Instance.SetName (effect.Value, effect.Name);
				break;
			case ResultEffect.ResultEffectType.SetFlag:
				GameEventManager.Instance.SetFlag(effect.Value);
				break;
			case ResultEffect.ResultEffectType.ClearFlag:
				GameEventManager.Instance.ClearFlag(effect.Value);
				break;
			case ResultEffect.ResultEffectType.SetItemProducer:
				ItemManager.Instance.SetItemProducer(effect.Value, effect.Amount.Value, effect.TurnsToProduce.Value);
				break;
			case ResultEffect.ResultEffectType.ChangeItemProducer:
				ItemManager.Instance.ChangeItemProducer(effect.Value, effect.Amount.Value, effect.TurnsToProduce.Value);
				break;
			case ResultEffect.ResultEffectType.SetItemCap:
				ItemManager.Instance.SetItemCap(effect.Value, amount, percent);
				break;
			case ResultEffect.ResultEffectType.ChangeItemCap:
				ItemManager.Instance.ChangeItemCap(effect.Value, amount, percent);
				break;
			case ResultEffect.ResultEffectType.EndGame:
				gameOver = true;
				break;
			}
		}
		InventoryChanged ();

		return gameOver;
	}

	public void InventoryChanged()
	{
		inventoryUI_.RenderInventory();
	}
}
