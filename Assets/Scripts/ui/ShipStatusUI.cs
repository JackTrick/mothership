using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipStatusUI : MonoBehaviour
{
	[SerializeField]
	private Text populationText_;

	public ShipStatusUI()
	{		
	}

	public void Awake()
	{
		populationText_.text = "Population: " + GameController.Instance.PlayerShip.Population;
	}

	public void NextEventClick()
	{
		GameController.Instance.TriggerNextEvent ();
	}
}


