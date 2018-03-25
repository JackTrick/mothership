using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: items can have a class type
// TODO: resulteffects can change a random one from the class type
public class Item
{
	private string id_;
	private int amount_;
	public int Amount { 
		get { return amount_; } 
		set { 
			amount_ = value;
			if (cap_.Defined && amount_ > cap_.Value) {
				amount_ = cap_.Value;
			}
			if (amount_ < 0) {
				amount_ = 0;
			}
		} 
	}

	private IntNull cap_;
	public IntNull Cap { 
		get { return cap_; } 
		set { 
			cap_ = value; 
			if (amount_ > cap_.Value) {
				amount_ = cap_.Value;
			}
		} 
	}
	private IntNull producePer_;
	public IntNull ProducePer { get { return producePer_; } set { producePer_ = value; } }
	private IntNull turnsToProduce_;
	public IntNull TurnsToProduce { get { return turnsToProduce_; } set { turnsToProduce_ = value; } }

	private int turnCounter_;
	public int TurnCounter { get { return turnCounter_; } set { turnCounter_ = value; } }

	public Item (string id)
	{
		id_ = id;
		amount_ = 0;
		turnCounter_ = 0;
		cap_ = new IntNull ();
		producePer_ = new IntNull ();
		turnsToProduce_ = new IntNull ();
	}

	// returns the amount to produce
	public void TurnPassed()
	{
		if (producePer_.Defined && turnsToProduce_.Defined) {
			++turnCounter_;
			int produces = (turnCounter_ / turnsToProduce_.Value);
			turnCounter_ = turnCounter_ % (produces * turnsToProduce_.Value);

			amount_ += produces * producePer_.Value;

			int max = Int32.MaxValue;
			if(cap_.Defined){
				max = cap_.Value;
			}
			amount_ = Mathf.Clamp(amount_, 0, max);
		}
	}
}


