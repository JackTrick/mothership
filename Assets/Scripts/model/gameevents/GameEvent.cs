using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System;
using System.Linq;

public class GameEvent {

	private string id_;
	public string Id { get { return id_; } }
	private string name_;
	public string Name { get { return name_; } }
	private string desc_;
	public string Desc { get { return desc_; } } 

	private string imageName_;
	public string ImageName { get { return imageName_; } }
	// related to spawning of the event
	private bool occurOnlyOnce_ = false;
	public bool OccurOnlyOnce { get { return occurOnlyOnce_; } }
	private bool mandatory_ = false;
	public bool Mandatory { get { return mandatory_; } }

	private IntNull weight_;
	public IntNull Weight { get { return weight_; } }

	private IntNull weightPerTurn_;
	public IntNull WeightPerTurn { get { return weightPerTurn_; } }

	// TODO: implement weight per turn percent
	private IntNull weightPerTurnPercent_;

	private IntNull minWeight_;
	public IntNull MinWeight { get { return minWeight_; } }
	private IntNull maxWeight_;
	public IntNull MaxWeight { get { return maxWeight_; } }

	private List<Trigger> triggers_;
	public List<Trigger> Triggers { get { return triggers_; } }
	private List<Listener> listeners_;
	private List<Choice> choices_;
	public List<Choice> Choices { get { return choices_; } } 

	public GameEvent(XmlNode eventInfo)
	{ 
		LoadFromXML (eventInfo);
	}

	private void LoadFromXML(XmlNode eventInfo)
	{
		bool success = true;
		string reason = "";

		weight_ = new IntNull();
		weightPerTurn_ = new IntNull();
		weightPerTurnPercent_ = new IntNull();
		minWeight_ = new IntNull();
		maxWeight_ = new IntNull();

		// grab the id for the event if it exists
		XmlAttribute xmlAttr = eventInfo.Attributes ["id"];
		if (xmlAttr != null) {
			id_ = xmlAttr.Value;
		}

		XmlNodeList content = eventInfo.ChildNodes;

		foreach (XmlNode xmlItem in content) {
			// load the basic info of an event
			if (xmlItem.Name == "name") {
				success = XMLHelper.SetUniqueString (xmlItem, ref name_, "name");
			} else if (xmlItem.Name == "desc") {
				success = XMLHelper.SetUniqueString (xmlItem, ref desc_, "desc");
			} else if (xmlItem.Name == "image") {
				success = XMLHelper.SetUniqueString (xmlItem, ref imageName_, "image");
			} else if (xmlItem.Name == "triggers") {
				success = XMLHelper.LoadXmlList (xmlItem, ref triggers_, "triggers", "trigger", id_);
			} else if (xmlItem.Name == "only_once") {
				occurOnlyOnce_ = XMLHelper.FetchBool (xmlItem);
			} else if (xmlItem.Name == "mandatory") {
				mandatory_ = XMLHelper.FetchBool (xmlItem);
			} else if (xmlItem.Name == "weight") {
				weight_ = XMLHelper.FetchIntNull (xmlItem);
			} else if (xmlItem.Name == "min_weight") {
				minWeight_ = XMLHelper.FetchIntNull (xmlItem);
			} else if (xmlItem.Name == "max_weight") {
				maxWeight_ = XMLHelper.FetchIntNull (xmlItem);
			} else if (xmlItem.Name == "listeners") {
				success = XMLHelper.LoadXmlList (xmlItem, ref listeners_, "listeners", "listener", id_);
			} else if (xmlItem.Name == "weight_per_turn") {
				weightPerTurn_ = XMLHelper.FetchIntNull (xmlItem);
			} else if (xmlItem.Name == "weight_per_turn_percent") {
				weightPerTurn_ = XMLHelper.FetchIntNull (xmlItem);
			}
			else if (xmlItem.Name == "choices") {
				success = LoadChoicesFromXML (xmlItem);
			}

			if (!success) {
				break;
			}

		}

		if (!success) {
			Debug.LogError ("Error loading XML for " + id_ + ": " + reason);
		}
	}

	private bool LoadChoicesFromXML(XmlNode info)
	{
		bool success = true;

		if (choices_ != null) {
			Debug.LogError ("Error loading XML for " + id_ + ": multiple <choices> defined");
			success = false;
			return success;
		}

		choices_ = new List<Choice> ();

		XmlNodeList xmlList = info.ChildNodes;
		List <Result> tempResults = new List<Result>();

		foreach (XmlNode xml in xmlList) {
			if (xml.Name == "choice") {
				choices_.Add (new Choice (xml, id_));
			} else if (xml.Name == "result") {
				tempResults.Add (new Result (xml));
			}
		}

		// match make the results to the choices
		Result resultTemp;
		Choice choiceTemp;
		string choiceID = "";
		string resultID = "";
		bool foundChoice = false;
		for (int i = 0; i < tempResults.Count; ++i) {
			foundChoice = false;
			resultTemp = tempResults[i];
			resultID = resultTemp.ID;

			for (int c = 0; c < choices_.Count; ++c) {
				choiceTemp = choices_ [c];
				choiceID = choiceTemp.ID;

				if (resultID.Equals (choiceID)) {
					success = choiceTemp.HandleResultLoaded (resultTemp);
				}

				if (!success) {
					break;
				} else {
					foundChoice = true;
				}
			}

			if (!foundChoice) {
				Debug.LogWarning ("reconciling generic choices with results, was unable to find a choice that matched");
			}

			if (!success) {
				break;
			}
		}

		if (!success) {
			Debug.LogError ("Error occurred in reconciling generic results with choices");
		}

		return success;
	}

	public override string ToString(){
		return "[Event: "+id_+"]";
	}

}
