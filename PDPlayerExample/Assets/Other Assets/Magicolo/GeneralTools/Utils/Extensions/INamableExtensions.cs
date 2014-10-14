using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Magicolo.GeneralTools;

public static class INamableExtensions {
	
	public static string GetUniqueName<T>(this INamable namable, string newName, string oldName, IList<T> array) where T : INamable {
		int suffix = 0;
		bool uniqueName = false;
		string currentName = "";
		
		while (!uniqueName) {
			uniqueName = true;
			currentName = newName;
			if (suffix > 0) currentName += suffix.ToString();
			
			foreach (INamable element in array) {
				if (element != namable && element.Name == currentName && element.Name != oldName) {
					uniqueName = false;
					break;
				}
			}
			suffix += 1;
		}
		return currentName;
	}
	
	public static string GetUniqueName<T>(this INamable namable, string newName, string oldName, string emptyName, IList<T> array) where T : INamable {
		string name = namable.GetUniqueName(newName, oldName, array);
		if (string.IsNullOrEmpty(newName)) {
			name = namable.GetUniqueName(emptyName, oldName, array);
		}
		return name;
	}
	
	public static string GetUniqueName<T>(this INamable namable, string newName, IList<T> array) where T : INamable {
		return namable.GetUniqueName(newName, namable.Name, array);
	}
	
	public static void SetUniqueName<T>(this INamable namable, string newName, string oldName, string emptyName, IList<T> array) where T : INamable {
		namable.Name = namable.GetUniqueName(newName, oldName, emptyName, array);
	}
	
	public static void SetUniqueName<T>(this INamable namable, string newName, string oldName, IList<T> array) where T : INamable {
		namable.Name = namable.GetUniqueName(newName, oldName, array);
	}
	
	public static void SetUniqueName<T>(this INamable namable, string newName, IList<T> array) where T : INamable {
		namable.Name = namable.GetUniqueName(newName, namable.Name, array);
	}
}