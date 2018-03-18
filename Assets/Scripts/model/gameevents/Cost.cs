using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

/*
		
			<costs>
				<cost value="Power" amount="50"/>
				<cost value="Power" percent="20"/>
			</costs>
	 */

public class Cost
{
	private string itemType_;
	public string ItemType { get { return itemType_; } }
	private IntNull amount_;
	public IntNull Amount { get { return amount_; } }
	private IntNull percent_;
	public IntNull Percent { get { return percent_; } }

	public Cost()
	{
	}

	public Cost (XmlNode info)
	{
		LoadFromXML (info);
	}

	private bool LoadFromXML(XmlNode info)
	{
		bool success = true;
		string reason = "";

		amount_ = new IntNull ();
		percent_ = new IntNull ();

		if (success) {
			success = XMLHelper.SetUniqueStringFromAttribute (info, ref itemType_, "value");
		}
		if (success) {
			success = XMLHelper.SetUniqueIntFromAttribute (info, ref amount_, "amount");
		}
		if (success) {
			success = XMLHelper.SetUniqueIntFromAttribute (info, ref percent_, "percent");
		}

		if (!success) {
			Debug.LogError ("Error loading result XML: " + reason + " " + info.OuterXml);
		}

		return success;
	}
}


