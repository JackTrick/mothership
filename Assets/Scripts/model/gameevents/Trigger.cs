using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class Trigger
{
	/*
		<trigger logic="NOTAND" type="Flag" value="TermiteChain"/>
		<trigger logic="NOTOR" type="Item" value="Power" amount="10"/>
		<trigger logic="NOT" type="Item" value="Power" amount="10"/>
		<trigger logic="AND" type="Item" value="Power" less_than="10"/>
		<trigger logic="OR" type="Item" value="Power" greater_than="50"/>
	 */
	public enum TriggerLogic
	{
		AND,
		OR,
		NOT,
		NOTAND,
		NOTOR
	};

	public enum TriggerType
	{
		Flag,
		Artifact,
		Item,
		Event
	};

	private TriggerLogic logic_;
	public TriggerLogic Logic { get { return logic_; } }
	private TriggerType type_;
	private string value_;
	private IntNull amount_;
	private IntNull lessThan_;

	public Trigger()
	{

	}

	public Trigger(XmlNode triggerInfo)
	{
		LoadFromXML (triggerInfo);
	}

	private void LoadFromXML(XmlNode info)
	{
		bool success = true;
		string reason = "";

		amount_ = new IntNull ();
		lessThan_ = new IntNull ();

		XmlAttribute xmlAttr = info.Attributes ["logic"];
		if (xmlAttr != null) {
			string tempString = XMLHelper.FetchString (xmlAttr);

			if (tempString == "and") {
				logic_ = TriggerLogic.AND;
			} else if (tempString == "or") {
				logic_ = TriggerLogic.OR;
			} else if (tempString == "not") {
				logic_ = TriggerLogic.NOT;
			} else if (tempString == "notand") {
				logic_ = TriggerLogic.NOTAND;
			} else if (tempString == "notor") {
				logic_ = TriggerLogic.NOTOR;
			} else {
				success = false;
				reason = "invalid logic tag: '" + tempString + "', was looking for (and, or, not)";
			}
		} else {
			logic_ = TriggerLogic.AND;
		}

		if(success){
			xmlAttr = info.Attributes ["type"];
			if (xmlAttr != null) {
				string tempString = XMLHelper.FetchString (xmlAttr);

				if (tempString == "flag") {
					type_ = TriggerType.Flag;
				}
				else if(tempString == "artifact"){
					type_ = TriggerType.Artifact;
				}
				else if(tempString == "item"){
					type_ = TriggerType.Item;
				}
				else if(tempString == "event"){
					type_ = TriggerType.Item;
				}
				else {
					success = false;
					reason = "invalid type tag found in trigger: '" + tempString + "'";
				}
			} else {
				type_ = TriggerType.Event;
			}
		}
		if (success) {
			success = XMLHelper.SetUniqueStringFromAttribute (info, ref value_, "value");
		}
		if (success) {
			success = XMLHelper.SetUniqueIntFromAttribute (info, ref amount_, "amount");
		}
		if (success) {
			success = XMLHelper.SetUniqueIntFromAttribute (info, ref lessThan_, "less_than");
		}
		if (success) {
			success = XMLHelper.SetUniqueIntFromAttribute (info, ref amount_, "greater_than");
		}

		if (!success) {
			Debug.LogError ("Error loading trigger XML: " + reason + " " + info.OuterXml);
		}
	}

	public bool Triggered()
	{
		switch(type_)
		{
		case TriggerType.Artifact:
			// TODO
			break;
		case TriggerType.Event:
			if (value_ != null) {
				return GameEventManager.Instance.IsEventCompleted (value_);
			} else {
				Debug.LogError ("Tried to check event trigger, but no event id defined");
			}
			break;
		case TriggerType.Flag:
			return GameEventManager.Instance.IsFlagSet (value_);
			break;
		case TriggerType.Item:
			int inventoryAmount = ItemManager.Instance.GetItemAmount (value_);
			if (amount_.Defined) {
				return inventoryAmount >= amount_.Value;
			} else if (lessThan_.Defined) {
				return inventoryAmount < amount_.Value;
			}
			break;
		}
		return true;
	}
}


