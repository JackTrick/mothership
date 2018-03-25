using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System;
using System.Linq;


public class Choice
{
	/*
		<choice>
			<name>Electrify the Hull</name>
			<desc>Zap 'em!</desc>
			<costs>
				<cost item="Power">
					<amount>50</amount>
				</cost>
			</costs>

			<results type="success">
				<desc>The smell of crispy bug permeates the ship, but the threat has ended.</desc>
				<new_weight>-20</new_weight>
			</results>
			<results type="failure">
				<desc>A power conduit explodes, frightening off the bugs but damaging your space batteries.</desc>
				<new_weight>-20</new_weight>
				<result type="ChangeItemAmount">
					<value>Power</value>
					<amount>-150</amount>
				</result>
			</results>
			<results type="success_crit">
				<desc>Satiated by the power infusion, ancient bio-coded programming awakens in the termites, who begin to perform maintenence on the ship.</desc>
				<new_weight>-20</new_weight>
				<result type="AddBuff">
					<value>NiceTermites</value>
				</result>
				<result type="SetFlag">
					<value>TermiteChain</value>
				</result>
			</results>
			<results type="failure_crit">
				<desc>A power conduit explodes, frightening off the bugs and setting off a systemic power failure.</desc>
				<new_weight>-20</new_weight>
				<result type="ChangeItemAmount">
					<value>Power</value>
					<amount>-350</amount>
				</result>
				<result type="AddBuff">
					<value>ShipShock</value>
				</result>
			</results>
		</choice>
		<choice id="1">
			<name>Fly through a Nebula</name>
			<desc>Nebulas generate electricity, right?</desc>
			<challenges>
				<challenge type="Astrogation">
					<level>2</level>
					<min_fail>10</min_fail>
				</challenge>
			</challenges>
		</choice>
		<choice id="1">
			<name>Do nothing.</name>
			<desc>Doesn't "bug" me lmao</desc>
		</choice>
		<choice>
			<name>Let 'The Gleeber' handle it.</name>
			<desc>GLEEB IT!</desc>
			<triggers>
				<trigger type="Artifact">
					<specific>Gleeber</specific>
					<amount>1</amount>
				</trigger>
			</triggers>	
			<costs>
				<cost item="Gleeber">
				<amount>1</amount>
				</cost>
			</costs>

			<results>
				<desc>The termites are gleebed to within an inch of their lives, and their stunned bodies drift away.</desc>
				<new_weight>-20</new_weight>
				<result type="SetFlag">
					<value>gleebed</value>
				</result>
			</results>
		</choice>	
	 */

	// id_ is used for linking the choice to a generically defined result
	private string id_;
	public string ID { get { return id_; } }

	private string name_;
	public string Name { get { return name_; } } 
	private string desc_;
	public string Desc { get { return desc_; } } 

	private List<Cost> costs_;
	public List<Cost> Costs { get { return costs_; } }
	private List<Trigger> triggers_;
	private List<Challenge> challenges_;

	private Result resultAny_;
	private Result resultSuccess_;
	private Result resultFailure_;
	private Result resultSuccessCrit_;
	private Result resultFailureCrit_;

	private string lastChallengeString_;
	public string LastChallengeResultString { get { return lastChallengeString_; } }
	private Result lastResult_;
	public Result LastResult { get { return lastResult_; } }

	public Choice()
	{

	}

	public string GetEffectsString()
	{
		string ret = "";
		if (costs_ != null) {
			for (int i = 0; i < costs_.Count; ++i) {
				Cost cost = costs_ [i];
				if (i > 0) {
					ret += ", ";
				}
				if (cost.Percent.Defined) {
					ret += "-" + (cost.Percent.Value) + "%";
				} else {
					ret += (-1*cost.Amount.Value) + "";
				}
				ret += " " + cost.ItemType;
			}
		}
		return ret;

	}

	public Choice(XmlNode info, string eventID = "")
	{
		LoadFromXML (info, eventID);
		if (costs_ == null) {
			costs_ = new List<Cost> ();
		}
		if (triggers_ == null) {
			triggers_ = new List<Trigger> ();
		}
		if (challenges_ == null) {
			challenges_ = new List<Challenge> ();
		}
	}

	public bool CanShowChoice()
	{
		return GameEventManager.Instance.TriggersTriggered (triggers_);
	}

	public bool CanAffordChoice()
	{
		//Debug.LogError ("Checking can afford choice");
		bool valid = true;
		int want = 0;
		int have = 0;
		foreach (Cost c in costs_) {
			if (c.Amount.Defined) {
				want = c.Amount.Value;
				have = ItemManager.Instance.GetItemAmount (c.ItemType);
				//Debug.LogError (c.ItemType + "Want: " + want + "  have: " + have);
				if (have < want) {
					return false;
				}
			}
		}

		return valid;
	}

	private void LoadFromXML(XmlNode info, string eventID)
	{
		bool success = true;
		string reason = "";
		
		XmlAttribute xmlAttr = info.Attributes ["id"];
		if (xmlAttr != null) {
			id_ = XMLHelper.FetchString (xmlAttr);
		} 

		if (success) {
			XmlNodeList content = info.ChildNodes;
			foreach (XmlNode xmlItem in content) {
				if (xmlItem.Name == "name") {
					success = XMLHelper.SetUniqueString (xmlItem, ref name_, "name");
				} else if (xmlItem.Name == "desc") {
					success = XMLHelper.SetUniqueString (xmlItem, ref desc_, "desc");
				} else if (xmlItem.Name == "costs") {
					success = XMLHelper.LoadXmlList (xmlItem, ref costs_, "costs", "cost", eventID);
				} else if (xmlItem.Name == "challenges") {
					success = XMLHelper.LoadXmlList (xmlItem, ref challenges_, "challenges", "challenge", eventID);
				} else if (xmlItem.Name == "triggers") {
					success = XMLHelper.LoadXmlList (xmlItem, ref triggers_, "triggers", "trigger", eventID);
				} else if (xmlItem.Name == "result") {
					Result result = new Result (xmlItem);
					success = HandleResultLoaded (result);
				} else {
					success = false;
					reason = "invalid xml in event id " + eventID + " for choice: " + xmlItem.Name + ", expecting (name, desc, costs, triggers, result)";
				}


				if (!success) {
					break;
				}
			}

		}


		if (!success) {
			Debug.LogError ("Error loading choice XML for event " + eventID + " : " + reason);
		}
	}

	public bool HandleResultLoaded(Result result)
	{
		bool success = true;
		string reason = "";
		if (result.Type == Result.ResultType.Any) {
			if (resultAny_ == null) {
				resultAny_ = result;
			} else {
				success = false;
			}
		} else if (result.Type == Result.ResultType.Success) {
			if (resultSuccess_ == null) {
				resultSuccess_ = result;
			} else {
				success = false;
			}
		} else if (result.Type == Result.ResultType.Failure) {
			if (resultFailure_ == null) {
				resultFailure_ = result;
			} else {
				success = false;
			}
		} else if (result.Type == Result.ResultType.SuccessCrit) {
			if (resultSuccessCrit_ == null) {
				resultSuccessCrit_ = result;
			} else {
				success = false;
			}
		} else if (result.Type == Result.ResultType.FailureCrit) {
			if (resultFailureCrit_ == null) {
				resultFailureCrit_ = result;
			} else {
				success = false;
			}
		} else {
			Debug.LogError ("attempted to define a result with an invalid/undefined type");
			return false;
		}

		if (!success) {
			Debug.LogError ("attempted to define/override a result of type " + result.Type + " that already existed");
		}

		return success;
	}

	public void PerformChallengeSetResult()
	{
		lastResult_ = null;
		lastChallengeString_ = "";

		bool success = true;
		bool crit = true;

		if (challenges_ != null) {
			string challengeString = "";

			if (challenges_.Count > 0) {
				challengeString = "[";
			}

			bool first = true;
			foreach (Challenge challenge in challenges_) {
				
				string type = challenge.Type;
				IntNull level = challenge.Level;
				int levelVal = 0;
				if (level.Defined) {
					levelVal = level.Value;
				}

				// TODO: implement min fail percent
				IntNull minFail = challenge.MinFailPercent;

				int itemAmount = ItemManager.Instance.GetItemAmount (type);
				int diceResult = UnityEngine.Random.Range (1, 6) + UnityEngine.Random.Range (1, 6) + itemAmount - levelVal;

				if (diceResult <= 2) {
					success = false;
					crit = true && crit;
				} else if (diceResult <= 6) {
					success = false;
					crit = false;
				} else if (diceResult <= 9) {
					success = true && success;
					crit = false;
				} else {
					success = true && success;
					crit = true && crit;
				}
				if (!first) {
					challengeString += ", ";
				}
				challengeString += type;
				first = false;
			}

			if (challenges_.Count > 0) {
				challengeString += "]";
			}

			if (challenges_.Count > 0) {
				if (crit) {
					if ((success && resultSuccessCrit_ != null) || (!success && resultFailureCrit_ != null)){
						lastChallengeString_ = "Critical ";
					}
				}
				if (success) {
					lastChallengeString_ += "Success";
				} else {
					lastChallengeString_ += "Miss";
				}

				lastChallengeString_ += ", " + challengeString;
			}
		}


		if (success) {
			if (crit && resultSuccessCrit_ != null) {
				lastResult_ = resultSuccessCrit_;
			} else if (resultSuccess_ != null) {
				lastResult_ = resultSuccess_;
			} else {
				lastResult_ = resultAny_;
			}
		} else {
			if (crit && resultFailureCrit_ != null) {
				lastResult_ = resultFailureCrit_;
			} else if (resultFailure_ != null) {
				lastResult_ = resultFailure_;
			} else {
				lastResult_ = resultAny_;
			}
		}
	}
}


