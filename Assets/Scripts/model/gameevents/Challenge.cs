using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

/*
	<challenge type="Astrogation" level="2" min_fail_percent="10"/>
*/

public class Challenge
{
	private string type_;
	private IntNull level_;
	private IntNull minFailPercent_;

	public Challenge()
	{
	}

	public Challenge (XmlNode info)
	{
		LoadFromXML (info);
	}

	private bool LoadFromXML(XmlNode info)
	{
		bool success = true;
		string reason = "";

		level_ = new IntNull ();
		minFailPercent_ = new IntNull ();

		if (success) {
			success = XMLHelper.SetUniqueStringFromAttribute (info, ref type_, "type");
		}
		if (success) {
			success = XMLHelper.SetUniqueIntFromAttribute (info, ref level_, "level");
		}
		if (success) {
			success = XMLHelper.SetUniqueIntFromAttribute (info, ref minFailPercent_, "min_fail_percent");
		}

		if (!success) {
			Debug.LogError ("Error loading result XML: " + reason);
		}

		return success;
	}
}

