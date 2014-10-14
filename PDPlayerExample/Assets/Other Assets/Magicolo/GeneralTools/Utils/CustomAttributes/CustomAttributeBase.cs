using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
public class CustomAttributeBase : PropertyAttribute {
	
	public string PrefixLabel = "";
	public bool NoPrefixLabel = false;
	public bool NoFieldLabel = false;
	public bool NoIndex = false;
}
