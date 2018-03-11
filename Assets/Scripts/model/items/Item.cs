using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
	private string id_;
	private int amount_;
	public int Amount { get { return amount_; } set { amount_ = value; } }

	private IntNull cap_;
	private IntNull producePer_;
	private IntNull turnsToProduce_;
	private int turnCounter_;

	public Item ()
	{
		id_ = null;
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


