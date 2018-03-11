using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

/*
	<weight type="change" amount="30"/>
	<weight type="change" percent="20"/>
	<weight type="set" amount="10"/>
	<weight amount="30"/>
 */

public class WeightChange
{
	public enum WeightChangeType
	{
		Change,
		Set
	};

	private WeightChangeType type_;
	private IntNull amount_;
	private IntNull percent_;

	public WeightChange ()
	{
	}

	public WeightChange(XmlNode info)
	{
		LoadFromXML (info);
	}

	public bool LoadFromXML(XmlNode info)
	{
		bool success = true;
		string reason = "";

		amount_ = new IntNull ();
		percent_ = new IntNull ();

		XmlAttribute xmlAttr = info.Attributes ["type"];
		if (xmlAttr != null) {
			string tempString = XMLHelper.FetchString (xmlAttr);

			if (tempString == "change") {
				type_ = WeightChangeType.Change;
			} else if (tempString == "set") {
				type_ = WeightChangeType.Set;
			} else {
				success = false;
				reason = "invalid type tag: '" + tempString + "', was looking for (change, set)";
			}
		} else {
			success = false;
			reason = "did not specify type tag, was looking for (change, set)";
		}

		if (success) {
			success = XMLHelper.SetUniqueIntFromAttribute (info, ref amount_, "amount");
		}
		if (success) {
			success = XMLHelper.SetUniqueIntFromAttribute (info, ref percent_, "percent");
		}

		if (!success) {
			Debug.LogError ("Error loading weight change XML: " + reason + " " + info.OuterXml);
		}

		return success;
	}
}


