using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship {

	private int population_;
	public int Population { get { return population_; } }

	public Ship()
	{
		population_ = 30000;
	}

	public void ChangePopulation(int amount)
	{
		population_ += amount;
		if(population_ < 0){
			population_ = 0;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
