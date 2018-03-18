using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
	[SerializeField]
	private Text title_;

	[SerializeField]
	private GameObject itemPanels_;

	private ItemPanelUI itemPanelPrefab_;
	Dictionary<Item, GameObject> itemToPanels_;

	public InventoryUI ()
	{
		itemToPanels_ = new Dictionary<Item, GameObject> ();
	}

	private void Awake()
	{
		title_.text = "";
		itemPanelPrefab_ = Resources.Load<ItemPanelUI> ("Prefabs/ItemPanel");
	}

	public void RenderInventory()
	{
		// TODO, optimize this so it's not re-rendering every time.
		itemPanels_.transform.DetachChildren ();
		string shipName = "";
		if (GameEventManager.Instance != null) {
			shipName = GameEventManager.Instance.GetName ("shipname");
		}

		if (shipName == "") {
			title_.text = "Ship's Manifest";
		} else {
			title_.text = GameEventManager.Instance.GetName ("shipname") + "'s Manifest";
		}

		Dictionary<string, Item> inventory = ItemManager.Instance.Inventory;

		Item item;
		string amountString;
		string produceString;

		foreach(KeyValuePair<string, Item> entry in inventory)
		{
			amountString = "";
			produceString = "";
			item = entry.Value;
			if(!ItemManager.Instance.IsItemHidden(entry.Key)){

				ItemPanelUI itemPanel = GameObject.Instantiate<ItemPanelUI> (itemPanelPrefab_, itemPanels_.transform);
				itemPanel.name.text = entry.Key;


				amountString = "" + item.Amount;
				if (item.Cap.Defined) {
					amountString += " / " + item.Cap.Value;
				}
				itemPanel.amount.text = amountString;

				if (item.ProducePer.Defined && item.TurnsToProduce.Defined) {
					produceString = item.ProducePer.Value + " every\n";
					if (item.TurnsToProduce.Value == 1) {
						produceString += "turn";
					} else {
						produceString += item.TurnsToProduce + "turns";
					}
				}
				itemPanel.production.text = produceString;
			}
		}
	}

	public void InventoryChanged(Item item)
	{

	}
}


