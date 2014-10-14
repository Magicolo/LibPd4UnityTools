using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections;

public static class ObjectExtensions {

	public static void Remove(this UnityEngine.Object obj) {
		if (obj != null) {
			if (Application.isPlaying)
				UnityEngine.Object.Destroy(obj);
			else
				UnityEngine.Object.DestroyImmediate(obj);
		}
	}
		
	public static void DisconnectPrefab(this UnityEngine.Object obj) {
		#if UNITY_EDITOR
		UnityEditor.PrefabUtility.DisconnectPrefabInstance(obj);
		#endif
	}
	
	public static T[] SendMessageToObjectsOfType<T>(this UnityEngine.Object obj, string methodName, object value, bool sendToSelf, SendMessageOptions options = SendMessageOptions.DontRequireReceiver) where T : Component {
		List<T> objects = new List<T>();
		foreach (T element in UnityEngine.Object.FindObjectsOfType<T>()) {
			if (!sendToSelf && element == obj) {
				continue;
			}
			element.SendMessage(methodName, value, options);
			objects.Add(element);
		}
		return objects.ToArray();
	}
	
	public static T[] SendMessageToObjectsOfType<T>(this UnityEngine.Object obj, string methodName, bool sendToSelf = false, SendMessageOptions options = SendMessageOptions.DontRequireReceiver) where T : Component {
		return obj.SendMessageToObjectsOfType<T>(methodName, obj, sendToSelf, options);
	}

	public static T Clone<T>(this T toClone) {
		if (!typeof(T).IsSerializable) {
			throw new ArgumentException("The type must be serializable.", "source");
		}

		if (object.ReferenceEquals(toClone, null)) {
			return default(T);
		}

		IFormatter formatter = new BinaryFormatter();
		Stream stream = new MemoryStream();
		using (stream) {
			formatter.Serialize(stream, toClone);
			stream.Seek(0, SeekOrigin.Begin);
			return (T)formatter.Deserialize(stream);
		}
	}
	
	public static void Copy<T>(this T copyTo, T copyFrom, params string[] parametersToIgnore) where T : class {
		if (typeof(Component).IsAssignableFrom(typeof(T))) {
			List<string> parametersToIgnoreList = new List<string>(parametersToIgnore);
			parametersToIgnoreList.Add("name");
			parametersToIgnoreList.Add("tag");
			if (!(copyFrom is MonoBehaviour)) {
				parametersToIgnoreList.Add("mesh");
				parametersToIgnoreList.Add("material");
				parametersToIgnoreList.Add("materials");
			}
			parametersToIgnore = parametersToIgnoreList.ToArray();
		}
		
		foreach (FieldInfo fieldInfo in copyFrom.GetType().GetFields()) {
			if ((fieldInfo.IsPublic || fieldInfo.GetCustomAttributes(typeof(SerializeField), true).Length != 0) && !fieldInfo.IsLiteral && fieldInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0 && !parametersToIgnore.Contains(fieldInfo.Name)) {
				try {
					fieldInfo.SetValue(copyTo, fieldInfo.GetValue(copyFrom));
				}
				catch {
				}
			}
		}
		foreach (PropertyInfo propertyInfo in copyFrom.GetType().GetProperties()) {
			if (propertyInfo.CanWrite && propertyInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length == 0 && !parametersToIgnore.Contains(propertyInfo.Name)) {
				try {
					propertyInfo.SetValue(copyTo, propertyInfo.GetValue(copyFrom, null), null);
				}
				catch {
				}
			}
		}
	}
}
