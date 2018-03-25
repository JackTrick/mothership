using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
	private static ItemManager instance_;
	public static ItemManager Instance { get { return instance_; } }

	// to save
	Dictionary<string, Item> inventory_;
	public Dictionary<string, Item> Inventory { get { return inventory_; } }

	[SerializeField] 
	private List<string> hideTheseItems_;
	private Dictionary<string, bool> hidden_;

	public ItemManager ()
	{
		inventory_ = new Dictionary<string, Item> ();
		hidden_ = new Dictionary<string, bool> ();
	}

	public void ResetSave()
	{
		inventory_ = new Dictionary<string, Item> ();
	}

	public void SetInventoryItem(string itemType, Item item)
	{
		inventory_.Add (itemType, item);
	}

	public bool SaveProgress(StreamWriter writer)
	{
		writer.WriteLine ("[inventory]");
		Item item;
		foreach (KeyValuePair<string, Item> entry in inventory_) {
			item = entry.Value;
			writer.WriteLine (entry.Key);
			writer.WriteLine (item.Amount);
			writer.WriteLine (item.Cap.Defined);
			writer.WriteLine (item.Cap.Value);
			writer.WriteLine (item.ProducePer.Defined);
			writer.WriteLine (item.ProducePer.Value);
			writer.WriteLine (item.TurnsToProduce.Defined);
			writer.WriteLine (item.TurnsToProduce.Value);
			writer.WriteLine (item.TurnCounter);
		}
		return true;
	}

	public void TurnPassed()
	{
		Item item;
		foreach (KeyValuePair<string, Item> entry in inventory_) {
			item = entry.Value;
			item.TurnPassed ();
		}
	}

	public void Reset()
	{
		inventory_.Clear ();
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

	public Item GetItem(string itemName)
	{
		Item item = null;

		if (inventory_.ContainsKey (itemName)) {
			item = inventory_ [itemName];
		}

		return item;
	}

	public int GetItemAmount(string itemName)
	{
		int amount = 0;

		if (inventory_.ContainsKey (itemName)) {
			amount = inventory_ [itemName].Amount;
		}

		return amount;
	}

	public IntNull GetItemCap(string itemName)
	{
		IntNull cap = new IntNull ();

		if (inventory_.ContainsKey (itemName)) {
			cap = inventory_ [itemName].Cap;
		}

		return cap;
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
		Item item = null;
		if (inventory_.ContainsKey (itemName)) {
			item = inventory_ [itemName];
		}
		else{
			if (amount > 0) {
				item = new Item (itemName);
				inventory_ [itemName] = item;
			}
		}

		if (!percent) {				
			item.Amount += amount;
		} else {
			item.Amount = (int)(Mathf.Round(item.Amount * amount / 100.0f));
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

