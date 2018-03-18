using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
	private static ItemManager instance_;
	public static ItemManager Instance { get { return instance_; } }

	Dictionary<string, Item> inventory_;
	public Dictionary<string, Item> Inventory { get { return inventory_; } }

	[SerializeField] 
	private List<string> hideTheseItems_;
	private Dictionary<string, bool> hidden_;

	public ItemManager ()
	{
		inventory_ = new Dictionary<string, Item> ();
		hidden_ = new Dictionary<string, bool> ();
		// TODO, implement production
	}

	public void SetItemHidden(string itemName)
	{
		hidden_.Add (itemName, true);
	}

	public bool IsItemHidden(string itemName)
	{
		return hidden_.ContainsKey (itemName);
	}

	private void Awake()
	{
		if (instance_ != null && instance_ != this)
		{
			Destroy(this.gameObject);
		} else {
			instance_ = this;
		}

		foreach (string itemName in hideTheseItems_) {
			SetItemHidden (itemName);
		}
	}

	public int GetItemAmount(string itemName)
	{
		int amount = 0;

		if (inventory_.ContainsKey (itemName)) {
			amount = inventory_ [itemName].Amount;
		}

		return amount;
	}

	public void SetItemAmount(string itemName, int amount)
	{
		Item item;
		if (inventory_.ContainsKey (itemName)) {
			item = inventory_ [itemName];
		}
		else{
			item = new Item(itemName);
			inventory_ [itemName] = item;
		}
		item.Amount = amount;
	}

	public void ChangeItemAmount(string itemName, int amount, bool percent = false)
	{
		Item item;
		if (inventory_.ContainsKey (itemName)) {
			item = inventory_ [itemName];
		}
		else{
			item = new Item(itemName);
			inventory_ [itemName] = item;
		}

		if (!percent) {				
			item.Amount += amount;
		} else {
			item.Amount *= (int)(amount / 100.0f);
		}
	}

	public void SetItemProducer(string itemName, int amount, int turnsToProduce)
	{
		Item item;
		if (inventory_.ContainsKey (itemName)) {
			item = inventory_ [itemName];
		}
		else{
			item = new Item(itemName);
			inventory_ [itemName] = item;
		}

		item.ProducePer.Value = amount;
		item.TurnsToProduce.Value = turnsToProduce;
	}

	public void ChangeItemProducer(string itemName, int amount, int turnsToProduce)
	{
		Item item;
		if (inventory_.ContainsKey (itemName)) {
			item = inventory_ [itemName];
		}
		else{
			item = new Item(itemName);
			inventory_ [itemName] = item;
		}

		item.ProducePer.Value += amount;
		item.TurnsToProduce.Value += turnsToProduce;
	}

	public void SetItemCap(string itemName, int amount, bool percent = false)
	{
		Item item;
		if (inventory_.ContainsKey (itemName)) {
			item = inventory_ [itemName];
		}
		else{
			item = new Item(itemName);
			inventory_ [itemName] = item;
		}

		item.Cap.Value = amount;
	}

	public void ChangeItemCap(string itemName, int amount, bool percent = false)
	{
		Item item;
		if (inventory_.ContainsKey (itemName)) {
			item = inventory_ [itemName];
		}
		else{
			item = new Item(itemName);
			inventory_ [itemName] = item;
		}

		item.Cap.Value += amount;
	}
}

