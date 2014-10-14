using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public static class TransformExtensions {

	public static void SetPosition(this Transform transform, Vector3 position, string axis = "XYZ") {
		Vector3 newPosition = transform.position;
		if (axis.Contains("X"))
			newPosition.x = position.x;
		if (axis.Contains("Y"))
			newPosition.y = position.y;
		if (axis.Contains("Z"))
			newPosition.z = position.z;
		transform.position = newPosition;
	}
	
	public static void SetPosition(this Transform transform, float position, string axis = "XYZ") {
		transform.SetPosition(new Vector3(position, position, position), axis);
	}
	
	public static void SetLocalPosition(this Transform transform, Vector3 position, string axis = "XYZ") {
		Vector3 newPosition = transform.localPosition;
		if (axis.Contains("X"))
			newPosition.x = position.x;
		if (axis.Contains("Y"))
			newPosition.y = position.y;
		if (axis.Contains("Z"))
			newPosition.z = position.z;
		transform.localPosition = newPosition;
	}
	
	public static void SetLocalPosition(this Transform transform, float position, string axis = "XYZ") {
		transform.SetLocalPosition(new Vector3(position, position, position), axis);
	}
	
	public static void Translate(this Transform transform, Vector3 translation, string axis) {
		transform.SetPosition(transform.position + translation, axis);
	}
	
	public static void Translate(this Transform transform, float translation, string axis = "XYZ") {
		transform.SetPosition(transform.position + new Vector3(translation, translation, translation), axis);
	}
	
	public static void SetEulerAngles(this Transform transform, Vector3 angles, string axis = "XYZ") {
		Vector3 newAngles = transform.eulerAngles;
		if (axis.Contains("X"))
			newAngles.x = angles.x;
		if (axis.Contains("Y"))
			newAngles.y = angles.y;
		if (axis.Contains("Z"))
			newAngles.z = angles.z;
		transform.eulerAngles = newAngles;
	}
	
	public static void SetEulerAngles(this Transform transform, float angle, string axis = "XYZ") {
		transform.SetEulerAngles(new Vector3(angle, angle, angle), axis);
	}
	
	public static void SetLocalEulerAngles(this Transform transform, Vector3 angles, string axis = "XYZ") {
		Vector3 newAngles = transform.localEulerAngles;
		if (axis.Contains("X"))
			newAngles.x = angles.x;
		if (axis.Contains("Y"))
			newAngles.y = angles.y;
		if (axis.Contains("Z"))
			newAngles.z = angles.z;
		transform.localEulerAngles = newAngles;
	}
	
	public static void SetLocalEulerAngles(this Transform transform, float angle, string axis = "XYZ") {
		transform.SetLocalEulerAngles(new Vector3(angle, angle, angle), axis);
	}
	
	public static void Rotate(this Transform transform, Vector3 rotation, string axis) {
		transform.SetEulerAngles(transform.eulerAngles + rotation, axis);
	}
	
	public static void Rotate(this Transform transform, float rotation, string axis = "XYZ") {
		transform.SetEulerAngles(transform.eulerAngles + new Vector3(rotation, rotation, rotation), axis);
	}
	
	public static void SetLocalScale(this Transform transform, Vector3 scale, string axis = "XYZ") {
		Vector3 newScale = transform.localScale;
		if (axis.Contains("X"))
			newScale.x = scale.x;
		if (axis.Contains("Y"))
			newScale.y = scale.y;
		if (axis.Contains("Z"))
			newScale.z = scale.z;
		transform.localScale = newScale;
	}
	
	public static void SetLocalScale(this Transform transform, float scale, string axis = "XYZ") {
		transform.SetLocalScale(new Vector3(scale, scale, scale), axis);
	}
	
	public static void Scale(this Transform transform, Vector3 scale, string axis = "XYZ") {
		transform.SetLocalScale(transform.localScale + scale, axis);
	}
	
	public static void Scale(this Transform transform, float scale, string axis = "XYZ") {
		transform.SetLocalScale(transform.localScale + new Vector3(scale, scale, scale), axis);
	}
	
	public static void FlipScale(this Transform transform, string axis = "Y") {
		Vector3 flippedScale = transform.localScale;
		
		if (axis.Contains("X"))
			flippedScale.x = -flippedScale.x;
		if (axis.Contains("Y"))
			flippedScale.y = -flippedScale.y;
		if (axis.Contains("Z"))
			flippedScale.z = -flippedScale.z;
		
		transform.localScale = flippedScale;
	}
	
	public static Quaternion LookingAt2D(this Transform transform, Vector3 target, float angleOffset, float damping = 100) {
		Vector3 targetDirection = (target - transform.position).normalized;
		float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg + angleOffset;
		return Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), damping * Time.deltaTime);
	}
	
	public static Quaternion LookingAt2D(this Transform transform, Transform target, float angleOffset, float damping = 100) {
		return transform.LookingAt2D(target.position, angleOffset, damping);
	}
	
	public static Quaternion LookingAt2D(this Transform transform, Vector3 target) {
		return transform.LookingAt2D(target, 0, 100);
	}
	
	public static Quaternion LookingAt2D(this Transform transform, Transform target) {
		return transform.LookingAt2D(target.position, 0, 100);
	}
	
	public static void LookAt2D(this Transform transform, Vector3 target, float angleOffset, float damping = 100) {
		transform.rotation = transform.LookingAt2D(target, angleOffset, damping);
	}
	
	public static void LookAt2D(this Transform transform, Transform target, float angleOffset, float damping = 100) {
		transform.LookAt2D(target.position, angleOffset, damping);
	}
	
	public static void LookAt2D(this Transform transform, Vector3 target) {
		transform.LookAt2D(target, 0, 100);
	}
	
	public static void LookAt2D(this Transform transform, Transform target) {
		transform.LookAt2D(target.position, 0, 100);
	}
	
	public static Transform[] GetChildren(this Transform parent) {
		var children = new List<Transform>();
		if (parent != null) {
			if (parent.childCount > 0) {
				for (int i = 0; i < parent.childCount; i++) {
					Transform child = parent.GetChild(i);
					children.Add(child);
				}
			}
		}
		return children.ToArray();
	}
	
	public static Transform[] GetChildrenRecursive(this Transform parent) {
		var children = new List<Transform>();
		if (parent != null) {
			foreach (Transform child in parent.GetChildren()) {
				children.Add(child);
				if (child.childCount > 0) {
					children.AddRange(child.GetChildrenRecursive());
				}
			}
		}
		return children.ToArray();
	}
	
	public static Transform FindChildRecursive(this Transform parent, string childName) {
		foreach (var child in parent.GetChildrenRecursive()) {
			if (child.name == childName)
				return child;
		}
		return null;
	}
	
	public static Transform AddChild(this Transform parent) {
		return parent.AddChild("");
	}
	
	public static Transform AddChild(this Transform parent, string childName) {
		var child = new GameObject();
		if (!string.IsNullOrEmpty(childName))
			child.name = childName;
		child.transform.Reset();
		child.transform.parent = parent;
		return child.transform;
	}
	
	public static Transform FindOrAddChild(this Transform parent, string childName) {
		Transform child = parent.FindChild(childName) ?? parent.AddChild(childName);
		return child;
	}
	
	public static void SortChildren(this Transform parent) {
		Transform[] children = parent.GetChildren();
		var childrendNames = new List<string>();
		
		foreach (var child in children) {
			childrendNames.Add(child.name);
			child.parent = null;
		}
		
		Array.Sort(childrendNames.ToArray(), children);
		
		foreach (var child in children) {
			child.parent = parent;
		}
	}
	
	public static void SortChildrenRecursive(this Transform parent) {
		parent.SortChildren();
		foreach (Transform child in parent.GetChildren()) {
			if (child.childCount > 0)
				child.SortChildrenRecursive();
		}
	}
	
	public static void SetChildrenActive(this Transform parent, bool value){
		foreach (Transform child in parent.GetChildren()) {
			child.gameObject.SetActive(value);
		}
	}
	
	public static void DestroyChildren(this Transform parent) {
		foreach (Transform child in parent.GetChildren()) {
			if (Application.isPlaying) {
				UnityEngine.Object.Destroy(child.gameObject);
			}
			else {
				UnityEngine.Object.DestroyImmediate(child.gameObject);
			}
		}
	}
	
	public static void DestroyChildrenImmediate(this Transform parent) {
		foreach (Transform child in parent.GetChildren()) {
			UnityEngine.Object.DestroyImmediate(child.gameObject);
		}
	}
	
	public static void Reset(this Transform transform) {
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}
}
