using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
	private static ItemManager instance_;
	public static ItemManager Instance { get { return instance_; } }

	Dictionary<string, Item> inventory_;

	public ItemManager ()
	{
		inventory_ = new Dictionary<string, Item> ();
	}

	public int GetItemAmount(string item)
	{
		int amount = 0;

		if (inventory_.ContainsKey (item)) {
			amount = inventory_ [item].Amount;
		}

		return amount;
	}
}

