using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedPool<T>
{
	struct WeightedEntry{
		public T item;
		public int weight;
	}

	private List<WeightedEntry> weightedEntries_;

	public int Count { get { return weightedEntries_.Count; } }

	private int totalWeight_;

	public WeightedPool ()
	{
		totalWeight_ = 0;
		weightedEntries_ = new List<WeightedEntry> ();
	}

	public void AddToPool(T item, int weight)
	{
		if (weight > 0) {
			WeightedEntry entry;
			entry.item = item;
			entry.weight = weight;

			totalWeight_ += weight;
			weightedEntries_.Add (entry);
		}
	}

	public T GetRandomItem()
	{
		T ret = default(T);

		int index = Random.Range(0, totalWeight_ - 1);
		int soFar = 0;
		int currentItemWeight = 0;

		foreach (WeightedEntry entry in weightedEntries_) {
			currentItemWeight = entry.weight;
			soFar += currentItemWeight;

			if(soFar > index)
			{
				return entry.item;
			}
		}
		return ret;
	}
}


