using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class Listener
{
	/*
			<listeners>
				<listener listenFor="VerminDeflectorDecay">
					<weight type="change" amount="30"/>
				</listener>
			</listeners>
	 */

	private string listenFor_;

	private WeightChange weightChange_;

	public Listener()
	{

	}

	public Listener(XmlNode info)
	{
		LoadFromXML (info);
	}

	private void LoadFromXML(XmlNode info)
	{
		bool success = true;
		string reason = "";

		XmlAttribute xmlAttr = info.Attributes ["for"];
		if (xmlAttr != null) {
			listenFor_ = XMLHelper.FetchString (xmlAttr);
		} else {
			success = false;
			reason = "listener created, but no 'for' defined " + info.OuterXml;
		}

		if (success) {
			XmlNodeList content = info.ChildNodes;
			foreach (XmlNode xmlItem in content) {
				if (xmlItem.Name == "weight") {
					weightChange_ = new WeightChange (xmlItem);
				} else {
					success = false;
					reason = "listener effect invalid, found " + xmlItem.Name + " was expecting (weight)";
				}

				if (!success) {
					break;
				}
			}

		}


		if (!success) {
			Debug.LogError ("Error loading listener XML: " + reason + " " + info.OuterXml);
		}
	}
}


