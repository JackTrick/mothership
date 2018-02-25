using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreationUI : MonoBehaviour
{
	public CreationUI()
	{
	}

	public void ConfirmCreationClick()
	{
		GameController.Instance.ConfirmShipCreation ();
	}
}


