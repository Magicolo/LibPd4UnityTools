using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public static class ListExtensions {

	public static T Pop<T>(this List<T> list, int index = 0) {
		if (list == null || list.Count == 0)
			return default(T);
			
		T item = list[index];
		list.RemoveAt(index);
		return item;
	}
	
	public static T PopRandom<T>(this List<T> list) {
		if (list == null || list.Count == 0)
			return default(T);
		
		int index = UnityEngine.Random.Range(0, list.Count);
		T item = list[index];
		list.RemoveAt(index);
		return item;
	}
	
	public static List<T> PopRange<T>(this List<T> list, int startIndex, int count) {
		List<T> popped = new List<T>();
		
		for (int i = 0; i < count; i++) {
			popped.Add(list.Pop(i + startIndex));
		}
		return popped;
	}
	
	public static List<T> PopRange<T>(this List<T> list, int count) {
		return list.PopRange(0, count);
	}
}
