using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XMLHelper
{
	public XMLHelper ()
	{
	}

	public static bool SetUniqueString(XmlNode item, ref string uniqueString, string name)
	{
		if (uniqueString == null) {
			uniqueString = item.InnerText;
			return true;
		} else {
			Debug.LogError ("found multiple instances of <" + name + ">"  + item.OuterXml);
			return false;
		}
	}

	public static bool SetUniqueString(XmlAttribute xmlAttr, ref string uniqueString, string name, bool forceLower)
	{
		if (uniqueString == null) {
			if (forceLower) {
				uniqueString = (xmlAttr.Value).ToLower();
			} else {				
				uniqueString = (xmlAttr.Value);
			}
			return true;
		} else {
			Debug.LogError ("found multiple instances of <" + name + ">" + xmlAttr.OuterXml);
			return false;
		}
	}

	public static bool SetUniqueStringFromAttribute(XmlNode info, ref string uniqueString, string attributeName, bool forceLower = true)
	{
		XmlAttribute xmlAttr = info.Attributes [attributeName];
		if (xmlAttr == null) {
			return true;
		} 
		return XMLHelper.SetUniqueString (xmlAttr, ref uniqueString, attributeName, forceLower);
	}

	public static bool SetUniqueInt(XmlAttribute xmlAttr, ref IntNull uniqueInt, string name)
	{
		if (uniqueInt == null) {
			uniqueInt = new IntNull (Int32.Parse (xmlAttr.Value));
			return true;
		} else if (!uniqueInt.Defined) {
			uniqueInt.Value = Int32.Parse (xmlAttr.Value);
			return true;
		}
		else {
			Debug.LogError ("found multiple instances of <" + name + ">" + xmlAttr.OuterXml);
			return false;
		}
	}

	public static bool SetUniqueIntFromAttribute(XmlNode info, ref IntNull uniqueInt, string attributeName)
	{
		XmlAttribute xmlAttr = info.Attributes [attributeName];
		if (xmlAttr == null) {
			return true;
		} 
		return XMLHelper.SetUniqueInt (xmlAttr, ref uniqueInt, attributeName);
	}


	public static IntNull FetchIntNull(XmlNode item)
	{
		return new IntNull(Int32.Parse (item.InnerText));
	}

	public static bool FetchBool(XmlAttribute xmlAttr)
	{
		return (xmlAttr.Value).ToLower () == "true";
	}

	public static bool FetchBool(XmlNode item)
	{
		return (item.InnerText).ToLower () == "true";
	}

	public static string FetchString(XmlAttribute xmlAttr, bool lowerCase = true)
	{
		if (lowerCase) {
			return (xmlAttr.Value).ToLower ();
		} else {
			return xmlAttr.Value;
		}
	}

	public static IntNull FetchIntNull(XmlAttribute xmlAttr)
	{
		return new IntNull(Int32.Parse(xmlAttr.Value));
	}

	public static bool LoadXmlList<T>(XmlNode info, ref List<T> list, string listName, string itemName, string id = "(not defined") where T : new()
	{
		bool success = true;

		if (list != null) {
			Debug.LogError ("Error loading XML for " + id + ": multiple <"+listName+"> defined " + info.OuterXml);
			success = false;
			return success;
		}
		list = new List<T>();
		XmlNodeList xmlList = info.ChildNodes;

		foreach (XmlNode xml in xmlList) {
			if (xml.Name == itemName) {
				T item = TypeFactory<T>.Create (xml);
				list.Add (item);
			} else {
				success = false;
				Debug.LogError ("Error loading XML for " + id + ": <"+xml.Name+"> defined in <"+listName+"> when expecting <"+itemName+"> " + xml.OuterXml);
				break;
			}
		}

		return success;
	}

	public static class TypeFactory<T>
	{
		private static Func<XmlNode, T> Func { get; set; }

		static TypeFactory()
		{
			TypeFactory<Listener>.Func = (a) => new Listener(a);
			TypeFactory<Result>.Func = (a) => new Result(a);
			TypeFactory<Trigger>.Func = (a) => new Trigger(a);
			TypeFactory<Cost>.Func = (a) => new Cost(a);
			TypeFactory<Challenge>.Func = (a) => new Challenge(a);
		}

		public static T Create(XmlNode a)
		{
			return Func(a);
		}
	}
}


