using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
	[SerializeField]
	private Text title_;

	Dictionary<Item, GameObject> itemPanels_;

	public InventoryUI ()
	{
		itemPanels_ = new Dictionary<Item, GameObject> ();
	}

	private void Awake()
	{
		string shipName = GameEventManager.Instance.GetName ("shipname");

		if (shipName == "") {
			title_.text = "Ship's Manifest";
		} else {
			title_.text = GameEventManager.Instance.GetName ("shipname") + "'s Manifest";
		}
	}

	public void RenderInventory()
	{

	}

	public void InventoryChanged(Item item)
	{

	}
}


