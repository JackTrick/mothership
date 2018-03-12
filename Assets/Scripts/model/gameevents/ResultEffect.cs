using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

/*
 * 
	<effect type="ChangeItemAmount" value="Power" amount="-150"/>
	<effect type="ChangeItemAmount" value="Power" percent="20"/>
 * 
 */
public class ResultEffect
{
	public enum ResultEffectType
	{
		SetItemAmount,
		ChangeItemAmount,
		AddBuff,
		SetFlag,
		ClearFlag,
		SetItemProducer,
		ChangeItemProducer,
		SetItemCap,
		ChangeItemCap,
		SetName,
		EndGame
	};

	private ResultEffectType type_;
	private string value_;
	private string name_;
	private IntNull amount_;
	private IntNull percent_;
	private IntNull turnsToProduce_;
	private bool hidden_ = false;

	public ResultEffect (XmlNode info)
	{
		LoadFromXML (info);
	}

	private bool LoadFromXML(XmlNode info)
	{
		bool success = true;
		string reason = "";

		amount_ = new IntNull ();
		percent_ = new IntNull ();
		turnsToProduce_ = new IntNull ();

		XmlAttribute xmlAttr = info.Attributes ["type"];
		if (xmlAttr != null) {
			string tempString = XMLHelper.FetchString (xmlAttr);
			if (tempString == "setitemamount") {
				type_ = ResultEffectType.SetItemAmount;
			} else if (tempString == "changeitemamount") {
				type_ = ResultEffectType.ChangeItemAmount;
			} else if (tempString == "addbuff") {
				type_ = ResultEffectType.AddBuff;
			} else if (tempString == "setflag") {
				type_ = ResultEffectType.SetFlag;
			} else if (tempString == "clearflag") {
				type_ = ResultEffectType.ClearFlag;
			} else if (tempString == "setitemproducer") {
				type_ = ResultEffectType.SetItemProducer;
			} else if (tempString == "changeitemproducer") {
				type_ = ResultEffectType.ChangeItemProducer;
			} else if (tempString == "setitemcap") {
				type_ = ResultEffectType.SetItemCap;
			} else if (tempString == "changeitemcap") {
				type_ = ResultEffectType.ChangeItemCap;
			} else if (tempString == "endgame") {
				type_ = ResultEffectType.EndGame;
			} else if (tempString == "setname") {
				type_ = ResultEffectType.SetName;
			} else {
				success = false;
				reason = "invalid type tag: '" + tempString + "' for result effect";
			}
		} else {
			success = false;
			reason = "no type defined for result effect";
		}

		if (success) {
			success = XMLHelper.SetUniqueStringFromAttribute (info, ref value_, "value");
		}
		if (success) {
			success = XMLHelper.SetUniqueStringFromAttribute (info, ref value_, "name");
		}
		if (success) {
			success = XMLHelper.SetUniqueIntFromAttribute (info, ref amount_, "amount");
		}
		if (success) {
			success = XMLHelper.SetUniqueIntFromAttribute (info, ref percent_, "percent");
		}
		if (success) {
			success = XMLHelper.SetUniqueIntFromAttribute (info, ref turnsToProduce_, "turns_to_produce");
		}

		if (success) {
			xmlAttr = info.Attributes ["hidden"];
			if (xmlAttr != null) {
				hidden_ = XMLHelper.FetchBool (xmlAttr);
			}
		}

		if (!success) {
			Debug.LogError ("Error loading result effect XML: " + reason);
		}

		return success;
	}
}


