using UnityEngine;
using System.Collections;

public static class DelegateExtensions {

	public static bool Contains(this System.Delegate del, System.Type type, string methodName) {
		return System.Array.TrueForAll(del.GetInvocationList(), invoker => invoker.Method.DeclaringType == type && invoker.Method.Name == methodName);
	}
	
	public static bool Contains(this System.Delegate del, object obj, string methodName) {
		return del.Contains(obj.GetType(), methodName);
	}
	
	public static bool Contains(this System.Delegate del, string methodName) {
		return System.Array.TrueForAll(del.GetInvocationList(), invoker => invoker.Method.Name == methodName);
	}
}
