using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System.Collections;

public static class TypeExtensions {

	public static object CreateDefaultInstance(this Type type) {
		object instance = null;
		
		if (type == typeof(string)) {
			instance = string.Empty;
		}
		else {
			instance = Activator.CreateInstance(type, type.GetDefaultConstructorParameters());
		}
		
		return instance;
	}
	
	public static object[] GetDefaultConstructorParameters(this Type type) {
		List<object> parameters = new List<object>();
		
		if (!type.HasEmptyConstructor() && type.HasConstructor()) {
			ParameterInfo[] parameterInfos = type.GetConstructors()[0].GetParameters();
		
			foreach (ParameterInfo info in parameterInfos) {
				parameters.Add(info.ParameterType.CreateDefaultInstance());
			}
		}
		
		return parameters.ToArray();
	}
	
	public static bool HasConstructor(this Type type) {
		return type.GetConstructors().Length > 0;
	}
	
	public static bool HasEmptyConstructor(this Type type) {
		return type.GetConstructor(Type.EmptyTypes) != null;
	}
	
	public static bool HasDefaultConstructor(this Type type) {
		return type.IsValueType || type.HasEmptyConstructor();
	}
	
	public static bool IsNumerical(this Type type) {
		return type == typeof(int) || type == typeof(float) || type == typeof(double);
	}
	
	public static bool IsVector(this Type type) {
		return type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4);
	}
}
