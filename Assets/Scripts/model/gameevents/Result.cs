using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class Result
{
	/*
	 
		<result type="success">
			<desc>The smell of crispy bug permeates the ship, but the threat has ended.</desc>
			<new_weight>-20</new_weight>
		</result>
		<result type="failure">
			<desc>A power conduit explodes, frightening off the bugs but damaging your space batteries.</desc>
			<new_weight>-20</new_weight>
			<effect type="ChangeItemAmount">
				<value>Power</value>
				<amount>-150</amount>
			</effect>
		</result>
		<result type="success_crit">
			<desc>Satiated by the power infusion, ancient bio-coded programming awakens in the termites, who begin to perform maintenence on the ship.</desc>
			<new_weight>-20</new_weight>
			<effect type="AddBuff">
				<value>NiceTermites</value>
			</effect>
			<effect type="SetFlag">
				<value>TermiteChain</value>
			</effect>
		</result>
		<result type="failure_crit">
			<desc>A power conduit explodes, frightening off the bugs and setting off a systemic power failure.</desc>
			<new_weight>-20</new_weight>
			<effect type="ChangeItemAmount">
				<value>Power</value>
				<amount>-350</amount>
			</effect>
			<effect type="AddBuff">
				<value>ShipShock</value>
			</effect>
		</result>
	 */


	public enum ResultType
	{
		Any,
		Success,
		Failure,
		SuccessCrit,
		FailureCrit
	};

	// id is used for linking it to genericized choices
	private string id_;
	public string ID { get { return id_; } }

	private ResultType type_;
	public ResultType Type { get { return type_; } }

	private string desc_;

	private WeightChange weightChange_;

	private bool neverAgain_ = false;

	private IntNull sleepFor_;

	List<ResultEffect> effects_;

	public Result()
	{

	}

	public Result(XmlNode info)
	{
		LoadFromXML (info);
	}

	private void LoadFromXML(XmlNode info)
	{
		bool success = true;
		string reason = "";

		sleepFor_ = new IntNull ();
		effects_ = new List<ResultEffect> ();

		XmlAttribute xmlAttr = info.Attributes ["type"];
		if (xmlAttr != null) {
			string tempString = XMLHelper.FetchString (xmlAttr);
			if (tempString == "success") {
				type_ = ResultType.Success;
			} else if (tempString == "failure") {
				type_ = ResultType.Failure;
			} else if (tempString == "success_crit") {
				type_ = ResultType.SuccessCrit;
			} else if (tempString == "failure_crit") {
				type_ = ResultType.FailureCrit;
			} else if (tempString == "any") {
				type_ = ResultType.Any;
			} else {
				success = false;
				reason = "invalid type tag: '" + tempString + "', was looking for (success, failure, failure_crit, success_crit, any, or blank)";
			}
		} else {
			type_ = ResultType.Any;
		}

		xmlAttr = info.Attributes ["id"];
		if (xmlAttr != null) {
			id_ = XMLHelper.FetchString (xmlAttr);
		} 

		if (success) {
			XmlNodeList content = info.ChildNodes;
			foreach (XmlNode xmlItem in content) {
				if (xmlItem.Name == "desc") {
					if (desc_ == null) {
						desc_ = xmlItem.InnerText;
					} else {
						success = false;
						reason = "found multiple instances of <desc>";
					}
				} else if (xmlItem.Name == "weight") {
					weightChange_ = new WeightChange (xmlItem);
				} else if (xmlItem.Name == "never_again") {
					neverAgain_ = XMLHelper.FetchBool (xmlItem);
				} else if (xmlItem.Name == "sleep_for") {
					IntNull temp = XMLHelper.FetchIntNull (xmlItem);
					if (temp.Value < 0) {
						success = false;
						reason = "invalid value for sleep_for. negative numbers not valid";
					} else {
						sleepFor_ = temp;
					}						
				} else if (xmlItem.Name == "effect") {
					effects_.Add (new ResultEffect (xmlItem));
				} else {
					success = false;
					reason = "invalid tag found for result: " + xmlItem.Name + ", was expecting desc, weight_inc, weight_set, never_again, effect";
				}

				if (!success) {
					break;
				}
			}
		}

		if (!success) {
			Debug.LogError ("Error loading result XML: " + reason);
		}
	}



}


